using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

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


    }
}
