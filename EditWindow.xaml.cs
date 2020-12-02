using CmdControl.Objs;
using System.IO;
using System.Windows;

namespace CmdControl
{
    /// <summary>
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        public CmdData CmdData { get; set; }
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

        private void Choise_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Title = "选择文件";
            openFileDialog1.Filter = "文件|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!File.Exists(openFileDialog1.FileName))
                {
                    App.ShowB("文件选择", "文件不存在");
                    return;
                }
                CmdData.路径 = openFileDialog1.FileName;
            }
        }

        private void Dir_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CmdData.运行路径 = openFileDialog.SelectedPath;
            }
        }
    }
}
