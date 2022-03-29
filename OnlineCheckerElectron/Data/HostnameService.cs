using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCheckerElectron.Data
{
    public class HostnameService
    {
        public async Task<List<ComputerHost>> GetTestDataAsync()
        {
            return await Task.FromResult(new List<ComputerHost>
            {
                new ComputerHost("www.google.com","TestData"),
                new ComputerHost("www.stuff.co.nz","TestData"),
                new ComputerHost("besmith.synology.me","TestData"),
                new ComputerHost("besmith.tech","TestData"),
                new ComputerHost("hauntheribarelyknowher.com","TestData"),
                new ComputerHost("badname.com","TestData")
            });
        }
    }
}