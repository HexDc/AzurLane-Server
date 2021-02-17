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
using System.Threading;

namespace Commander
{
    [ProtoContract]
    public class PRO_COMMANDER
    {
        [ProtoMember(1)]
        public string TYPE { get; set; }

        [ProtoMember(2)]
        public string BOX1 { get; set; }

        [ProtoMember(3)]
        public string BOX2 { get; set; }
    }
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public string id;
        public string pw;
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
            id = ID.Text;
            pw = PW.Password;
            new Thread(new ThreadStart(OnConnect)).Start();
        }

        private void OnConnect()
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                Serializer.Serialize(memoryStream, new PRO_COMMANDER
                {
                    TYPE = "LOGIN",
                    BOX1 = id,
                    BOX2 = pw
                });
                byte[] Array = memoryStream.ToArray();
                memoryStream.Close();

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
