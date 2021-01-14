using CmdControl.Custom;
using CmdControl.Objs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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

        private static CmdItem ShowItem;
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
            switch (packid)
            {
                case 49:
                    var pack = JsonConvert.DeserializeObject<GroupMessageEventPack>(data);
                    string temp = pack.message[^1];
                    if (temp.StartsWith(Config.机器人指令.启动指令))
                    {
                        if (!Config.机器人设置.管理员账户.Contains(pack.fid))
                        {
                            return;
                        }
                        string name = temp.Remove(0, Config.机器人指令.启动指令.Length);
                        foreach (var item in CmdList)
                        {
                            if (item.CmdData.名字 == name && !item.ProcessRun)
                            {
                                if (item.CmdData.远程控制)
                                    item.OnDo(FC.Start);
                                else
                                    SendMessage("该实例不允许远程控制");
                                continue;
                            }
                        }
                    }
                    else if (temp.StartsWith(Config.机器人指令.关闭指令))
                    {
                        if (!Config.机器人设置.管理员账户.Contains(pack.fid))
                        {
                            return;
                        }
                        string name = temp.Remove(0, Config.机器人指令.关闭指令.Length);
                        foreach (var item in CmdList)
                        {
                            if (item.CmdData.名字 == name && item.ProcessRun)
                            {
                                if (item.CmdData.远程控制)
                                    item.OnDo(FC.Stop);
                                else
                                    SendMessage("该实例不允许远程控制");
                                continue;
                            }
                        }
                    }
                    else if (temp.StartsWith(Config.机器人指令.列表指令))
                    {
                        string send = "实例列表：";
                        foreach (var item in CmdList)
                        {
                            string state = item.ProcessRun ? "运行" : "关闭";
                            send += $"\n{item.名字}，当前状态：{state}";
                        }
                        SendMessage(send);
                        return;
                    }
                    else if (temp.StartsWith(Config.机器人指令.控制指令))
                    {
                        if (!Config.机器人设置.管理员账户.Contains(pack.fid))
                        {
                            return;
                        }
                        string name = temp.Remove(0, Config.机器人指令.控制指令.Length);
                        foreach (var item in CmdList)
                        {
                            if (item.CmdData.名字 == name)
                            {
                                ShowItem = item;
                                item.StartSend();
                                SendMessage($"开始监控：{item.名字}");
                                return;
                            }
                        }
                    }
                    else if (temp.StartsWith(Config.机器人指令.信息指令))
                    {
                        string name = temp.Remove(0, Config.机器人指令.启动指令.Length);
                        foreach (var item in CmdList)
                        {
                            if (item.CmdData.名字 == name)
                            {
                                SendMessage(item.ToString());
                                continue;
                            }
                        }
                    }
                    else if (temp.StartsWith(Config.机器人指令.退出指令))
                    {
                        if (ShowItem != null)
                        {
                            ShowItem.EndSend();
                            SendMessage($"退出监控：{ShowItem.名字}");
                            ShowItem = null;
                        }
                    }
                    else if (ShowItem != null)
                    {
                        if (!Config.机器人设置.管理员账户.Contains(pack.fid))
                        {
                            return;
                        }
                        if (temp.StartsWith("/"))
                        {
                            ShowItem.OnDo(FC.Input, temp.Remove(0, 1));
                        }
                    }
                    break;
                case 50:
                    break;
                case 51:
                    break;
            }
        }
        private static void LogEvent(LogType type, string data)
        {

        }
        private static void StateEvent(StateType type)
        {
            switch (type)
            {
                case StateType.Connect:
                    MainWindow_.BotSet(true);
                    break;
                case StateType.Disconnect:

                    break;
            }
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            MainWindow_?.Activate();
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //WindowsIdentity identity = WindowsIdentity.GetCurrent();
            //WindowsPrincipal principal = new WindowsPrincipal(identity);
            //if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            //{
            //    ProcessStartInfo startInfo = new();
            //    startInfo.UseShellExecute = true;
            //    startInfo.WorkingDirectory = Environment.CurrentDirectory;
            //    startInfo.FileName = Assembly.GetExecutingAssembly().Location;
            //    startInfo.Verb = "runas";
            //    try
            //    {
            //        Process.Start(startInfo);
            //    }
            //    catch
            //    {
            //        return;
            //    }
            //    Current.Shutdown();
            //}

            ThisApp = this;
            Local = AppDomain.CurrentDomain.BaseDirectory;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            notifyIcon = new();
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipText = "CmdControl";
            notifyIcon.Click += NotifyIcon_Click;

            Config = ConfigUtils.Read(Local + "/Config.json", new ConfigObj
            {
                实例列表 = new(),
                机器人指令 = new()
                {
                    关闭指令 = "关闭：",
                    列表指令 = "列表",
                    启动指令 = "启动：",
                    信息指令 = "信息：",
                    控制指令 = "控制：",
                    退出指令 = "退出"
                },
                机器人设置 = new()
                {
                    机器人号 = 0,
                    运行群号 = 0,
                    管理员账户 = new()
                },
                机器人连接 = new()
                {
                    地址 = "127.0.0.1",
                    端口 = 23333,
                    自动连接 = false
                }
            });

            if (Config.机器人设置.管理员账户 == null)
            {
                Config.机器人设置.管理员账户 = new();
                Save();
            }

            Robot = new();
            Robot.IsFirst = false;

            Task.Run(() =>
            {
                Thread.Sleep(200);
                if (Config.机器人连接.自动连接)
                {
                    RobotStart();
                }
                foreach (var item in Config.实例列表)
                {
                    var temp = new CmdItem(item);
                    ThisApp.Dispatcher.Invoke(() => CmdList.Add(temp));
                    temp.Init();
                }
                ShowA("启动", "所有实例已加载");
            });
        }

        public static void RobotStart()
        {
            if (Robot.IsRun)
            {
                Robot.Stop();
            }
            if (Config.机器人设置.机器人号 == 0 || Config.机器人设置.运行群号 == 0)
            {
                ShowB("机器人连接", "参数为空，机器人连接失败");
                return;
            }
            RobotConfig = new()
            {
                Name = "CmdControl",
                Check = true,
                Groups = new() { Config.机器人设置.运行群号 },
                RunQQ = Config.机器人设置.机器人号,
                Pack = new() { 49, 50, 51 },
                Time = 10000,
                IP = Config.机器人连接.地址,
                Port = Config.机器人连接.端口,
                CallAction = Call,
                LogAction = LogEvent,
                StateAction = StateEvent
            };
            Robot.Set(RobotConfig);
            Robot.Start();
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                SendMessage("[CmdControl]机器人已连接");
            });
        }

        public static void RobotStop()
        {
            if (Robot.IsRun)
            {
                Robot.Stop();
            }
        }

        public static void SendMessage(string data)
        {
            if (Robot.IsRun)
            {
                Robot.SendGroupMessage(Config.机器人设置.机器人号,
                    Config.机器人设置.运行群号, new List<string>() { data });
            }
        }

        public static void Remove(CmdItem data)
        {
            if (CmdList.Contains(data))
            {
                CmdList.Remove(data);
                Config.实例列表.Remove(data.CmdData);
                Save();
            }
        }

        public static void New(CmdData data)
        {
            Config.实例列表.Add(data);
            Save();
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
        public static void Save()
        {
            ConfigUtils.Write(Config, Local + "/Config.json");
        }

        public static void OnClose(CancelEventArgs e)
        {
            foreach (var item in CmdList)
            {
                if (item.ProcessRun)
                {
                    var win = new MessageShow()
                    {
                        Title = "关闭",
                        Show_ = "还有进程在运行，你确定要杀死吗"
                    };
                    var res = win.ShowThis();
                    if (res == 1)
                    {
                        foreach (var item1 in CmdList)
                        {
                            if (item1.ProcessRun)
                            {
                                item1.OnDo(FC.Kill);
                            }
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
            if (Robot.IsRun)
            {
                Robot.Stop();
            }
        }
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                MessageBox.Show("捕获未处理异常:" + e.Exception.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生错误" + ex.ToString());
            }

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder sbEx = new StringBuilder();
            if (e.IsTerminating)
            {
                sbEx.Append("发生错误，将关闭\n");
            }
            sbEx.Append("捕获未处理异常：");
            if (e.ExceptionObject is Exception)
            {
                sbEx.Append(((Exception)e.ExceptionObject).ToString());
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }
            MessageBox.Show(sbEx.ToString());
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show("捕获线程内未处理异常：" + e.Exception.ToString());
            e.SetObserved();
        }
    }
}
