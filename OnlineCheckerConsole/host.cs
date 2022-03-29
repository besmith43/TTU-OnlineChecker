using System;
using System.ComponentModel;
using System.Net.NetworkInformation;

namespace OnlineCheckerConsole
{
    public class Host
    {
        public string Hostname { get; init; }
        public status Status { get; set; }
        public string Address { get; set; }
        public string Origin { get; init; }

        public Host(string hostname)
        {
            Hostname = hostname;
            Status = status.offline;
            Address = "N/A";
            Origin = "Manual";
        }

        public Host(string hostname, string origin)
        {
            Hostname = hostname;
            Status = status.offline;
            Address = "N/A";
            Origin = origin;
        }

        public void TestOnline()
        {
            Status = status.testing;

            Ping test = new();

            try
            {
                PingReply reply = test.Send(Hostname);

                if (reply.Status == IPStatus.Success)
                {
                    Status = status.online;
                    Address = reply.Address.ToString();
                }
                else
                {
                    Status = status.offline;
                }
            }
            catch
            {
                Status = status.offline;
            }
        }
    }
}