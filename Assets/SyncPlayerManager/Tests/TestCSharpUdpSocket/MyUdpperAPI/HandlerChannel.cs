using DotNetty.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using DotNetty.Transport.Channels.Sockets;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace GoldSprite.MyUdpperAPI {
    public class HandlerChannel : IHandlerContext, IHandler {
        private readonly Udpper upr;
        private readonly Socket udpSocket;
        private Thread receiveThread;
        private readonly List<IHandler> inboundHandlers = new List<IHandler>();
        private IHandler currentInboundHandler;
        private readonly List<IHandler> outboundHandlers = new List<IHandler>();
        private IHandler currentOutboundHandler;
        private readonly IByteBufferAllocator alloc = new UnpooledByteBufferAllocator();
        private IByteBuffer buf;
        private DatagramPacket dpk;
        public HandlerChannel Channel => this;
        public IPEndPoint LocalPoint => upr.ConnectData.LocalAddress;


        #region 初始化
        public HandlerChannel(Socket udpSocket, Udpper upr)
        {
            this.udpSocket = udpSocket;
            this.upr = upr;
        }

        private void InitHandlers()
        {
            inboundHandlers.Add(this);
            currentInboundHandler = inboundHandlers[0];
            outboundHandlers.Insert(0, this);
            currentOutboundHandler = outboundHandlers[outboundHandlers.Count - 1];
        }


        public void Start()
        {
            InitHandlers();

            StartReceiveThread();
        }

        private void StartReceiveThread()
        {
            receiveThread = new Thread(() => {
                try {
                    while (true) {
                        EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                        var bytes = new byte[1024];
                        int byteLength = udpSocket.ReceiveFrom(bytes, ref sender);

                        var buf = Alloc().Buffer();
                        buf.WriteBytes(bytes, 0, byteLength);
                        LogTools.NLogDebug($"接收到{upr.NetTrans.IdentityName(sender)}数据包.");

                        var dpk = new DatagramPacket(buf, sender, null);
                        try {
                            currentInboundHandler.Read(this, dpk);
                        }
                        catch (Exception ex) {
                            currentInboundHandler.ExceptionCaught(this, ex);
                        }
                    }
                }catch (Exception ex) {
                    LogTools.NLogWarn("频道接收器已被关闭: "+ex.Message);
                }
            });
            receiveThread.Start();
        }

        public void Handlers(Action<HandlerChannel> initializer)
        {
            initializer?.Invoke(this);
        }

        public void AddLast(IHandler handler)
        {
            if (handler is InboundHandlerAdapter inboundHandler) {
                inboundHandlers.Add(inboundHandler);
            } else
            if (handler is OutboundHandlerAdapter outboundHandler) {
                outboundHandlers.Add(outboundHandler);
            }
        }
        #endregion


        #region 网络传输
        public void Read(IHandlerContext ctx, object msg)
        {
            currentInboundHandler = inboundHandlers[0];
        }

        public void Write(IHandlerContext ctx, object msg)
        {
            currentOutboundHandler = outboundHandlers[outboundHandlers.Count - 1];
            if (IsDataValid(msg, out DatagramPacket dpk)) this.dpk = dpk;
        }

        public void Flush()
        {
            var bytes = dpk.Content.Array;
            var recipient = dpk.Recipient;
            udpSocket.SendTo(bytes, recipient);
        }
        #endregion


        #region 管道传输
        public void ExceptionCaught(IHandlerContext ctx, Exception e)
        {
            LogTools.NLogErr("处理器异常: \n"+e.StackTrace);
        }
        public void FireChannelRead(object msg)
        {
            var handler = FindNextInboundHandler();
            handler?.Read(this, msg);
        }

        public void Write(object msg)
        {
            var handler = FindNextOutboundHandler();
            handler?.Write(this, msg);
        }

        public void Flush(IHandlerContext ctx)
        {
            var handler = FindNextOutboundHandler();
            handler?.Flush(this);
        }

        public void WriteAndFlushAsync(object msg)
        {
            try {
                currentOutboundHandler.Write(this, msg);
                currentOutboundHandler.Flush(this);
            }catch(Exception e) {
                currentOutboundHandler.ExceptionCaught(this, e);
            }
        }
        #endregion


        #region 功能
        public IByteBufferAllocator Alloc() { return alloc; }

        private IHandler FindNextInboundHandler()
        {
            var index = inboundHandlers.IndexOf(currentInboundHandler);
            return index + 1 < inboundHandlers.Count ? (currentInboundHandler = inboundHandlers[index + 1]) : null;
        }

        private IHandler FindNextOutboundHandler()
        {
            var index = outboundHandlers.IndexOf(currentOutboundHandler);
            return index - 1 >= 0 ? (currentOutboundHandler = outboundHandlers[index - 1]) : null;
        }

        private bool IsDataValid(object msg, out DatagramPacket outDpk)
        {
            if (!(msg is DatagramPacket dpk)) {
                throw new Exception("发包异常: 不是一个DatagramPacket数据类型.");
            }
            if (dpk == null || dpk.Content.ReadableBytes <= 0) {
                throw new Exception("发包异常: 内容为空.");
            }
            outDpk = dpk;
            return true;
        }

        public void CloseChannel()
        {
            try {
                receiveThread.Interrupt();
            }catch(Exception e) {
                LogTools.NLogWarn("频道接收器已关闭: " + e.Message);
            }
        }
        #endregion
    }
}
