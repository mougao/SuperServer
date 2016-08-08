using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

            StartAccept();

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

        private void StartAccept()
        {
            
        }

        private void ProcessAccept()
        {
            
        }

        public void ProcessReceive()
        {
            
        }

        public void ProcessSend()
        {
            
        }

        public void CloseClientSocket()
        {
            
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
    }
}
