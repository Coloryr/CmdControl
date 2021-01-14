using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CmdControl.Objs
{
    public enum Coding
    { 
        UTF8, Unicode, ANSI
    }
    public enum FC
    {
        Start, Stop, Restart, Input,
        Kill, Remove, Edit
    }
    public record Robot
    {
        public string 地址 { get; set; }
        public int 端口 { get; set; }
        public bool 自动连接 { get; set; }
    }
    public record RobotConfig
    {
        public long 运行群号 { get; set; }
        public long 机器人号 { get; set; }
        public ObservableCollection<long> 管理员账户 { get; set; }
    }
    class CmdData_
    {
        public string 名字;
        public string 路径;
        public string 命令;
        public string 参数;
        public string 运行路径;
        public string 关闭指令;
        public bool 自动启动;
        public bool 远程控制 = true;
        public bool 自动重启;
        public bool 启动反馈 = true;
        public bool 关闭反馈 = true;
        public Coding 输入编码 = Coding.ANSI;
        public Coding 输出编码 = Coding.ANSI;
    }
    public class CmdData : INotifyPropertyChanged
    {
        private CmdData_ CmdData_ = new();
        public Coding 输入编码
        {
            get
            {
                return CmdData_.输入编码;
            }
            set
            {
                CmdData_.输入编码 = value;
            }
        }
        public Coding 输出编码
        {
            get
            {
                return CmdData_.输出编码;
            }
            set
            {
                CmdData_.输出编码 = value;
            }
        }
        public string 名字
        {
            set
            {
                UpdateProperty(ref CmdData_.名字, value);
            }
            get
            {
                return CmdData_.名字;
            }
        }
        public string 路径
        {
            set
            {
                UpdateProperty(ref CmdData_.路径, value);
            }
            get
            {
                return CmdData_.路径;
            }
        }
        public string 命令
        {
            set
            {
                UpdateProperty(ref CmdData_.命令, value);
            }
            get
            {
                return CmdData_.命令;
            }
        }
        public string 参数
        {
            set
            {
                UpdateProperty(ref CmdData_.参数, value);
            }
            get
            {
                return CmdData_.参数;
            }
        }
        public string 关闭指令
        {
            set
            {
                UpdateProperty(ref CmdData_.关闭指令, value);
            }
            get
            {
                return CmdData_.关闭指令;
            }
        }
        public bool 自动启动
        {
            set
            {
                UpdateProperty(ref CmdData_.自动启动, value);
            }
            get
            {
                return CmdData_.自动启动;
            }
        }
        public bool 远程控制
        {
            set
            {
                UpdateProperty(ref CmdData_.远程控制, value);
            }
            get
            {
                return CmdData_.远程控制;
            }
        }
        public bool 自动重启
        {
            set
            {
                UpdateProperty(ref CmdData_.自动重启, value);
            }
            get
            {
                return CmdData_.自动重启;
            }
        }
        public bool 启动反馈
        {
            set
            {
                UpdateProperty(ref CmdData_.启动反馈, value);
            }
            get
            {
                return CmdData_.启动反馈;
            }
        }
        public bool 关闭反馈
        {
            set
            {
                UpdateProperty(ref CmdData_.关闭反馈, value);
            }
            get
            {
                return CmdData_.关闭反馈;
            }
        }

        public string 运行路径
        {
            set
            {
                UpdateProperty(ref CmdData_.运行路径, value);
            }
            get
            {
                return CmdData_.运行路径;
            }
        }
        private void UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (Equals(properValue, newValue))
            {
                return;
            }
            properValue = newValue;

            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public record CommandObj
    {
        public string 列表指令 { get; set; }
        public string 启动指令 { get; set; }
        public string 关闭指令 { get; set; }
        public string 信息指令 { get; set; }
        public string 控制指令 { get; set; }
        public string 退出指令 { get; set; }
    }
    public record ConfigObj
    {
        public List<CmdData> 实例列表 { get; init; }
        public Robot 机器人连接 { get; init; }
        public RobotConfig 机器人设置 { get; init; }
        public CommandObj 机器人指令 { get; init; }
    }
}
