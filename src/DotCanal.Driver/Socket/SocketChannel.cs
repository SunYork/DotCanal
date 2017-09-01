using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace DotCanal.Driver.Socket
{
    /// <summary>
    /// 封装DotNetty的通信channel和数据接收缓存，实现读、写、连接校验的功能。
    /// </summary>
    public class SocketChannel
    {
        private object _lock = new object();
        private IByteBuffer _cache = PooledByteBufferAllocator.Default.Buffer(1024 * 1024);

        public IChannel Channel { get; set; }

        public void WriteCache(IByteBuffer buffer)
        {
            lock (_lock)
            {
                _cache.DiscardReadBytes();
                _cache.WriteBytes(buffer);
            }
        }

        public void WriteChannel(params byte[][] buffers)
        {
            if (Channel != null && Channel.IsWritable)
            {
                Channel.WriteAndFlushAsync(Unpooled.CopiedBuffer(buffers));
            }
            else
            {
                throw new IOException("Write Failed, Please Checking !");
            }
        }

        public byte[] Read(int readSize)
        {
            do
            {
                if (readSize > _cache.ReadableBytes)
                {
                    if (Channel == null)
                        throw new IOException("Socket has Interrupted !");
                    lock(this)
                    {
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    byte[] back = new byte[readSize];
                    lock(_lock)
                    {
                        _cache.ReadBytes(back);
                    }
                    return back;
                }
            } while (true);
        }

        public bool IsConnected
        {
            get
            {
                return Channel != null ? true : false;
            }
        }

        public EndPoint RemoteSocketAddress
        {
            get
            {
                return Channel != null ? Channel.RemoteAddress : null;
            }
        }

        public void Close()
        {
            Channel?.CloseAsync();
            _cache.DiscardReadBytes();
            _cache.Release();
        }
    }
}
