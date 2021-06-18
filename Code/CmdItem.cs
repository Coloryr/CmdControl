using CmdControl.Custom;
using CmdControl.Objs;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CmdControl
{
    public class CmdItem
    {
        public string 名字 { get; set; }
        public bool ProcessRun { get; private set; }
        public CmdData CmdData { get; private set; }

        private CmdShow CmdShow;
        private UTabItem UTabItem;
        private Process ThisProcess;
        private StreamWriter StandardInput;
        private Thread Thread;
        private bool User;
        private bool TaskRun;
        private bool Send;

        public CmdItem(CmdData CmdData)
        {
            this.CmdData = CmdData;
            名字 = CmdData.名字;
        }

        public override string ToString()
        {
            return $"实例名字:{CmdData.名字}\n运行应用:{CmdData.路径}\n运行路径:{CmdData.运行路径}\n运行参数:{CmdData.参数}\n运行命令:{CmdData.命令}\n关闭指令:{CmdData.关闭指令}\n自动启动:{CmdData.自动启动}\n远程控制:{CmdData.远程控制}\n自动重启:{CmdData.自动重启}\n启动反馈:{CmdData.启动反馈}\n关闭反馈:{CmdData.关闭反馈}";
        }

        public void StartSend()
        {
            Send = true;
        }

        public void EndSend()
        {
            Send = false;
        }

        private void Check()
        {
            try
            {
                while (ThisProcess != null)
                {
                    if (ThisProcess?.HasExited == true || ThisProcess == null)
                    {
                        OnDo(FC.Stop);
                        return;
                    }
                    Thread.Sleep(100);
                }
            }
            catch
            {
                return;
            }
        }

        public void Edit()
        {
            CmdShow.Edit_Click(null, null);
        }

        public void Init()
        {
            App.Run(() =>
            {
                UTabItem = new()
                {
                    Header = CmdData.名字,
                    ShowColor = "Blue"
                };
                CmdShow = new(OnDo, CmdData);
                CmdShow.Height = 553;
                CmdShow.Width = 855;
                UTabItem.Content = CmdShow;
            });
            App.MainWindow_.Add(UTabItem);
            Task.Run(() =>
            {
                Thread.Sleep(500);
                if (CmdData.自动启动)
                {
                    App.Run(() => OnDo(FC.Start));
                }
            });
        }
        public void OnDo(FC type, string data = "")
        {
            if (TaskRun)
                return;
            App.Run(async () =>
            {
                User = true;
                switch (type)
                {
                    case FC.Start:
                        await Start();
                        break;
                    case FC.Stop:
                        await Stop();
                        break;
                    case FC.Restart:
                        await Restart();
                        break;
                    case FC.Input:
                        if (CmdData.输入编码 != Coding.ANSI)
                        {
                            if (CmdData.输入编码 == Coding.UTF8)
                                data = App.UTF8.GetString(Encoding.Default.GetBytes(data));
                            else if (CmdData.输入编码 == Coding.Unicode)
                                data = App.Unicode.GetString(Encoding.Default.GetBytes(data));
                            else if (CmdData.输入编码 == Coding.GBK)
                                data = App.GBK.GetString(Encoding.Default.GetBytes(data));
                        }
                        StandardInput.WriteLine(data);
                        break;
                    case FC.Remove:
                        await Remove();
                        break;
                    case FC.Kill:
                        Kill();
                        break;
                    case FC.Edit:
                        UTabItem.Header = CmdData.名字;
                        break;
                }
                User = false;
            });
        }

        private async Task Restart()
        {
            await Stop();
            await Start();
        }

        private void OnClose(object sender, EventArgs e)
        {
            ThisProcess = null;
            App.Run(() =>
            {
                CmdShow.Set(false);
                ProcessRun = false;
                UTabItem.ShowColor = "Blue";
            });
        }

        private void OnOutPut(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            string data = e.Data;
            if (CmdData.输出编码 != Coding.ANSI)
            {
                if (CmdData.输出编码 == Coding.UTF8)
                    data = App.ANSI.GetString(App.UTF8.GetBytes(data));
                else if (CmdData.输出编码 == Coding.Unicode)
                    data = App.ANSI.GetString(App.Unicode.GetBytes(data));
                else if (CmdData.输出编码 == Coding.GBK)
                    data = App.ANSI.GetString(App.GBK.GetBytes(data));
            }
            CmdShow.AddLog(data);
            if (Send)
            {
                App.SendMessage(data);
            }
        }
        private void OnErrorOutPut(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            string data = e.Data;
            if (CmdData.输出编码 != Coding.ANSI)
            {
                if (CmdData.输出编码 == Coding.UTF8)
                    data = App.ANSI.GetString(App.UTF8.GetBytes(data));
                else if (CmdData.输出编码 == Coding.Unicode)
                    data = App.ANSI.GetString(App.Unicode.GetBytes(data));
                else if (CmdData.输出编码 == Coding.GBK)
                    data = App.ANSI.GetString(App.GBK.GetBytes(data));
            }
            CmdShow.AddLog(data);
            if (Send)
            {
                App.SendMessage(data);
            }
        }

        private async Task Remove()
        {
            TaskRun = true;
            await Stop();
            App.MainWindow_.Remove(UTabItem);
            App.Remove(this);
            TaskRun = false;
        }

        private async Task Start()
        {
            TaskRun = true;
            CmdShow.Set(true);
            ProcessRun = true;
            CmdShow.Clear();
            try
            {
                if (ThisProcess != null)
                {
                    ThisProcess.Dispose();
                }
                var ProcessStartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    FileName = CmdData.路径,
                    WorkingDirectory = CmdData.运行路径,
                    Arguments = CmdData.参数,
                    CreateNoWindow = true
                };
                CmdData.路径 = CmdData.路径.Trim();
                if (!File.Exists(CmdData.路径))
                {
                    CmdShow.AddLog($"文件{CmdData.路径}不存在");
                    CmdShow.Set(false);
                    ProcessRun = false;
                    TaskRun = false;
                    return;
                }
                ThisProcess = new()
                {
                    StartInfo = ProcessStartInfo
                };
                ThisProcess.OutputDataReceived += OnOutPut;
                ThisProcess.ErrorDataReceived += OnErrorOutPut;
                ThisProcess.Start();
                ThisProcess.BeginOutputReadLine();
                ThisProcess.BeginErrorReadLine();
                StandardInput = ThisProcess.StandardInput;
                UTabItem.ShowColor = "Lime";
                await Task.Run(() =>
                {
                    Thread.Sleep(20);
                    if (ThisProcess?.HasExited == false)
                    {
                        var temp = CmdData.命令.Split('\n');
                        foreach (var item in temp)
                        {
                            StandardInput.WriteLine(item);
                        }
                    }
                });
                Thread = new Thread(Check);
                Thread.Start();
            }
            catch (Exception e)
            {
                CmdShow.AddLog(e.ToString());
                ThisProcess.Dispose();
                CmdShow.Set(false);
                ProcessRun = false;
            }
            App.MainWindow_.RunCount++;
            if (CmdData.启动反馈)
                App.SendMessage($"实例[{CmdData.名字}]已启动");
            TaskRun = false;
        }

        private async Task Stop()
        {
            TaskRun = true;
            if (ThisProcess != null && ThisProcess.HasExited == false)
            {
                App.ShowA("关闭", $"正在关闭[{CmdData.名字}]");
                if (!string.IsNullOrWhiteSpace(CmdData.关闭指令))
                {
                    StandardInput.WriteLine(CmdData.关闭指令);
                }
                ThisProcess.CloseMainWindow();
                if (ThisProcess.HasExited == false)
                {
                    var task1 = ThisProcess.WaitForExitAsync();
                    var task2 = Task.Delay(10000);
                    var res = await Task.WhenAny(task1, task2);
                    if (res == task2)
                    {
                        App.ShowA("关闭", $"实例[{CmdData.名字}]关闭时间过长，强制结束");
                        ThisProcess.Kill(true);
                    }
                }
                ThisProcess.Dispose();
            }
            OnClose(null, null);
            App.MainWindow_.RunCount--;
            if (CmdData.关闭反馈)
                App.SendMessage($"实例[{CmdData.名字}]已关闭");
            if (!User && CmdData.自动重启)
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                    OnDo(FC.Start);
                });
            }
            TaskRun = false;
        }

        private void Kill()
        {
            TaskRun = true;
            if (ThisProcess?.HasExited == false)
            {
                ThisProcess.Kill(true);
                ThisProcess.Dispose();
            }
            OnClose(null, null);
            App.MainWindow_.RunCount--;
            if (CmdData.关闭反馈)
                App.SendMessage($"实例[{CmdData.名字}]已强制结束");
            TaskRun = false;
        }
    }
}
