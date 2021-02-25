using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Scripts
{
    internal class SCMD
    {
        private static string GetCmd(byte[] data)
        {
            return Encoding.UTF8.GetString(data).Contains("?cmd=load_server?") ? Load_Server() : "NoCmd";
        }

        private static string Load_Server()
        {
            IniFile ini = new IniFile();
            ini.Load(Directory.GetCurrentDirectory() + "\\config\\gatewaysvr.ini");
            ArrayList arr = new ArrayList();
            for (int i = 0; i < ini["Default"]["server_cnt"].ToInt(); i++)
            {
                arr.Add(new Dictionary<string, object>
                {
                    {"id", i+1 },
                    { "name", ini[$"Server_{i+1}"]["name"].ToString() },
                    {"state", ini[$"Server_{i+1}"]["state"].ToInt() },
                    {"flag", 1 },
                    {"sort", ini[$"Server_{i+1}"]["sort"].ToInt() }
                });
            }
            return JsonFx.Json.JsonWriter.Serialize(arr);
        }

        public static string OnWeb(byte[] data)
        {
            return GetCmd(data);
        }
    }
}
