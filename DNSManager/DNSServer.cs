using System.Net.NetworkInformation;
using System.Text;

namespace DNSManager
{
    public class DNSServer
    {
        private readonly Ping ping = new Ping();
        public string Name { get; set; }

        public string Server1 { get; set; }

        public string Server2 { get; set; }

        public string Speed { get; set; }

        public void Test()
        {
            var builder = new StringBuilder();
            if (string.IsNullOrEmpty(Server1) == false)
            {
                var reply = ping.Send(Server1);
                builder.AppendFormat("dns1:{0}ms ", reply?.RoundtripTime);
            }
            if (string.IsNullOrEmpty(Server2) == false)
            {
                var reply = ping.Send(Server2);
                builder.AppendFormat("dns2:{0}ms ", reply?.RoundtripTime);
            }

            Speed = builder.ToString();
        }
    }
}