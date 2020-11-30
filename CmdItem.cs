using CmdControl.Custom;
using CmdControl.Objs;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text;
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
        private bool Run;

        public CmdItem(CmdData CmdData)
        {
            this.CmdData = CmdData;
            名字 = CmdData.名字;
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
        private void OnDo(FC type, string data)
        {
            if (Run)
                return;
            Run = true;
            switch (type)
            {
                case FC.Start:
                    Start();
                    break;
                case FC.Stop:
                    Stop();
                    break;
                case FC.Restart:
                    Stop();
                    Start();
                    break;
                case FC.Input:
                    StandardInput.WriteLine(data);
                    break;
                case FC.Remove:
                    Remove();
                    break;
                case FC.Kill:
                    Kill();
                    break;
                case FC.Edit:
                    UTabItem.Header = CmdData.名字;
                    break;
            }
            Run = false;
        }

        private void OnClose(object sender, EventArgs e)
        {
            CmdShow.Set(false);
            UTabItem.ShowColor = "Blue";
            CmdShow.AddLog("\n进程关闭");
        }

        private void OnOutPut(object sender, DataReceivedEventArgs e)
        {
            CmdShow.AddLog(e.Data);
        }
        private void OnErrorOutPut(object sender, DataReceivedEventArgs e)
        {
            CmdShow.Set(false);
            UTabItem.ShowColor = "Red";
            App.MainWindow_.CrashCount += 1;
            CmdShow.AddLog("进程以外关闭:" + e.Data);
        }

        public void Remove()
        {
            Stop();
            App.MainWindow_.Remove(UTabItem);
            App.Remove(this);
        }

        public async void Start()
        {
            CmdShow.Set(true);

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
                Process = new Process();
                Process.StartInfo = ProcessStartInfo;
                Process.Exited += OnClose;
                Process.OutputDataReceived += OnOutPut;
                Process.ErrorDataReceived += OnErrorOutPut;
                Process.Start();
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
                CmdShow.AddLog("\n进程启动");
            }
            catch(Exception e)
            {
                CmdShow.AddLog(e.ToString());
                Process.Dispose();
                CmdShow.Set(false);
            }
        }

        public void Stop()
        {
            if (Process?.HasExited == false)
            {
                if (!string.IsNullOrWhiteSpace(CmdData.关闭指令))
                {
                    StandardInput.WriteLine(CmdData.关闭指令);
                }
                Process.Close();
                Process.Dispose();
            }
        }

        public void Kill()
        {
            if (Process?.HasExited == false)
            {
                Process.Kill();
                Process.Dispose();
            }
        }
    }
}
