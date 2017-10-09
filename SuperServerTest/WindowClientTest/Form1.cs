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
        Socket c = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (c != null)
            {
                MessageBox.Show("连接未断开");
                return;
            }
                

            try
            {
                int port = 2000;
                string host = "127.0.0.1";
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndPoint实例
                c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket
                Console.WriteLine("Conneting...");
                c.Connect(ipe);//连接到服务器
                
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

        private void button2_Click(object sender, EventArgs e)
        {
            if(c == null)
                MessageBox.Show("连接已断开");
                
            string sendStr = "hello!This is a socket test";
            byte[] bs = Encoding.ASCII.GetBytes(sendStr);
            Console.WriteLine("Send Message");
            c.Send(bs, bs.Length, 0);//发送测试信息
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (c == null)
                MessageBox.Show("连接已断开");

            c.Close();

            c = null;
        }
    }
}
