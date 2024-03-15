

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoldSprite.MyUdpperAPI {
    public abstract class SimpleInboundHandler<T> : InboundHandlerAdapter {
        public abstract override void ExceptionCaught(IHandlerContext ctx, Exception e);

        public override void Read(IHandlerContext ctx, object msg)
        {
            Read0(ctx, (T)msg);
        }

        public abstract void Read0(IHandlerContext ctx, T msg);
    }
}
