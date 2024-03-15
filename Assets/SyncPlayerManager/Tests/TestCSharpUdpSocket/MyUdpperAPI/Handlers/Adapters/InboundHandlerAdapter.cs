

using System;
using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public abstract class InboundHandlerAdapter : HandlerAdapter {
        public abstract override void ExceptionCaught(IHandlerContext ctx, Exception e);
        public abstract override void Read(IHandlerContext ctx, object msg);
    }
}