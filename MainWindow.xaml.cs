using CmdControl.Custom;
using CmdControl.Objs;
using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CmdControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool state;
        private int CrashCount_ = 0;
        private int RunCount_ = 0;
        public int CrashCount
        {
            set
            {
                CrashCount_ = value;
                Dispatcher.Invoke(() => CrashCountShow.Content = CrashCount_);
            }
            get
            {
                return CrashCount_;
            }
        }
        public int RunCount
        {
            set
            {
                RunCount_ = value;
                Dispatcher.Invoke(() => RunCountShow.Content = RunCount_);
            }
            get
            {
                return RunCount_;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow_ = this;
            DataContext = App.Config;

            BitmapSource m = (BitmapSource)Icon;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight,
                PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
            new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppPArgb);

            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);

            IntPtr iconHandle = bmp.GetHicon();
            System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(iconHandle);

            App.notifyIcon.Icon = icon;
            CmdsList.ItemsSource = App.CmdList;
        }

        public void Remove(UTabItem show)
        {
            Dispatcher.Invoke(() =>
            {
                AllCountShow.Content = App.CmdList.Count;
                TabList.Items.Remove(show);
            });
        }

        public void Add(UTabItem show)
        {
            Dispatcher.Invoke(() =>
            {
                AllCountShow.Content = App.CmdList.Count;
                TabList.Items.Add(show);
            });
        }

        public void BotSet(bool state)
        {
            Dispatcher.Invoke(() =>
            {
                this.state = state;
                if (state)
                {
                    RebotPort.IsEnabled =
                    RebotIP.IsEnabled = false;
                    RebotButton.Content = "断开";
                }
                else
                {
                    RebotPort.IsEnabled =
                    RebotIP.IsEnabled = true;
                    RebotButton.Content = "连接";
                }
            });
        }

        private void AddCmd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RunLocal.Text))
            {
                App.ShowB("快速添加错误", "运行路径为空");
                return;
            }
            var item = new CmdData
            {
                名字 = "新建实例",
                路径 = RunLocal.Text,
                命令 = RunCommand.Text,
                参数 = RunArg.Text
            };
            App.New(item);
            var temp = new CmdItem(item);
            App.Add(temp);
            Clear_Click(null, null);
            App.ShowA("快速添加", "已添加实例");
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            RunLocal.Text = "";
            RunCommand.Text = "";
            RunArg.Text = "";
        }

        private void Robot_Click(object sender, RoutedEventArgs e)
        {
            if (state)
            {
                App.RobotStop();
            }
            else
            {
                App.RobotStart();
            }
        }

        private void NewCmd_Click(object sender, RoutedEventArgs e)
        {
            var item = new CmdData
            {
                名字 = "新建实例",
                路径 = RunLocal.Text,
                命令 = RunCommand.Text,
                参数 = RunArg.Text
            };
            item = new EditWindow(item).Edit();
            App.New(item);
            var temp = new CmdItem(item);
            App.Add(temp);
            Clear_Click(null, null);
            App.ShowA("添加实例", "已添加实例");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            App.OnClose(e);
        }

        private void CmdsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmdsList.SelectedItem != null)
            {
                CmdItem item = (CmdItem)CmdsList.SelectedItem;
                if (item.ProcessRun)
                {
                    RunButton.IsEnabled = false;
                    CloseButton.IsEnabled = true;
                    RestartButton.IsEnabled = true;
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                }
                else
                {
                    RunButton.IsEnabled = true;
                    CloseButton.IsEnabled = false;
                    RestartButton.IsEnabled = false;
                    EditButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
                }
            }
            else
            {
                RunButton.IsEnabled = false;
                CloseButton.IsEnabled = false;
                RestartButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (CmdsList.SelectedItem == null)
            {
                return;
            }
            CmdItem item = (CmdItem)CmdsList.SelectedItem;
            item.OnDo(FC.Start);
            CmdsList.SelectedItem = null;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (CmdsList.SelectedItem == null)
            {
                return;
            }
            CmdItem item = (CmdItem)CmdsList.SelectedItem;
            item.OnDo(FC.Stop);
            CmdsList.SelectedItem = null;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (CmdsList.SelectedItem == null)
            {
                return;
            }
            CmdItem item = (CmdItem)CmdsList.SelectedItem;
            item.OnDo(FC.Restart);
            CmdsList.SelectedItem = null;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (CmdsList.SelectedItem == null)
            {
                return;
            }
            CmdItem item = (CmdItem)CmdsList.SelectedItem;
            item.Edit();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CmdsList.SelectedItem == null)
            {
                return;
            }
            CmdItem item = (CmdItem)CmdsList.SelectedItem;
            item.OnDo(FC.Remove);
            CmdsList.SelectedItem = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Save();
        }
    }
}
