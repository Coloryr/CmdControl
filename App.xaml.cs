using CmdControl.Objs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
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
        public static string Local;
        public static MainWindow MainWindow_;
        public static ObservableCollection<CmdItem> CmdList = new();
        public static App ThisApp;
        public static System.Windows.Forms.NotifyIcon notifyIcon;

        private static Robot Robot;
        private static RobotConfig RobotConfig;
        public static void ShowA(string title, string data)
        {
            notifyIcon.ShowBalloonTip(300, title, data, System.Windows.Forms.ToolTipIcon.Info);
        }
        public static void ShowB(string title, string data)
        {
            notifyIcon.ShowBalloonTip(300, title, data, System.Windows.Forms.ToolTipIcon.Error);
        }

        private static void Call(byte packid, string data)
        {

        }
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            MainWindow_?.Activate();
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                ProcessStartInfo startInfo = new();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Assembly.GetExecutingAssembly().Location;
                startInfo.Verb = "runas";
                try
                {
                    Process.Start(startInfo);
                }
                catch
                {
                    return;
                }
                Current.Shutdown();
            }

            ThisApp = this;
            Local = AppDomain.CurrentDomain.BaseDirectory;

            notifyIcon = new();
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipText = "CmdControl";
            notifyIcon.Click += NotifyIcon_Click;

            Config = ConfigUtils.Read(Local + "/Config.json", new ConfigObj
            {
                实例列表 = new(),
                机器人指令 = new()
                {
                    关闭指令 = "关闭",
                    列表指令 = "列表",
                    启动指令 = "启动"
                },
                机器人设置 = new()
                {
                    机器人号 = 0,
                    运行群号 = 0
                },
                机器人连接 = new()
                {
                    地址 = "127.0.0.1",
                    端口 = 23333,
                    自动连接 = false
                }
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
            Robot = new();

            Task.Run(() =>
            {
                Thread.Sleep(2000);
                if (Config.机器人连接.自动连接)
                {
                    RobotStart();
                }
                Load();
            });
        }

        public static void RobotStart()
        {
            if (Config.机器人设置.机器人号 == 0 || Config.机器人设置.运行群号 == 0)
            {
                ShowB("机器人连接", "参数为空，机器人连接失败");
                return;
            }
            Robot.Set(RobotConfig);
            Robot.Start();
        }

        public static void RobotStop()
        {
            if (Robot.IsRun)
            {
                Robot.Stop();
            }
        }

        public static void Remove(CmdItem data)
        {
            if (CmdList.Contains(data))
            {
                CmdList.Remove(data);
            }
        }

        public static void Load()
        {
            foreach (var item in Config.实例列表)
            {
                var temp = new CmdItem(item);
                ThisApp.Dispatcher.Invoke(() => CmdList.Add(temp));
                temp.Init();
            }
            ShowA("启动", "所有实例已加载");
        }

        public static void New(CmdData data)
        {
            Config.实例列表.Add(data);

        }

        public static void Add(CmdItem item)
        {
            if (CmdList.Contains(item))
                return;
            else
            {
                CmdList.Add(item);
                item.Init();
            }
        }

        public static void Run(Action action)
        {
            ThisApp.Dispatcher.Invoke(action);
        }

        public static void SendMessage(string data)
        {
            if (Robot.IsConnect)
            {
                Robot.SendGroupMessage(0, Config.机器人设置.运行群号, new() { data });
            }
        }
        public static void Save()
        {
            ConfigUtils.Write(Config, Local + "/Config.json");
        }
    }
}
