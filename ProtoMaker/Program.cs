using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoMaker
{
    class Program
    {
        static void Main()
        {
            string Path = @"C:\Users\Admin\Desktop\AzurLaneData-master\AzurLaneData-master\ko-KR\net\protocol\p11_pb.lua";
            string[] TextLines = File.ReadAllLines(Path);
            List<string> Contents = new List<string>();
            string word = Console.ReadLine().ToUpper();
            Console.Clear();

            foreach (string line in TextLines)
            {
                if (line.Contains(word) && !line.Contains("Descriptor()"))
                {
                    Contents.Add(line);
                }
            }

            var name = new List<string>();
            var type = new List<string>();

            foreach (string line in Contents)
            {
                if (line.Contains(".name"))
                {
                    string key = "_FIELD.name = ";
                    int idx = line.IndexOf(key);
                    if (idx == -1) continue;
                    string temp = line.Substring(idx + key.Length);
                    temp.Replace("\"", "");
                    name.Add(temp);
                }
                if (line.Contains(".type"))
                {
                    string key = "_FIELD.type = ";
                    int idx = line.IndexOf(key);
                    if (idx == -1) continue;
                    string temp = line.Substring(idx + key.Length);
                    type.Add(temp);
                }
            }

            string T = "";
            int flag = 0;
            for (int i = 0; i < name.Count; i++)
            {
                if (name[i].ToLower().Contains("list"))
                {
                    flag = 1;
                }
            }
            if(flag == 1)
            {
                T = "using ProtoBuf;\nusing System.Collections.Generic;\n\nnamespace p11\n{\n    [ProtoContract]\n    public class " + word.ToLower() + "\n    {";
            }
            else
            {
                T = "using ProtoBuf;\n\nnamespace p11\n{\n    [ProtoContract]\n    public class " + word.ToLower() + "\n    {";
            }
            for (int i = 0; i < name.Count; i++)
            {

                T = T + string.Format("\n        [ProtoMember({0})]", i + 1);
                if (type[i] == "13")
                {
                    if(name[i].ToLower().Contains("list"))
                        T = T + "\n        public List<uint> " + name[i] + " { get; set; }";
                    else
                        T = T + "\n        public uint " + name[i] + " { get; set; }";
                }
                else if (type[i] == "9")
                {
                    if (name[i].ToLower().Contains("list"))
                        T = T + "\n        public List<string> " + name[i] + " { get; set; }";
                    else
                        T = T + "\n        public string " + name[i] + " { get; set; }";
                }
                else if (type[i] == "10" || type[i] == "11")
                {
                    if (name[i].ToLower().Contains("list"))
                        T = T + "\n        public List<MESSAGE> " + name[i] + " { get; set; }";
                    else
                        T = T + "\n        public MESSAGE " + name[i] + " { get; set; }";
                }
                if (!(i == name.Count - 1))
                    T = T + "\n";
            }
            T = T +
@"
    }
}";
            Console.WriteLine(T.Replace("\"", ""));
            Main();
        }
    }
}
