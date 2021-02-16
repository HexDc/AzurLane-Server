using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.IO;
using ProtoBuf;

namespace Commander
{
    public class PRO_COMMANDER
    {
        public string TYPE { get; set; }
        public string BOX1 { get; set; }
        public string BOX2 { get; set; }
    }
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Permission.Content = "Processing ..";
            string id = ID.Text;
            string pw = PW.Password;
            OnConnect(id, pw);

        }
        private void OnConnect(string _id, string _pw)
        {
            try
            {
                /////////////////////////////
                IniFile ini = new IniFile();
                ini.Load(Directory.GetCurrentDirectory() + "\\config.ini");

                string svrAddr = ini["Default"]["IP"].ToString();
                int svrPort = ini["Default"]["PORT"].ToInt();
                /////////////////////////////
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(svrAddr), svrPort);
                socket.Connect(endPoint);
                byte[] buffer = new byte[512];
                //BLOCKING
                MemoryStream memoryStream = new MemoryStream();
                Serializer.Serialize(memoryStream, new PRO_COMMANDER
                {
                    TYPE = "LOGIN",
                    BOX1 = _id,
                    BOX2 = _pw
                });
                byte[] Array = memoryStream.ToArray();
                memoryStream.Close();
                socket.Send(Array, 0, Array.Length, SocketFlags.None);
                while (Encoding.UTF8.GetString(buffer) != "disconnect")
                {
                    switch (Encoding.Default.GetString(buffer).Trim())
                    {
                        case "SUCCESS"://로그인 성공
                            Permission.Content = "Administrator";
                            break;
                        case "FAILED"://로그인 실패
                            Permission.Content = "FAILED TO LOGIN";
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "ERROR");
                Environment.Exit(0);
            }
        }
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
