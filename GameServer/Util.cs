using System;
using System.Linq;
using System.Text;

namespace Tool
{
    public class Util
    {
        public byte[] StringToBytes(string str)
        {
            byte[] StrByte = Encoding.UTF8.GetBytes(str);
            return StrByte;
        }

        public string BytesToString(byte[] strByte)
        {
            string str = Encoding.Default.GetString(strByte);
            return str;
        }

        public string PrintBytes(byte[] bytes)
        {
            return string.Join(", ", bytes);
        }

        public void ColorMsg(ConsoleColor backcolor, ConsoleColor forecolor, string format, params object[] args)
        {
            Console.BackgroundColor = backcolor;
            Console.ForegroundColor = forecolor;
            Console.Write($"\r{format}\n", args);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public byte[] HexToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
