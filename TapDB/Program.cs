using System;
using TNetwork;

namespace TapDB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TapDB";
            new Network().Start();
        }
    }
}
