using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuperServer
{
    /// <summary>
    /// 服务器实例
    /// </summary>
    public class SuperServer
    {
        public SuperServer(SuperServerConfig config)
        {
            if (config == null)
                return;
            _NumConnectedSockets = 0;
            _TotalBytesRead = 0;
            IPAddress ip = IPAddress.Parse(config.IP);
            _Ipe = new IPEndPoint(ip, config.Port);
            _NumConnections = config.NumConnections;
            _ReceiveBufferSize = config.ReceiveBufferSize;

            _ReadWritePool = new SocketAsyncEventArgsPool(config.NumConnections);

            _BufferPool = new BufferManager(_ReceiveBufferSize * _NumConnections * opsToPreAlloc,
                _ReceiveBufferSize);

            _MaxNumberAcceptedClients = new Semaphore(_NumConnections, _NumConnections);
        }
        /// <summary>
        /// 服务器初始化
        /// </summary>
        public void Init()
        {
            _BufferPool.InitBuffer();

            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < _NumConnections; i++)
            {
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.UserToken = new AsyncUserToken();

                _BufferPool.SetBuffer(readWriteEventArg);

                _ReadWritePool.Push(readWriteEventArg);
            }
        }
        /// <summary>
        /// 服务器启动
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            Init();

            if (_Ipe == null)
                return false;

            _ListenSocket = new Socket(_Ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _ListenSocket.Bind(_Ipe);
            _ListenSocket.Listen(_Ipe.Port);

            StartAccept(null);

            Console.WriteLine("服务器启动成功！");
            return true;
        }
        /// <summary>
        /// 服务器停止
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            bool ret = false;

            return ret;
        }

        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                acceptEventArg.AcceptSocket = null;
            }

            _MaxNumberAcceptedClients.WaitOne();

            bool willRaiseEvent = _ListenSocket.AcceptAsync(acceptEventArg);

            if (!willRaiseEvent)//同步完成
            {
                ProcessAccept(acceptEventArg);
            }
        }

        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref _NumConnectedSockets);
            Console.WriteLine("Client connection accepted. There are {0} clients connected to the server",
                _NumConnectedSockets);

            SocketAsyncEventArgs readEventArgs = _ReadWritePool.Pop();
            ((AsyncUserToken)readEventArgs.UserToken).Socket = e.AcceptSocket;

            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);

            if (!willRaiseEvent)
            {
                ProcessReceive(readEventArgs);
            }

            StartAccept(e);
        }


        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // check if the remote host closed the connection
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                Interlocked.Add(ref _TotalBytesRead, e.BytesTransferred);
                Console.WriteLine("The server has read a total of {0} bytes", _TotalBytesRead);
                string recvStr = Encoding.ASCII.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                Console.WriteLine("收到信息内容：{0} ", recvStr);
                //echo the data received back to the client
                e.SetBuffer(e.Offset, e.BytesTransferred);
                bool willRaiseEvent = token.Socket.SendAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessSend(e);
                }

            }
            else
            {
                CloseClientSocket(e);
            }
        }

  
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // done echoing data back to the client
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                // read the next block of data send from the client
                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            // close the socket associated with the client
            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) { }
            token.Socket.Close();

            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref _NumConnectedSockets);
            _MaxNumberAcceptedClients.Release();
            Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", _NumConnectedSockets);

            // Free the SocketAsyncEventArg so they can be reused by another client
            _ReadWritePool.Push(e);
        }


        const int opsToPreAlloc = 2;
        /// <summary>
        /// Ip信息
        /// </summary>
        private IPEndPoint _Ipe;
        /// <summary>
        /// 最大连接数
        /// </summary>
        private int _NumConnections;
        /// <summary>
        /// 读写缓存大小
        /// </summary>
        private int _ReceiveBufferSize;
        /// <summary>
        /// 缓存池
        /// </summary>
        private BufferManager _BufferPool;
        /// <summary>
        /// 监听Socket
        /// </summary>
        private Socket _ListenSocket;         
        /// <summary>
        /// 当前连接数量
        /// </summary>
        private int _NumConnectedSockets;
        /// <summary>
        /// 累计读取byte数量
        /// </summary>
        private int _TotalBytesRead;
        /// <summary>
        /// 信号量管理
        /// </summary>
        private Semaphore _MaxNumberAcceptedClients;
        /// <summary>
        /// 当前空闲连接对象集合
        /// </summary>
        SocketAsyncEventArgsPool _ReadWritePool;

    }
}
