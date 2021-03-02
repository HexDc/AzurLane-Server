using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace pcapTest
{
    public partial class MainForm : Form
    {
        private readonly List<LibPcapLiveDevice> interfaceList = new List<LibPcapLiveDevice>();
        private readonly int selectedIntIndex;
        private readonly LibPcapLiveDevice wifi_device;
        private CaptureFileWriterDevice captureFileWriter;
        private readonly Dictionary<int, Packet> capturedPackets_list = new Dictionary<int, Packet>();
        private int packetNumber = 1;
        private string time_str = "", sourceIP = "", destinationIP = "", protocol_type = "", length = "";
        private bool startCapturingAgain = false;
        private Thread sniffing;
        public string TargetIP;

        public MainForm(List<LibPcapLiveDevice> interfaces, int selectedIndex)
        {
            InitializeComponent();
            interfaceList = interfaces;
            selectedIntIndex = selectedIndex;
            wifi_device = interfaceList[selectedIntIndex];
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)// Start sniffing
        {
            if (startCapturingAgain == false) //first time 
            {
                System.IO.File.Delete(Environment.CurrentDirectory + "capture.pcap");
                wifi_device.OnPacketArrival += new PacketArrivalEventHandler(Device_OnPacketArrival);
                sniffing = new Thread(new ThreadStart(sniffing_Proccess));
                sniffing.Start();
                toolStripButton1.Enabled = false;
                toolStripButton2.Enabled = true;
                textBox1.Enabled = false;

            }
            else if (startCapturingAgain)
            {
                if (MessageBox.Show("Your packets are captured in a file. Starting a new capture will override existing ones.", "Confirm", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    // user clicked ok
                    System.IO.File.Delete(Environment.CurrentDirectory + "capture.pcap");
                    listView1.Items.Clear();
                    capturedPackets_list.Clear();
                    packetNumber = 1;
                    textBox2.Text = "";
                    wifi_device.OnPacketArrival += new PacketArrivalEventHandler(Device_OnPacketArrival);
                    sniffing = new Thread(new ThreadStart(sniffing_Proccess));
                    sniffing.Start();
                    toolStripButton1.Enabled = false;
                    toolStripButton2.Enabled = true;
                    textBox1.Enabled = false;
                }
            }
            startCapturingAgain = true;
        }

        private string PrintBytes(byte[] bytes)
        {
            return string.Join(", ", bytes);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            string protocol = e.Item.SubItems[4].Text;
            int key = int.Parse(e.Item.SubItems[0].Text);
            bool getPacket = capturedPackets_list.TryGetValue(key, out Packet packet);

            switch (protocol)
            {
                case "TCP":
                    if (getPacket)
                    {
                        TcpPacket tcpPacket = (TcpPacket)packet.Extract(typeof(TcpPacket));
                        if (tcpPacket != null)
                        {
                            string Data = PrintBytes(tcpPacket.Bytes).Replace(PrintBytes(tcpPacket.Header) + ",", "");
                            if (!string.IsNullOrEmpty(TargetIP) && Data.Length > 1)
                            {
                                textBox2.Text = "";
                                textBox2.Text = PacketDecrypt(Data);
                            }
                            else
                            {
                                int srcPort = tcpPacket.SourcePort;
                                int dstPort = tcpPacket.DestinationPort;
                                ushort checksum = tcpPacket.Checksum;
                                textBox2.Text = "";
                                textBox2.Text = "Packet number: " + key +
                                                " Type: TCP" +
                                                "\r\nSource port: " + srcPort +
                                                "\r\nDestination port: " + dstPort +
                                                "\r\nTCP header size: " + tcpPacket.DataOffset +
                                                "\r\nWindow size: " + tcpPacket.WindowSize + // bytes that the receiver is willing to receive
                                                                                             //"\r\nChecksum:" + checksum.ToString() + (tcpPacket.ValidChecksum ? ",valid" : ",invalid") +
                                                                                             //"\r\nTCP checksum: " + (tcpPacket.ValidTCPChecksum ? ",valid" : ",invalid") +
                                                                                             //"\r\nSequence number: " + tcpPacket.SequenceNumber.ToString() +
                                                                                             //"\r\nAcknowledgment number: " + tcpPacket.AcknowledgmentNumber + (tcpPacket.Ack ? ",valid" : ",invalid") +
                                                                                             //// flags
                                                                                             //"\r\nUrgent pointer: " + (tcpPacket.Urg ? "valid" : "invalid") +
                                                                                             //"\r\nACK flag: " + (tcpPacket.Ack ? "1" : "0") + // indicates if the AcknowledgmentNumber is valid
                                                                                             //"\r\nPSH flag: " + (tcpPacket.Psh ? "1" : "0") + // push 1 = the receiver should pass the data to the app immidiatly, don't buffer it
                                                                                             //"\r\nRST flag: " + (tcpPacket.Rst ? "1" : "0") + // reset 1 is to abort existing connection
                                                                                             //                                                 // SYN indicates the sequence numbers should be synchronized between the sender and receiver to initiate a connection
                                                                                             //"\r\nSYN flag: " + (tcpPacket.Syn ? "1" : "0") +
                                                                                             //// closing the connection with a deal, host_A sends FIN to host_B, B responds with ACK
                                                                                             //// FIN flag indicates the sender is finished sending
                                                                                             //"\r\nFIN flag: " + (tcpPacket.Fin ? "1" : "0") +
                                                                                             //"\r\nECN flag: " + (tcpPacket.ECN ? "1" : "0") +
                                                                                             //"\r\nCWR flag: " + (tcpPacket.CWR ? "1" : "0") +
                                                                                             //"\r\nNS flag: " + (tcpPacket.NS ? "1" : "0") +
                                                "\r\nRawBytes: " + Data;
                            }
                        }
                    }
                    break;
                case "UDP":
                    if (getPacket)
                    {
                        UdpPacket udpPacket = (UdpPacket)packet.Extract(typeof(UdpPacket));
                        if (udpPacket != null)
                        {
                            int srcPort = udpPacket.SourcePort;
                            int dstPort = udpPacket.DestinationPort;
                            ushort checksum = udpPacket.Checksum;

                            textBox2.Text = "";
                            textBox2.Text = "Packet number: " + key +
                                            " Type: UDP" +
                                            "\r\nSource port:" + srcPort +
                                            "\r\nDestination port: " + dstPort +
                                            "\r\nChecksum:" + checksum.ToString() + " valid: " + udpPacket.ValidChecksum +
                                            "\r\nValid UDP checksum: " + udpPacket.ValidUDPChecksum +
                                            "\r\nData: " + PrintBytes(udpPacket.Bytes) +
                                            "\r\nTryGetString: " + Encoding.Default.GetString(udpPacket.Bytes);

                        }
                    }
                    break;
                case "ARP":
                    if (getPacket)
                    {
                        ARPPacket arpPacket = (ARPPacket)packet.Extract(typeof(ARPPacket));
                        if (arpPacket != null)
                        {
                            System.Net.IPAddress senderAddress = arpPacket.SenderProtocolAddress;
                            System.Net.IPAddress targerAddress = arpPacket.TargetProtocolAddress;
                            System.Net.NetworkInformation.PhysicalAddress senderHardwareAddress = arpPacket.SenderHardwareAddress;
                            System.Net.NetworkInformation.PhysicalAddress targerHardwareAddress = arpPacket.TargetHardwareAddress;

                            textBox2.Text = "";
                            textBox2.Text = "Packet number: " + key +
                                            " Type: ARP" +
                                            "\r\nHardware address length:" + arpPacket.HardwareAddressLength +
                                            "\r\nProtocol address length: " + arpPacket.ProtocolAddressLength +
                                            "\r\nOperation: " + arpPacket.Operation.ToString() + // ARP request or ARP reply ARP_OP_REQ_CODE, ARP_OP_REP_CODE
                                            "\r\nSender protocol address: " + senderAddress +
                                            "\r\nTarget protocol address: " + targerAddress +
                                            "\r\nSender hardware address: " + senderHardwareAddress +
                                            "\r\nTarget hardware address: " + targerHardwareAddress;
                        }
                    }
                    break;
                case "ICMP":
                    if (getPacket)
                    {
                        ICMPv4Packet icmpPacket = (ICMPv4Packet)packet.Extract(typeof(ICMPv4Packet));
                        if (icmpPacket != null)
                        {
                            textBox2.Text = "";
                            textBox2.Text = "Packet number: " + key +
                                            " Type: ICMP v4" +
                                            "\r\nType Code: 0x" + icmpPacket.TypeCode.ToString("x") +
                                            "\r\nChecksum: " + icmpPacket.Checksum.ToString("x") +
                                            "\r\nID: 0x" + icmpPacket.ID.ToString("x") +
                                            "\r\nSequence number: " + icmpPacket.Sequence.ToString("x");
                        }
                    }
                    break;
                case "IGMP":
                    if (getPacket)
                    {
                        IGMPv2Packet igmpPacket = (IGMPv2Packet)packet.Extract(typeof(IGMPv2Packet));
                        if (igmpPacket != null)
                        {
                            textBox2.Text = "";
                            textBox2.Text = "Packet number: " + key +
                                            " Type: IGMP v2" +
                                            "\r\nType: " + igmpPacket.Type +
                                            "\r\nGroup address: " + igmpPacket.GroupAddress +
                                            "\r\nMax response time" + igmpPacket.MaxResponseTime;
                        }
                    }
                    break;
                default:
                    textBox2.Text = "";
                    break;
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)// last packet
        {
            ListView.ListViewItemCollection items = listView1.Items;
            ListViewItem last = items[items.Count - 1];
            last.EnsureVisible();
            last.Selected = true;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)// fist packet
        {
            ListViewItem first = listView1.Items[0];
            first.EnsureVisible();
            first.Selected = true;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)//next
        {
            if (listView1.SelectedItems.Count == 1)
            {
                int index = listView1.SelectedItems[0].Index;
                listView1.Items[index + 1].Selected = true;
                listView1.Items[index + 1].EnsureVisible();
            }
        }

        private void chooseInterfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Interfaces openInterfaceForm = new Interfaces();
            Hide();
            openInterfaceForm.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)// prev
        {
            if (listView1.SelectedItems.Count == 1)
            {
                int index = listView1.SelectedItems[0].Index;
                listView1.Items[index - 1].Selected = true;
                listView1.Items[index - 1].EnsureVisible();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)// Stop sniffing
        {
            try
            {
                sniffing.Abort();
                wifi_device.StopCapture();
                wifi_device.Close();
                captureFileWriter.Close();

                toolStripButton1.Enabled = true;
                textBox1.Enabled = true;
                toolStripButton2.Enabled = false;
            }
            catch
            {
                Environment.Exit(1);
            }
        }

        private void sniffing_Proccess()
        {
            // Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            wifi_device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            // Start the capturing process
            if (wifi_device.Opened)
            {
                if (textBox1.Text != "")
                {
                    wifi_device.Filter = textBox1.Text;
                }
                captureFileWriter = new CaptureFileWriterDevice(wifi_device, Environment.CurrentDirectory + "capture.pcap");
                wifi_device.Capture();
            }
        }
        public void Device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            DateTime time = e.Packet.Timeval.Date;
            time_str = (time.Hour + 1) + ":" + time.Minute + ":" + time.Second + ":" + time.Millisecond;
            length = e.Packet.Data.Length.ToString();
            Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            IpPacket ipPacket = (IpPacket)packet.Extract(typeof(IpPacket));
            if (ipPacket != null)
            {
                System.Net.IPAddress srcIp = ipPacket.SourceAddress;
                System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
                protocol_type = ipPacket.Protocol.ToString();
                sourceIP = srcIp.ToString();
                destinationIP = dstIp.ToString();

                if (!string.IsNullOrEmpty(TargetIP) && (sourceIP == TargetIP || destinationIP == TargetIP))
                {
                    packetNumber += 1;
                    capturedPackets_list.Add(packetNumber, packet);

                    Packet protocolPacket = ipPacket.PayloadPacket;

                    ListViewItem item = new ListViewItem(packetNumber.ToString());
                    item.SubItems.Add(time_str);
                    item.SubItems.Add(sourceIP);
                    item.SubItems.Add(destinationIP);
                    item.SubItems.Add(protocol_type);
                    item.SubItems.Add(length);
                    captureFileWriter.Write(e.Packet);

                    try
                    {
                        Action action = () => listView1.Items.Add(item);
                        listView1.Invoke(action);
                    }
                    catch
                    {
                        Environment.Exit(1);
                    }

                }
                else if (string.IsNullOrEmpty(TargetIP))
                {
                    packetNumber += 1;
                    capturedPackets_list.Add(packetNumber, packet);

                    Packet protocolPacket = ipPacket.PayloadPacket;

                    ListViewItem item = new ListViewItem(packetNumber.ToString());
                    item.SubItems.Add(time_str);
                    item.SubItems.Add(sourceIP);
                    item.SubItems.Add(destinationIP);
                    item.SubItems.Add(protocol_type);
                    item.SubItems.Add(length);
                    captureFileWriter.Write(e.Packet);

                    try
                    {
                        Action action = () => listView1.Items.Add(item);
                        listView1.Invoke(action);
                    }
                    catch
                    {
                        Environment.Exit(1);
                    }
                }
            }
        }
    }
}