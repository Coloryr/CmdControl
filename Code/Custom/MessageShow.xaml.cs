using System.Windows;

namespace CmdControl.Custom
{
    /// <summary>
    /// MessageShow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageShow : Window
    {
        public string Show_;
        private int Res;

        public MessageShow()
        {
            InitializeComponent();
        }

        public int ShowThis()
        {
            ShowData.Text = Show_;
            ShowDialog();
            return Res;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Res = 1;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Res = 0;
            Close();
        }
    }
}
