using System.Windows;

namespace CmdControl
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        public string old { get; set; }
        public InputWindow(string title, string old = "")
        {
            this.old = old;
            InitializeComponent();
            Title = title;
            DataContext = this;
        }

        public string Set()
        {
            ShowDialog();
            return old;
        }
    }
}
