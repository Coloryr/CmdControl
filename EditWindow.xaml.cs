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

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            CmdData.关闭反馈 = false;
            CmdData.关闭指令 = "";
            CmdData.参数 = "";
            CmdData.名字 = "";
            CmdData.启动反馈 = false;
            CmdData.命令 = "";
            CmdData.崩溃重启 = false;
            CmdData.自动启动 = false;
            CmdData.路径 = "";
            CmdData.远程控制 = false;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
