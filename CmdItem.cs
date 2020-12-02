using CmdControl.Custom;
using CmdControl.Objs;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CmdControl
{
    public class CmdItem
    {
        public string 名字 { get; set; }

        private CmdData CmdData;
        private UTabItem UTabItem;
        private CmdShow CmdShow;
        private Process Process;
        private StreamWriter StandardInput;
        private Thread Thread;
        private bool Run;

        public CmdItem(CmdData CmdData)
        {
            this.CmdData = CmdData;
            名字 = CmdData.名字;
        }

        private void Check()
        {
            try
            {
                while (Process?.HasExited == false)
                {
                    if (Process?.HasExited == true)
                    {
                        Stop();
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
                    Start();
                }
            });
        }
        private async void OnDo(FC type, string data)
        {
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

        public async Task Remove()
        {
            if (Run)
                return;
            Run = true;
            await Stop();
            App.MainWindow_.Remove(UTabItem);
            App.Remove(this);
            Run = false;
        }

        public async Task Start()
        {
            if (Run)
                return;
            Run = true;
            CmdShow.Set(true);
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
            }
            Run = false;
        }

        public async Task Stop()
        {
            if (Run)
                return;
            Run = true;
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
            Run = false;
        }

        public bool IsRun()
        {
            try
            {
                return !Process.HasExited;
            }
            catch
            {
                return false;
            }
        }

        public void Kill()
        {
            if (Process?.HasExited == false)
            {
                Process.Kill();
                Process.Dispose();
            }
            OnClose(null, null);
            Run = false;
        }
    }
}
