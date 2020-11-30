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
        private ProcessStartInfo ProcessInfo;
        private StreamWriter StandardInput;

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
            switch (type)
            {
                case FC.Start:
                    Start();
                    break;
                case FC.Stop:

                    break;
                case FC.Restart:

                    break;
                case FC.Input:
                    StandardInput.WriteLine(data);
                    break;
            }
        }

        private void OnClose(object sender, EventArgs e)
        {
            CmdShow.Set(false);
            CmdShow.AddLog("进程关闭:" + e.ToString());
        }

        private void OnOutPut(object sender, DataReceivedEventArgs e)
        {
            CmdShow.AddLog(e.Data);
        }
        private void OnErrorOutPut(object sender, DataReceivedEventArgs e)
        {
            CmdShow.Set(false);
            CmdShow.AddLog("进程以外关闭:" + e.Data);
        }

        public void Remove()
        {
            Process?.Dispose();
            App.MainWindow_.Remove(UTabItem);
            App.Remove(this);
        }

        public void Start()
        {
            CmdShow.Set(true);
            ProcessInfo = new()
            {

            };
            Process = Process.Start(ProcessInfo);
            StandardInput = Process.StandardInput;
            Process.Exited += OnClose;
            Process.OutputDataReceived += OnOutPut;
            Process.ErrorDataReceived += OnErrorOutPut;

        }

        public async void Stop()
        {
            if (Process?.HasExited == false)
            {
                if (!string.IsNullOrWhiteSpace(CmdData.关闭指令))
                {
                    StandardInput.WriteLine(CmdData.关闭指令);
                }
                Process.Close();
                await Process.WaitForExitAsync();
                Process.Dispose();
            }
        }

        public async void Kill()
        {
            if (Process?.HasExited == false)
            {
                Process.Kill();
                await Process.WaitForExitAsync();
                Process.Dispose();
            }
        }
    }
}
