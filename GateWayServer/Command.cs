using GNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateWayServer
{
    public class Command : TSingleton<Command>
    {
        public void CommandProcess()
        {
            while (true)
            {
                string[] split = Console.ReadLine().ToLower().Split(' ');
                if (split.Length > 0 && split[0].StartsWith("/"))
                {
                    Network network = TSingleton<Network>.Instance;
                    switch (split[0].Replace("/", ""))
                    {
                        case "list":
                            Console.WriteLine("Currently Client Count: {0}", network.GetUserCount());
                            int i = 0;
                            foreach (string item in network.GetUserIPList())
                            {
                                ++i;
                                Console.WriteLine("[{0}] - {1}", i, item);
                            }
                            break;
                        case "clear":
                            Console.Clear();
                            Console.WriteLine("Console Cleared -- {0}", DateTime.Now.ToString("yyyy-MM-dd::HH-mm-ss"));
                            break;
                        case "help":
                            Help();
                            break;
                        default:
                            Console.WriteLine("Unknown Command: {0}", split);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("If you want to use Command, please Append '/'");
                }
            }
        }

        private void Help()
        {
            Console.WriteLine("--------------help--------------");
            Console.WriteLine("/help - display command list");
            Console.WriteLine("/list - display current user list(ip)");
            Console.WriteLine("/clear - clear console display");
        }
    }
}
