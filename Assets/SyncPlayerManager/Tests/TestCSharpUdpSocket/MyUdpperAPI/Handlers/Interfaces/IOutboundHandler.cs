using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public interface IOutboundHandler: IExceptionHandler {
        void Write(IHandlerContext ctx, object msg);
        void Flush(IHandlerContext ctx);
    }
}