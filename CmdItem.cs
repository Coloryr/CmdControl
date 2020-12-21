using CmdControl.Custom;
using CmdControl.Objs;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CmdControl
{
    public class CmdItem : IDisposable
    {
        public string 名字 { get; set; }
        public bool ProcessRun { get; private set; }
        public CmdData CmdData { get; private set; }

        private CmdShow CmdShow;
        private UTabItem UTabItem;
        private Process Process;
        private StreamWriter StandardInput;
        private Thread Thread;
        private bool User;
        private bool TaskRun;

        public CmdItem(CmdData CmdData)
        {
            this.CmdData = CmdData;
            名字 = CmdData.名字;
        }

        public override string ToString()
        {
            return $"实例名字:{CmdData.名字}\n运行应用:{CmdData.路径}\n运行路径:{CmdData.运行路径}\n运行参数:{CmdData.参数}\n运行命令:{CmdData.命令}\n关闭指令:{CmdData.关闭指令}\n自动启动:{CmdData.自动启动}\n远程控制:{CmdData.远程控制}\n自动重启:{CmdData.自动重启}\n启动反馈:{CmdData.启动反馈}\n关闭反馈:{CmdData.关闭反馈}";
        }

        private void Check()
        {
            try
            {
                while (Process?.HasExited == false)
                {
                    if (Process?.HasExited == true)
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
            App.Run(() =>
            {
                CmdShow.Set(false);
                ProcessRun = false;
                UTabItem.ShowColor = "Blue";
            });
        }

        private void OnOutPut(object sender, DataReceivedEventArgs e)
        {
            CmdShow.AddLog(e.Data);
        }
        private void OnErrorOutPut(object sender, DataReceivedEventArgs e)
        {
            CmdShow.AddLog(e.Data);
        }

        private async Task Remove()
        {
            if (TaskRun)
                return;
            TaskRun = true;
            await Stop();
            App.MainWindow_.Remove(UTabItem);
            App.Remove(this);
            TaskRun = false;
        }

        private async Task Start()
        {
            if (TaskRun)
                return;
            TaskRun = true;
            CmdShow.Set(true);
            ProcessRun = true;
            CmdShow.Clear();
            try
            {
                if (Process != null)
                {
                    Process.Dispose();
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
                Process = new()
                {
                    StartInfo = ProcessStartInfo
                };
                Process.OutputDataReceived += OnOutPut;
                Process.ErrorDataReceived += OnErrorOutPut;
                Process.Start();
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();
                StandardInput = Process.StandardInput;
                UTabItem.ShowColor = "Lime";
                await Task.Run(() =>
                {
                    Thread.Sleep(20);
                    if (Process?.HasExited == false)
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
                Process.Dispose();
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
            if (TaskRun)
                return;
            TaskRun = true;
            if (Process?.HasExited == false)
            {
                if (!string.IsNullOrWhiteSpace(CmdData.关闭指令))
                {
                    StandardInput.WriteLine(CmdData.关闭指令);
                }
                Process.CloseMainWindow();
                if (Process?.HasExited == false)
                {
                    await Process.WaitForExitAsync();
                }
                Process.Dispose();
            }
            OnClose(null, null);
            App.MainWindow_.RunCount--;
            if (CmdData.关闭反馈)
                App.SendMessage($"实例[{CmdData.名字}]已关闭");
            if (!User && CmdData.自动启动)
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
            if (Process?.HasExited == false)
            {
                Process.Kill();
                Process.Dispose();
            }
            OnClose(null, null);
            App.MainWindow_.RunCount--;
            if (CmdData.关闭反馈)
                App.SendMessage($"实例[{CmdData.名字}]已强制结束");
            TaskRun = false;
        }

        public void Dispose()
        {
            Kill();
            StandardInput.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
