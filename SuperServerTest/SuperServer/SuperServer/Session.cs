using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SuperServer
{
    /// <summary>
    /// 连接对象
    /// </summary>
    public class Session
    {
        public void Init(BufferManager buffermanager,Socket socket)
        {
            _EventArgs = new SocketAsyncEventArgs();
            _EventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            _EventArgs.UserToken = this;

            buffermanager.SetBuffer(_EventArgs);

            _Socket = socket;
        }

        public void Clear(BufferManager buffermanager)
        {
            buffermanager.FreeBuffer(_EventArgs);
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive();
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend();
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        public void ProcessReceive()
        {





            ProcessReceive();
        }

        public void ProcessSend()
        {



        }

        
        private Socket _Socket = null;


        private SocketAsyncEventArgs _EventArgs = null;

        public Socket Socket
        {
            get { return _Socket; }
            set { _Socket = value; }
        }

        public SocketAsyncEventArgs EventArgs
        {
            get { return _EventArgs; }
            set { _EventArgs = value; }
        }
    }
}
