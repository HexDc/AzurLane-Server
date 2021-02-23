using System;

namespace AZ_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TSingleton<AZLOG>.CreateInstance();
            TSingleton<GameServer>.Instance.Start();
        }
    }
}
