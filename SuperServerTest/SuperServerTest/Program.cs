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
            SuperServer.Server server = new SuperServer.Server();


            server.Start();
        }
    }
}
