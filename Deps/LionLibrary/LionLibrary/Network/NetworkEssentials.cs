using System;
using System.Net;
using System.Net.Sockets;

namespace LionLibrary.Network
{
    public static class NetworkEssentials
    {
        public static string LocalIPv4Address
        {
            get
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
        }

        public static bool IsConnected(Socket s) => !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
    }
}
