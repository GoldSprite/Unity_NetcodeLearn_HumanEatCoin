using System;
using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public abstract class OutboundHandlerAdapter : HandlerAdapter {
        public abstract override void ExceptionCaught(IHandlerContext ctx, Exception e);
        public abstract override void Flush(IHandlerContext ctx);
        public abstract override void Write(IHandlerContext ctx, object msg);
    }
}