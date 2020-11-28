using CmdControl.Objs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CmdControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ConfigObj Config { get; set; }
        public static MainWindow MainWindow_;
        public static Dictionary<string, CmdItem> CmdList = new();
        public static App ThisApp;
        public static Robot Robot;
        public static RobotConfig RobotConfig;
        public static System.Windows.Forms.NotifyIcon notifyIcon;
        public static void ShowA(string title, string data)
        {
            notifyIcon.ShowBalloonTip(300, title, data, System.Windows.Forms.ToolTipIcon.Info);
        }
        public static void ShowB(string title, string data)
        {
            notifyIcon.ShowBalloonTip(300, title, data, System.Windows.Forms.ToolTipIcon.Error);
        }

        private void Call(byte packid, string data)
        { 
            
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThisApp = this;
            Config = ConfigUtils.Read("/Config.json", new ConfigObj
            {
                实例列表 = new(),
                //机器人指令 = new("列表", "启动", "关闭"),
                //机器人设置 = new(0, 0),
                //机器人连接 = new("127.0.0.1", 23333, false)
            });
            RobotConfig = new()
            {
                name = "CmdControl",
                check = true,
                groups = new() { Config.机器人设置.运行群号 },
                runqq = Config.机器人设置.机器人号,
                pack = new() { 49, 50, 51 },
                time = 10000,
                ip = Config.机器人连接.地址,
                port = Config.机器人连接.端口,
                action = Call
            };
            Robot = new(RobotConfig);
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                if (Config.机器人连接.自动连接)
                {
                    Robot.Start();
                }
                var data = new CmdData 
                {
                     名字 = "测试"
                };
                var item = new CmdItem(data);
                Add(data, item);
                item.Init();
                Load();
            });
        }

        public static void Remove(string data)
        {
            if (CmdList.ContainsKey(data))
            {
                CmdList.Remove(data);
            }
        }

        public static void Load()
        {
            foreach (var item in Config.实例列表)
            {
                CmdList.Add(item.Key, new(item.Value));
            }
        }

        public static void Add(CmdData key, CmdItem item)
        {
            if (CmdList.ContainsKey(key.名字))
                return;
            else
                CmdList.Add(key.名字, item);
        }

        public static void Run(Action action)
        {
            ThisApp.Dispatcher.Invoke(action);
        }
    }
}
