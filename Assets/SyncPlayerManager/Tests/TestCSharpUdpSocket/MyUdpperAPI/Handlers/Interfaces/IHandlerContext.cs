using DotNetty.Buffers;
using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public interface IHandlerContext {
        HandlerChannel Channel { get; }
        void FireChannelRead(object msg);
        void Write(object msg);
        void Flush();
        void WriteAndFlushAsync(object msg);
        IByteBufferAllocator Alloc();
    }
}