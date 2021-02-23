using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AZ_Server
{
    public class GameServer : TSingleton<GameServer>
    {
        private Thread ServerThread;
        private TcpListener Listener;
        public void Start()
        {
            AZLOG.Info("Game Server Installed");
            ServerThread = new Thread(() =>
            {
                while (true)
                {
                    Listener.BeginAcceptTcpClient(AcceptTcpClient, null);
                }
            });
        }

        public void AcceptTcpClient(IAsyncResult IAR)
        {
        }
    }
}
