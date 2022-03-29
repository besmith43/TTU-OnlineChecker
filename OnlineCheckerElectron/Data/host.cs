using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;

namespace OnlineCheckerElectron.Data
{
    public class ComputerHost : INotifyPropertyChanged
    {
        private string _hostname;
        private status _status;
        public string _statusCSS = "";
        private string _address;
        private string _origin;

        public event PropertyChangedEventHandler PropertyChanged;

        public ComputerHost(string hostname)
        {
            _hostname = hostname;
            _status = status.offline;
            _address = "N/A";
            _origin = "Manual";
        }

        public ComputerHost(string hostname, string origin)
        {
            _hostname = hostname;
            _status = status.offline;
            _address = "N/A";
            _origin = origin;
        }

        public string Hostname
        {
            get { return _hostname; }
        }

        public status Status
        {
            get { return _status; }
            set
            {
                _status = value;

                if (_status == status.offline)
                {
                    _statusCSS = "background-color: red";
                }
                else if (_status == status.testing)
                {
                    _statusCSS = "background-color: grey";
                }
                else if (_status == status.online)
                {
                    _statusCSS = "background-color: green";
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Address"));
            }
        }

        public string Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Origin"));
            }
        }

        public void TestOnline()
        {
            Console.WriteLine($"{ Hostname }: starting TestOnline");
            Status = status.testing;

            Console.WriteLine($"{ Hostname }: creating Ping");

            Ping test = new();
            
            Console.WriteLine($"{ Hostname }: adding event handler");

            test.PingCompleted += new PingCompletedEventHandler(PingCompleted);

            try
            {
                Console.WriteLine($"{ Hostname }: starting send async");
                test.SendAsync(Hostname, null);
            }
            catch
            {
                Console.WriteLine("bad DNS entry");
                Status = status.offline;
            }
        }

        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            Console.WriteLine($"{ Hostname }: starting PingCompleted");
            PingReply reply = e.Reply;

            try
            {
                Console.WriteLine($"{ Hostname }: PingReply { reply.Status }");

                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine($"{ Hostname }: Setting Status to Success");

                    Status = status.online;

                    Console.WriteLine($"{ Hostname }: Setting Address to { reply.Address.ToString() }");

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
                Console.WriteLine($"reply failed on { Hostname }");
            }
        }
    }
}