using SuperServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();

            server.Start();

            while (true)
            {
                string cmd = Console.ReadLine();

                if (cmd == "quit")
                {
                    server.Stop();
                    break;
                }
                //主线程逻辑
            }



        }
    }
}
