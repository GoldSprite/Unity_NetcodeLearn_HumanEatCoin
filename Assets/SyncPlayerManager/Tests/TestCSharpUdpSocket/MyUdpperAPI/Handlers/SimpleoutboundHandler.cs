

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoldSprite.MyUdpperAPI {
    public abstract class SimpleOutboundHandler<T> : OutboundHandlerAdapter {
        public abstract override void ExceptionCaught(IHandlerContext ctx, Exception e);

        public override void Write(IHandlerContext ctx, object msg)
        {
            Write0(ctx, (T)msg);
        }

        public abstract void Write0(IHandlerContext ctx, T msg);
    }
}
