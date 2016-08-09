using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SuperServer
{
    /// <summary>
    /// 服务器启动对象
    /// </summary>
    public class Server
    {
        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            bool ret = false;

            //加载服务器配置
            SuperServerConfig ServerConfig = (SuperServerConfig)System.Configuration.ConfigurationManager.GetSection("SuperServerConfig");

            //IPAddress ip = IPAddress.Parse(ServerConfig.IP);
            //IPEndPoint ipe = new IPEndPoint(ip, ServerConfig.Port);

            //AsyncServer server = new AsyncServer(100, 1024);

            //server.Init();

            //server.Start(ipe);

            SuperServer server = new SuperServer(ServerConfig);

            server.Start();



            return ret;
        }
    }
}
