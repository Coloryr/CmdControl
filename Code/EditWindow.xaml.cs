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
        private bool IsRun;
        public EditWindow(CmdData CmdData = null)
        {
            if (CmdData == null)
                CmdData = new();
            this.CmdData = CmdData;
            InitializeComponent();
            DataContext = this;
            switch (CmdData.输入编码)
            {
                case Coding.ANSI:
                    A3.IsChecked = true;
                    break;
                case Coding.UTF8:
                    A1.IsChecked = true;
                    break;
                case Coding.Unicode:
                    A2.IsChecked = true;
                    break;
                case Coding.GBK:
                    A4.IsChecked = true;
                    break;
            }
            switch (CmdData.输出编码)
            {
                case Coding.ANSI:
                    B3.IsChecked = true;
                    break;
                case Coding.UTF8:
                    B1.IsChecked = true;
                    break;
                case Coding.Unicode:
                    B2.IsChecked = true;
                    break;
                case Coding.GBK:
                    B4.IsChecked = true;
                    break;
            }
            IsRun = true;
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
            CmdData.自动重启 = false;
            CmdData.自动启动 = false;
            CmdData.路径 = "";
            CmdData.远程控制 = false;
            CmdData.输入编码 = Coding.ANSI;
            CmdData.输出编码 = Coding.ANSI;
            A3.IsChecked = true;
            B3.IsChecked = true;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Choise_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog1 = new System.Windows.Forms.OpenFileDialog
            {
                Title = "选择文件",
                Filter = "文件|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };
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

        private void A1_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRun)
                CmdData.输入编码 = Coding.UTF8;
        }

        private void A2_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRun)
                CmdData.输入编码 = Coding.Unicode;
        }

        private void A3_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRun)
                CmdData.输入编码 = Coding.ANSI;
        }

        private void B1_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRun)
                CmdData.输出编码 = Coding.UTF8;
        }

        private void B2_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRun)
                CmdData.输出编码 = Coding.Unicode;
        }

        private void B3_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRun)
                CmdData.输出编码 = Coding.ANSI;
        }

        private void A4_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRun)
                CmdData.输入编码 = Coding.GBK;
        }

        private void B4_Checked(object sender, RoutedEventArgs e)
        {
            if (IsRun)
                CmdData.输出编码 = Coding.GBK;
        }
    }
}
