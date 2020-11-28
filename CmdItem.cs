using CmdControl.Custom;
using CmdControl.Objs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdControl
{
    public class CmdItem
    {
        private CmdData CmdData;
        private UTabItem UTabItem;
        private CmdShow CmdShow;
        private Process Process;
        private ProcessStartInfo ProcessInfo;
        private StreamWriter StandardInput;

        public CmdItem(CmdData CmdData)
        {
            this.CmdData = CmdData;
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
        }
        private void OnDo(FC type, string data)
        {
            switch (type)
            {
                case FC.Start:

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
            
        }

        private void OnOutPut(object sender, DataReceivedEventArgs e)
        { 
            
        }
        private void OnErrorOutPut(object sender, DataReceivedEventArgs e)
        {

        }

        public void Remove()
        {
            Process?.Dispose();
            App.MainWindow_.Remove(UTabItem);
            App.Remove(CmdData.名字);
        }

        public void Start()
        {
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
            if (!string.IsNullOrWhiteSpace(CmdData.关闭指令))
            { 
                
            }
            Process.Close();
            await Process.WaitForExitAsync();
        }
    }
}
