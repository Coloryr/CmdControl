using CmdControl.Objs;
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
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        private CmdData CmdData { get; set; }
        public EditWindow(CmdData CmdData = null)
        {
            if (CmdData == null)
                CmdData = new();
            this.CmdData = CmdData;
            InitializeComponent();
            DataContext = this;
        }
        public CmdData Edit()
        {
            ShowDialog();
            return CmdData;
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
