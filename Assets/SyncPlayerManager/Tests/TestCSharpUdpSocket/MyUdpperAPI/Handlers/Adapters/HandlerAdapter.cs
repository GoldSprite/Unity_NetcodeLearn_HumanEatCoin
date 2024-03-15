

using System;
using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public abstract class HandlerAdapter : IHandler {
        public virtual void ExceptionCaught(IHandlerContext ctx, Exception e) { }
        public virtual void Flush(IHandlerContext ctx) { }

        public virtual void Write(IHandlerContext ctx, object msg) { }

        public virtual void Read(IHandlerContext ctx, object msg) { }
    }
}