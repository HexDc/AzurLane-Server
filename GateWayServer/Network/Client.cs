using System;
using System.Threading;
using System.Net.Sockets;

namespace GNetwork
{
    public class Client
    {
        TcpClient client;

        public delegate void Disconnected(TcpClient client);
        public event Disconnected OnDisconnected;

        public delegate void Received(byte[] data, TcpClient client);
        public event Received OnReceived;

        public void StartClient(TcpClient tcpClient)
        {
            client = tcpClient;
            Thread t = new Thread(new ThreadStart(DoReceive));
            t.IsBackground = true;
            t.Start();
        }

        private void DoReceive()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();

                byte[] buffer = new byte[1024];
                while (client.Connected)
                {
                    if(stream.Read(buffer, 0, buffer.Length) > 1)
                        OnReceived(buffer, client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                if (client != null)
                {
                    OnDisconnected?.Invoke(client);
                    client.Close();
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                if (client != null)
                {
                    OnDisconnected?.Invoke(client);
                    client.Close();
                    stream.Close();
                }
            }
        }
    }
}
