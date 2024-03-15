using System;
using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public interface IExceptionHandler {
        void ExceptionCaught(IHandlerContext ctx, Exception e);
    }
}