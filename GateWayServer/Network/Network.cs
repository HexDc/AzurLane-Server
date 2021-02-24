using Tool;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace GNetwork
{
    public class Network
    {
        Util util;
        TcpListener TL;
        TcpClient TC;
        Dictionary<TcpClient, string> Clients;
        int count;
        private int _port;
        const string Http = "HTTP/1.1 200 OK\nContent-Type: text/plain;charset=utf-8\nAccess-Control-Allow-Origin:\n\n";
        public Network(int port = 80)
        {
            util = new Util();
            Clients = new Dictionary<TcpClient, string>();
            _port = port;
        }

        public bool Start()
        {
            Console.WriteLine($"Network working on {_port}");
            new Thread(new ThreadStart(ServiceStart)).Start();
            return true;
        }

        private void ServiceStart()
        {
            TL = new TcpListener(IPAddress.Any, _port);
            TL.Start();
            byte[] m_bData = new byte[1024];

            while (true)
            {
                try
                {
                    ++count;
                    TC = TL.AcceptTcpClient();
                    Console.WriteLine(count.ToString() + TC.Client.RemoteEndPoint.ToString());
                    Clients.Add(TC, TC.Client.RemoteEndPoint.ToString());
                    Client client = new Client();
                    client.OnReceived += new Client.Received(OnReceived);
                    client.OnDisconnected += new Client.Disconnected(OnDisconnected);
                    client.StartClient(TC);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    TC.Close();
                }
            }
        }

        public void OnDisconnected(TcpClient client)
        {
            lock (client)
            {
                --count;
                string ip;
                if (Clients.TryGetValue(client, out ip))
                    Console.WriteLine("Disconnected" + ip);

                if (Clients.ContainsKey(client))
                    Clients.Remove(client);
                try
                {
                    client.Close();
                }
                catch { }
            }
        }

        private void OnReceived(byte[] data, TcpClient client)
        {
            int command = data[3] << 8 | data[4];
            int idx = data[5] << 8 | data[6];
            string result = $"command: {command} | idx: {idx}\n";
            switch (command)
            {
                case 8239:
                    Send(client, "Hello World웱");
                    break;

            }
            Console.WriteLine(Encoding.Default.GetString(data));
        }

        private void Send(TcpClient Client, byte[] data)
        {
            NetworkStream stream = Client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void Send(TcpClient Client, string msg, bool isWeb = true)
        {
            NetworkStream stream = Client.GetStream();
            if (isWeb)
            {
                msg = Http + msg;
            }
            stream.Write(Encoding.Default.GetBytes(msg), 0, Encoding.Default.GetByteCount(msg));
            Client.Close();
        }

        private void SendToAll(byte[] data)
        {
            foreach (var pair in Clients)
            {
                TcpClient client = pair.Key;
                try
                {
                    if (client.Connected && (client != null))
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

    }
}
