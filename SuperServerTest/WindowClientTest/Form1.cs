using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowClientTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int port = 2000;
                string host = "127.0.0.1";
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndPoint实例
                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket
                Console.WriteLine("Conneting...");
                c.Connect(ipe);//连接到服务器

                for (int i = 0; i < 3; i++)
                {
                    string sendStr = "hello!This is a socket test";
                    byte[] bs = Encoding.ASCII.GetBytes(sendStr);
                    Console.WriteLine("Send Message");
                    c.Send(bs, bs.Length, 0);//发送测试信息
                    //string recvStr = "";
                    //byte[] recvBytes = new byte[1024];
                    //int bytes;
                    //bytes = c.Receive(recvBytes, recvBytes.Length, 0);//从服务器端接受返回信息
                    //recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                    //Console.WriteLine("Client Get Message:{0}", recvStr);//显示服务器返回信息
                }

                c.Close();
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(string.Format("ArgumentNullException: {0}",ex));
            }
            catch (SocketException ex)
            {
                MessageBox.Show(string.Format("SocketException: {0}",ex));
            }

            MessageBox.Show("发起一次连接");
        }
    }
}
