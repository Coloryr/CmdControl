using CmdControl.Custom;
using System.Windows;

namespace CmdControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool state;
        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow_ = this;
            DataContext = App.Config;
        }

        public void Remove(UTabItem show)
        {
            Dispatcher.Invoke(() => TabList.Items.Remove(show));
        }

        public void Add(UTabItem show)
        {
            Dispatcher.Invoke(() => TabList.Items.Add(show));
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

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Robot_Click(object sender, RoutedEventArgs e)
        {
            if (state)
            {
                App.Robot.Stop();
            }
            else
            {
                App.Robot.Start();
            }
        }
    }
}
