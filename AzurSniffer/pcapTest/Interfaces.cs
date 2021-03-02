using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace pcapTest
{
    public partial class Interfaces : Form
    {
        private readonly List<LibPcapLiveDevice> interfaceList = new List<LibPcapLiveDevice>();

        public Interfaces()
        {
            InitializeComponent();
        }

        private void Interfaces_Load(object sender, EventArgs e)
        {
            LibPcapLiveDeviceList devices = LibPcapLiveDeviceList.Instance;

            foreach (LibPcapLiveDevice device in devices)
            {
                if (!device.Interface.Addresses.Exists(a => a != null && a.Addr != null && a.Addr.ipAddress != null))
                {
                    continue;
                }

                PcapInterface devInterface = device.Interface;
                string friendlyName = devInterface.FriendlyName;
                string description = devInterface.Description;

                interfaceList.Add(device);
                mInterfaceCombo.Items.Add(friendlyName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mInterfaceCombo.SelectedIndex >= 0 && mInterfaceCombo.SelectedIndex < interfaceList.Count)
            {
                MainForm openMainForm = new MainForm(interfaceList, mInterfaceCombo.SelectedIndex);
                Hide();

                openMainForm.TargetIP = textBox1.Text;
                openMainForm.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}

