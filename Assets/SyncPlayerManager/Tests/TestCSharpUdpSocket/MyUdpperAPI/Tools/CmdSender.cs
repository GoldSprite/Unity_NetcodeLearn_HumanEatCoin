using System;

namespace GoldSprite.MyUdpperAPI {
    public class CmdSender {
        public static CmdSender Instance { get; private set; } = new CmdSender();

        public CmdSender()
        {
#if APPLICATION_CONSOLE || APPLICATION_CLOUD
            LogTools.NLogInfo(helpMsg);
            while (true) {
                var str = Console.ReadLine();
                var cmd = str.Split(' ');
                CmdHandler(cmd);
            }
#endif
        }


        string helpMsg = "please input [help] to show the commands menu.";
        public void CmdHandler(string[] cmd)
        {
            try {
                string cmdHead = cmd[0];
                switch (cmdHead) {
                    case "help": {
                        var helpManual = "commands: "
                                + "\n停止链接: stop"
                                + "\n登录: login name password"
                                //                        + "\n在线玩家列表: list"
                                //                        + "\n移动: move x y z"
                                + "\n消息: msg message..."
                                + "\n广播: broadcast message..."
                                //+ "\n日志等级: loglevel int-level(1~5 ERR WARN DEBUG INFO MSG) int-onoff(1~0)"
                                //                        + "\n自杀: kill"
                                ;
                        LogTools.NLog(ILogLevel.FORCE, helpManual);
                        break;
                    }
                    case "stop": {
                        Udpper.Singleton.Shutdown();
                        break;
                    }
                    case "msg": {
                        var msg = String.Join(" ", cmd, 1, cmd.Length - 1);
                        LogTools.NLogMsg("你发了: " + msg);
                        Cmd_SendMsg(msg);
                        break;
                    }
                    case "broadcast": {
                        var msg = String.Join(" ", cmd, 1, cmd.Length - 1);
                        LogTools.NLogMsg("你广播了: " + msg);
                        Cmd_SendBroadcast(msg);
                        break;
                    }
                    case "login": {
                        Cmd_Login(cmd[1], cmd[2]);
                        break;
                    }
                    case "list": {
                        break;
                    }
                    case "move": {
                        break;
                    }
                    //case "loglevel": {
                    //    var key = Integer.parseInt(cmd[1]);
                    //    var onoff = "1".equals(cmd[2]);
                    //    LogTools.logLevels.replace(key, onoff);
                    //    LogTools.NLog(ILogLevel.FORCE, "logLevel-" + ILogLevel.getLogMsg(key) + (onoff ? "开启" : "关闭") + ".");
                    //    break;
                    //}
                    default:
                        LogTools.NLogInfo(helpMsg);
                        break;
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }


        public void Cmd_Login(string username, string pwd)
        {
            var pk = new LoginRequestPacket { UserName = username, Password = pwd };
            Udpper.Singleton.NetTrans.SendPacket(pk);
        }

        public void Cmd_SendMsg(String msg)
        {
            var pk = new MessageRequestPacket() { Message = msg };
            Udpper.Singleton.NetTrans.SendPacket(pk);
        }

        public void Cmd_SendBroadcast(String msg)
        {
            var pk = new BroadcastRequestPacket() { Message = msg };
            if (Udpper.Singleton.NetTrans.IsServer) {
                var sender = Udpper.Singleton.ConnectData.LocalAddress;
                Udpper.Singleton.NetTrans.PacketHandler.HandleBroadcastRequestPacket(pk, sender);
            } else {
                Udpper.Singleton.NetTrans.SendPacket(pk);
            }
        }
    }
}