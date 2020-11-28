using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdControl.Objs
{
    public record Robot(string 地址, int 端口, bool 自动连接);
    public record CmdData(string 名字, string 路径, int 命令, string 参数, bool 自动启动, bool 崩溃重启, bool 启动反馈, bool 崩溃反馈);
    public record CommandObj(string 指令列表, string 启动指令, string 关闭指令);
    public record ConfigObj
    {
        public Dictionary<string, CmdData> 实例列表 { get; init; }
        public Robot 机器人配置 { get; init; }
    }
}
