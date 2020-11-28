using CmdControl.Objs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CmdControl.Custom
{
    /// <summary>
    /// CmdShow.xaml 的交互逻辑
    /// </summary>
    public partial class CmdShow : UserControl
    {
        private delegate void Input(FC type, string data = null);
        private Input CallInput;
        private CmdData CmdData;
        private bool IsSave;
        public CmdShow(Action<FC, string> action, CmdData CmdData)
        {
            this.CmdData = CmdData;
            InitializeComponent();
            DataContext = CmdData;
            CallInput = new Input(action);
        }

        public void AddLog(string data)
        {
            Log.Text += data + "\n";
            if (Log.Text.Length >= 100000)
                Log.Text = "";
        }
        public void Set(bool run)
        {
            if (run)
            {
                StartButton.IsEnabled = false;
                CloseButton.IsEnabled = true;
                RestartButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
            else
            {
                StartButton.IsEnabled = true;
                CloseButton.IsEnabled = false;
                RestartButton.IsEnabled = true;
                EditButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
        }

        private void Command_Click(object sender, RoutedEventArgs e)
        {
            CallInput.Invoke(FC.Input, Command.Text);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Log.Text = "";
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (IsSave)
                return;
            var SaveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "日志文件|*.log"
            };
            if (SaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                IsSave = true;
                var file = SaveFileDialog.FileName;
                await File.WriteAllTextAsync(file, Log.Text);
                Log.Text = "";
                IsSave = false;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            CallInput.Invoke(FC.Start);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
