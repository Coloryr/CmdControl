using System.Collections.Generic;

namespace CmdControl.Objs
{
    public enum FC
    {
        Start, Stop, Restart, Input
    }
    public class Robot
    {
        public string 地址 { get; set; }
        public int 端口 { get; set; }
        public bool 自动连接 { get; set; }
    }
    public class RobotConfig
    {
        public long 运行群号 { get; set; }
        public long 机器人号 { get; set; }
    }
    public class CmdData
    {
        public string 名字 { get; set; }
        public string 路径 { get; set; }
        public string 命令 { get; set; }
        public string 参数 { get; set; }
        public string 关闭指令 { get; set; }
        public bool 自动启动 { get; set; }
        public bool 远程控制 { get; set; }
        public bool 崩溃重启 { get; set; }
        public bool 启动反馈 { get; set; }
        public bool 关闭反馈 { get; set; }
    }
    public class CommandObj
    {
        public string 列表指令 { get; set; }
        public string 启动指令 { get; set; }
        public string 关闭指令 { get; set; }
    }
    public class ConfigObj
    {
        public List<CmdData> 实例列表 { get; init; }
        public Robot 机器人连接 { get; init; }
        public RobotConfig 机器人设置 { get; init; }
        public CommandObj 机器人指令 { get; init; }
    }
}
