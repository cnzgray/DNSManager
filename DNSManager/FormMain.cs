using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace DNSManager
{
    public partial class FormMain : Form
    {
        private const string CONFIG_FILE = "config.json";
        private readonly List<DNSServer> dns = new List<DNSServer>();

        public FormMain()
        {
            InitializeComponent();
        }

        private NetworkInterface CurrentInterface => comboBoxNICList.SelectedItem as NetworkInterface;

        private DNSServer CurrentDNSServer
        {
            get
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    return (DNSServer)dataGridView1.SelectedRows[0].DataBoundItem;
                }
                return null;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadNI();

            dns.AddRange(
                (List<DNSServer>)
                    SimpleJson.SimpleJson.DeserializeObject(
                        File.ReadAllText(CONFIG_FILE),
                        typeof(List<DNSServer>)));
            bindingSource.DataSource = dns;
        }

        private void LoadNI()
        {
            comboBoxNICList.Items.Clear();

            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in interfaces)
            {
                // 忽略Loopback类型的网卡
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    comboBoxNICList.Items.Add(ni);
                }
            }
        }

        private void comboBoxNICList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ni = CurrentInterface;
            if (ni == null)
            {
                labelNicInfo.Text = string.Empty;
                return;
            }

            var ip = ni.GetIPProperties();
            var builder = new StringBuilder();

            if (ip.UnicastAddresses.Count > 0)
            {
                builder.AppendFormat(
                    "IP:{0}\nMASK:{1}\n",
                    ip.UnicastAddresses[0].Address.MapToIPv4(),
                    ip.UnicastAddresses[0].IPv4Mask);
            }

            if (ip.GatewayAddresses.Count > 0)
            {
                builder.AppendFormat("GAYEWAY:{0}\n", ip.GatewayAddresses[0].Address.MapToIPv4());
            }
            if (ip.DnsAddresses.Count >= 1)
            {
                builder.AppendFormat("DNS1:{0}\n", ip.DnsAddresses[0].MapToIPv4());
            }
            if (ip.DnsAddresses.Count >= 2)
            {
                builder.AppendFormat("DNS2:{0}\n", ip.DnsAddresses[1].MapToIPv4());
            }

            labelNicInfo.Text = builder.ToString();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            var dnsServer = CurrentDNSServer;
            var ni = CurrentInterface;
            if (dnsServer == null)
            {
                return;
            }
            if (ni == null)
            {
                return;
            }

            var wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var moc = wmi.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                //如果没有启用IP设置的网络设备则跳过
                if (ni.Id.Equals(mo["SettingId"]) == false)
                {
                    continue;
                }
                var inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                inPar["DNSServerSearchOrder"] = new[]
                {
                    dnsServer.Server1, dnsServer.Server2
                };
                mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                break;
            }

            wmi.Dispose();

            MessageBox.Show("设置DNS成功");
            LoadNI();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            File.WriteAllText(CONFIG_FILE, SimpleJson.SimpleJson.SerializeObject(dns));
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            foreach (var item in dns)
            {
                item.Test();
            }

            dataGridView1.Refresh();
        }
    }
}