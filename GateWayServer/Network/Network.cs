﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Tool;
using Scripts;
using GateWayServer;
using p10;
using System.IO;
using ProtoBuf;

namespace GNetwork
{
    public class Network : TSingleton<Network>
    {
        private readonly Util util = new Util();
        private TcpListener TL;
        private TcpClient TC;
        private readonly Dictionary<TcpClient, string> Clients = new Dictionary<TcpClient, string>();
        private int port;
        private const string Http = "HTTP/1.1 200 OK\nContent-Type: text/plain;charset=utf-8\nAccess-Control-Allow-Origin:\n\n";

        public bool Start(int _port)
        {
            port = _port;
            Console.WriteLine($"Network working on {port}");
            new Thread(new ThreadStart(ServiceStart)).Start();
            return true;
        }

        private void ServiceStart()
        {
            TL = new TcpListener(IPAddress.Any, 14009);
            TL.Start();
            byte[] m_bData = new byte[1024];

            while (true)
            {
                try
                {
                    TC = TL.AcceptTcpClient();
                    Console.WriteLine(Clients.Count + 1 + " . " + TC.Client.RemoteEndPoint.ToString());
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
                if (Clients.TryGetValue(client, out string ip))
                {
                    Console.WriteLine("Disconnected . " + ip);
                }
                if (Clients.ContainsKey(client))
                {
                    Clients.Remove(client);
                }
                try
                {
                    client.Close();
                }
                catch (Exception) { }
            }
        }

        private void OnReceived(byte[] data, TcpClient client)
        {
            int command = data[3] << 8 | data[4];
            int idx = data[5] << 8 | data[6];
            string result = $"command: {command} | idx: {idx}\n";
            util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, result);
            switch (command)
            {
                case 8239:
                    Send(client, SCMD.OnWeb(data), true);
                    break;
                case 10800:
                    //Send(client, S10801.OnHash(), false);
                    SendToAll(S10801.OnHash());
                    break;
                case 10020:
                    //Send(client, new S10021().OnLogin(), false);
                    SendToAll(new S10021().OnLogin());
                    break;
                case 10022:
                    MemoryStream ms = new MemoryStream(util.PacketHeaderRemove(util.PacketResize(data)));
                    cs_10022 _10022 = Serializer.Deserialize<cs_10022>(ms);
                    Console.WriteLine($"    account_id: {_10022.account_id}");
                    Console.WriteLine($"    server_ticket: {_10022.server_ticket}");
                    Console.WriteLine($"    platform: {_10022.platform}");
                    Console.WriteLine($"    serverid: {_10022.serverid}");
                    Console.WriteLine($"    check_key: {_10022.check_key}");
                    Console.WriteLine($"    device_id: {_10022.device_id}");
                    //Send(client, new S10023().OnTutorial(), false);
                    SendToAll(new S10023().OnTutorial());
                    //Send(client, S11000._11000(), true);
                    break;
                default:
                    util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, result);
                    util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, "Unknown Packet:");
                    util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, util.PrintBytes(data));
                    break;
            }
        }

        private void Send(TcpClient Client, byte[] data, bool isNeedDisconn = false)
        {
            lock (Client)
            {
                using NetworkStream stream = Client.GetStream();
                stream.Write(data, 0, data.Length);
                if(isNeedDisconn)
                {
                    OnDisconnected(Client);
                }
            }
        }

        private void Send(TcpClient Client, string msg, bool isWeb = true)
        {
            lock (Client)
            {
                using NetworkStream stream = Client.GetStream();
                if (isWeb)
                {
                    msg = Http + msg;
                }
                byte[] data = Encoding.UTF8.GetBytes(msg);
                stream.Write(data, 0, data.Length);
                if (isWeb)
                {
                    OnDisconnected(Client);
                }
            }
        }

        public int GetUserCount()
        {
            return Clients.Count;
        }

        public List<string> GetUserIPList()
        {
            List<string> p = new List<string>();
            foreach(string item in Clients.Values)
            {
                p.Add(item);
            }
            return p;
        }

        private void SendToAll(byte[] data)
        {
            foreach (KeyValuePair<TcpClient, string> pair in Clients)
            {
                TcpClient client = pair.Key;
                {
                    try
                    {
                        if (client.Connected && (client != null))
                        {
                            using NetworkStream stream = client.GetStream();
                            stream.Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        OnDisconnected(client);
                    }
                }
            }

        }
    }
}
