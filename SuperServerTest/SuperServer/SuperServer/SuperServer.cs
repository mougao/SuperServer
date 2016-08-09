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
            
            IPAddress ip = IPAddress.Parse(config.IP);
            _Ipe = new IPEndPoint(ip, config.Port);
            _NumConnections = config.NumConnections;
            _ReceiveBufferSize = config.ReceiveBufferSize;
        }
        /// <summary>
        /// 服务器初始化
        /// </summary>
        public void Init()
        {
            _BufferPool = new BufferManager(_ReceiveBufferSize * _NumConnections * opsToPreAlloc,
                _ReceiveBufferSize);

            _TotalBytesRead = 0;
            _NumConnectedSockets = 0;

            _MaxNumberAcceptedClients = new Semaphore(_NumConnections, _NumConnections);
        }
        /// <summary>
        /// 服务器启动
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            if (_Ipe == null)
                return false;

            _ListenSocket = new Socket(_Ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _ListenSocket.Bind(_Ipe);
            _ListenSocket.Listen(_Ipe.Port);

            StartAccept(null);
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

            Session session = new Session();
            session.Init(_BufferPool, e.AcceptSocket);

            _Sessions.Add(session);

            session.ProcessReceive();

            StartAccept(e);
        }

        public void CloseClientSocket(Session session)
        {
            try
            {
                session.Socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception)
            {
                //TODO::关闭连接异常
            }

            session.Socket.Close();
            _Sessions.Remove(session);
            
            Interlocked.Decrement(ref _NumConnectedSockets);
            _MaxNumberAcceptedClients.Release();
            Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", _NumConnectedSockets);
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
        /// 总共读取的字节数量
        /// </summary>
        private int _TotalBytesRead;           
        /// <summary>
        /// 当前连接数量
        /// </summary>
        private int _NumConnectedSockets;      
        /// <summary>
        /// 信号量管理
        /// </summary>
        private Semaphore _MaxNumberAcceptedClients;
        /// <summary>
        /// 已经连接的对象集合
        /// </summary>
        private List<Session> _Sessions = new List<Session>();

    }
}
