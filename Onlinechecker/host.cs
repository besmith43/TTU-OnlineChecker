using System;
using System.ComponentModel;
using System.Net.NetworkInformation;

namespace Onlinechecker
{
    public class Host : INotifyPropertyChanged
    {
        private string _hostname;
        private status _status;
        private string _address;
        private string _origin;

        public Host(string hostname)
        {
            _hostname = hostname;
            _status = status.offline;
            _address = "N/A";
            _origin = "Manual";
        }

        public Host(string hostname, string origin)
        {
            _hostname = hostname;
            _status = status.offline;
            _address = "N/A";
            _origin = origin;
        }

        public string Name
        {
            get { return _hostname; }
        }

        public status Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
            }
        }

        public string Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;
                OnPropertyChanged("Origin");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public void TestOnline()
        {
            _status = status.testing;
            OnPropertyChanged("Status");

            Ping test = new();
            
            test.PingCompleted += new PingCompletedEventHandler(PingCompleted);

            try
            {
                test.SendAsync(_hostname, null);
            }
            catch
            {
                Console.WriteLine("bad DNS entry");
                _status = status.offline;
                OnPropertyChanged("Status");
            }
        }

        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            PingReply reply = e.Reply;

            if (reply.Status == IPStatus.Success)
            {
                _status = status.online;
                OnPropertyChanged("Status");

                _address = reply.Address.ToString();
                OnPropertyChanged("Address");
            }
            else
            {
                _status = status.offline;
                OnPropertyChanged("Status");
            }
        }
    }
}