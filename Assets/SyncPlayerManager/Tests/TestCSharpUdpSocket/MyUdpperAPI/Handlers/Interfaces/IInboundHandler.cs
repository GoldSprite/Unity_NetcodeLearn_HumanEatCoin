using System;
using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public interface IInboundHandler: IExceptionHandler {
        void Read(IHandlerContext ctx, object msg);
    }
}