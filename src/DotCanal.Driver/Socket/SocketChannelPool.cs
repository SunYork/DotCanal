using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Common.Utilities;
using System.Net;
using System.Threading.Tasks;

namespace DotCanal.Driver.Socket
{
    /// <summary>
    /// 实现channel的管理（监听连接、读数据、回收）
    /// </summary>
    public abstract class SocketChannelPool
    {
        private static IEventLoopGroup _group = new MultithreadEventLoopGroup();
        private static Bootstrap _boot = new Bootstrap();
        private static ConcurrentDictionary<IChannel, SocketChannel> _chManager = new ConcurrentDictionary<IChannel, SocketChannel>();

        static SocketChannelPool()
        {
            _boot.Group(_group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.SoRcvbuf, 32 * 1024)
                .Option(ChannelOption.SoSndbuf, 32 * 1024)
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoKeepalive, true)
                .Option(ChannelOption.RcvbufAllocator, new AdaptiveRecvByteBufAllocator())
                .Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    channel.Pipeline.AddLast(new BusinessHandler(_chManager));
                }));
        }

        public static async Task<SocketChannel> OpenAsync(EndPoint address)
        {
            var socket = new SocketChannel();
            var channel = await _boot.ConnectAsync(address);
            socket.Channel = channel;
            _chManager.TryAdd(socket.Channel, socket);

            return socket;
        }

        public class BusinessHandler : ChannelHandlerAdapter
        {
            private SocketChannel _socket;
            private ConcurrentDictionary<IChannel, SocketChannel> _chManager;

            public BusinessHandler(ConcurrentDictionary<IChannel, SocketChannel> chManager)
            {
                _chManager = chManager;
            }

            public override void ChannelInactive(IChannelHandlerContext context)
            {
                _socket.Channel = null;
                SocketChannel obj = null;
                _chManager.Remove(context.Channel, out obj);
            }

            public override void ChannelRead(IChannelHandlerContext context, object message)
            {
                var byteBuffer = message as IByteBuffer;
                if (_socket == null)
                {
                    _chManager.TryGetValue(context.Channel, out _socket);
                }

                if(_socket != null)
                {
                    _socket.WriteCache(byteBuffer);
                }

                //防止内存泄漏
                ReferenceCountUtil.Release(message);
            }

            public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
            {
                context.CloseAsync().Wait();
            }
        }
    }
}
