using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProtoMaker
{
    class Program
    {
        static void Main()
        {
            string pname = Console.ReadLine();
            VT(pname.ToLower());
        }
        static void VP(string p)
        {
            VT(p);
        }
        static void VT(string pname)
        {
            if(pname.IndexOf("_pb") == -1)
                pname = pname + "_pb";
            string Path = @"C:\Users\Admin\Desktop\AzurLaneData-master\AzurLaneData-master\ko-KR\net\protocol\"+pname+".lua";
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
            var message = new List<string>();

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
                if(line.Contains(".message_type"))
                {
                    string key = "_FIELD.message_type = ";
                    int idx = line.IndexOf(key);
                    if (idx == -1) continue;
                    string temp = line.Substring(idx + key.Length).Replace("slot0.", "").Replace("slot1.", "").Replace("slot2.", "");

                    message.Add(temp);
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
                pname = pname.Replace("_pb", "");
            char[] rev = word.ToCharArray().Reverse().ToArray();
            if (!int.TryParse(rev[0].ToString(), out _))
            {
                word = word.ToUpper();
            }
            else
            {
                word = word.ToLower();
            }
            if (flag == 1)
            {
                T = "using ProtoBuf;\nusing System.Collections.Generic;\n\nnamespace "+pname+"\n{\n    [ProtoContract]\n    public class " + word + "\n    {";
            }
            else
            {
                T = "using ProtoBuf;\n\nnamespace "+pname+"\n{\n    [ProtoContract]\n    public class " + word + "\n    {";
            }
            int k = 0;
            for (int i = 0; i < name.Count; i++)
            {

                T = T + string.Format("\n        [ProtoMember({0})]", i + 1);
                if (type[i] == "13")
                {
                    if (name[i].ToLower().Contains("list"))
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
                    if (message[k].ToUpper().Trim() != word.ToUpper().Trim())
                    {
                        if (name[i].ToLower().Contains("list"))
                            T = T + "\n        public List<" + message[k] + "> " + name[i] + " { get; set; }";
                        else
                            T = T + "\n        public " + message[k] + " " + name[i] + " { get; set; }";
                    }
                    else
                    {
                        if (name[i].ToLower().Contains("list"))
                            T = T + "\n        public List<" + message[k+1] + "> " + name[i] + " { get; set; }";
                        else
                            T = T + "\n        public " + message[k+1] + " " + name[i] + " { get; set; }";
                    }
                    ++k;

                }
                if (!(i == name.Count - 1))
                    T = T + "\n";
            }
            T = T +
@"
    }
}";
            Console.WriteLine(T.Replace("\"", ""));
            VP(pname);
        }
    }
}
