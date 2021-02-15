using Tool;
using System;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Scripts
{
    class SCMD
    {
        static Util util = new Util();
        private static string GetCmd(byte[] data)
        {
            string m_szdata = util.BytesToString(data);
            if (m_szdata.Contains("?cmd=load_server?"))
            {
                util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, "?cmd=load_server?");
                return @"HTTP/1.1 200 OK
Content-Type: text/plain;charset=utf-8

" + Load_Server();
            }
            else
            {
                util.ColorMsg(ConsoleColor.White, ConsoleColor.Black, "NoCmd");

                return "NoCmd";
            }
        }
        private static string Load_Server()
        {
            Dictionary<string, object> dic_1 = new Dictionary<string, object>
            {
                { "id", 1 },
                { "name", "명량" },
                { "state", 0 },
                { "flag", 1 },
                { "sort", 1 }
            };
            Dictionary<string, object> dic_2 = new Dictionary<string, object>
            {
                { "id", 2 },
                { "name", "노량" },
                { "state", 0 },
                { "flag", 1 },
                { "sort", 0 }
            };

            Dictionary<string, object> dic_3 = new Dictionary<string, object>
            {
                { "id", 3 },
                { "name", "한산도" },
                { "state", 0 },
                { "flag", 1 },
                { "sort", 0 }
            };

            Dictionary<string, object> dic_4 = new Dictionary<string, object>
            {
                { "id", 4 },
                { "name", "옥계" },
                { "state", 0 },
                { "flag", 1 },
                { "sort", 0 }
            };
            ArrayList arr = new ArrayList
            {
                dic_1,
                dic_2,
                dic_3,
                dic_4
            };
            string data = JsonFx.Json.JsonWriter.Serialize(arr);
            return data;
        }//Just Dummy.. Client never Check
        public static void OnWeb(Socket ClientSocket, byte[] m_bBuffer)
        {
            byte[] buff = Encoding.UTF8.GetBytes(GetCmd(m_bBuffer));
            ClientSocket.Send(buff, 0, buff.Length, SocketFlags.None);
            return;
        }
    }
}
