using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
