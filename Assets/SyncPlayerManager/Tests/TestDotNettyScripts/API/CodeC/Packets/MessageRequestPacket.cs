using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldSprite.TestDotNetty_API
{
    public class MessageRequestPacket : Packet
    {
        public string Message { get; set; }
        public override byte Command => ICommand.MESSAGE_REQUEST;
    }
}
