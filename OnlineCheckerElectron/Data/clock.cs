using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCheckerElectron.Data
{
    public class Clock
    {
        public List<ComputerHost> hostnames;

        public Clock(List<ComputerHost> hostNames)
        {
            hostnames = hostNames;
        }
        
        public async void Tick()
        {
            while(true)
            {
                Console.WriteLine("Starting new Tick Loop");
                PingAll();
                await Task.Delay(60000); // should be 60 seconds
            }
        }

        private void PingAll()
        {
            foreach(var host in hostnames)
            {
                host.TestOnline();
            }
        }
    }
}