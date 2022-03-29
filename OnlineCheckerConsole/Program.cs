using System;
using System.IO;
using System.Collections.Generic;
using System.DirectoryServices;
using ConsoleTables;

namespace OnlineCheckerConsole
{
    class Program
    {
        public static Options cmdFlags;
        public static List<Host> hostnames;

        static void Main(string[] args)
        {
            cmdParser cmdP = new(args);

            cmdFlags = cmdP.Parse();

            if (cmdFlags.version)
            {
                Console.WriteLine("Online Checker Console Version 0.1.0");
                return;
            }
            else if (cmdFlags.help)
            {
                Console.WriteLine(cmdFlags.helpText);
                return;
            }
            else
            {
                Run();
            }
        }

        public static void Run()
        {
            hostnames = new();

            if (cmdFlags.ldapstring != null)
            {
                RunLDAP();
                return;
            }

            if (cmdFlags.inputfile != null)
            {
                readInputFile(cmdFlags.inputfile, "Text File");
            }
            else
            {
                GetInput();
            }

            PingHosts();

            PrintTable();
        }

        private static void readInputFile(string filename, string origin)
        {
            if (File.Exists(filename))
            {
                string[] lines = System.IO.File.ReadAllLines(filename);

                Console.WriteLine("Contents of input file");

                foreach (var line in lines)
                {
                    Console.WriteLine($"{ line }");
                    hostnames.Add(new Host(line, origin));
                }
            }
            else
            {
                Console.WriteLine("Given File not found");
            }
        }

        private static void GetInput()
        {
            bool Continue = true;

            while (Continue)
            {
                Console.WriteLine("Would you like to input a hostname manually? (Y/n)");
                string userInput = Console.ReadLine();

                if (userInput != "n")
                {
                    Console.WriteLine("What hostname would you like to add?");
                    string newHost = Console.ReadLine();

                    hostnames.Add(new Host(newHost));
                }
                else
                {
                    Continue = false;
                }
            }
        }

        private static void RunLDAP()
        {
            using (DirectoryEntry entry = new DirectoryEntry($"LDAP://{ cmdFlags.ldapstring }"))
            {
                using (DirectorySearcher mySearcher = new DirectorySearcher(entry))
                {
                    mySearcher.Filter = ("(objectClass=computer)");
                    mySearcher.SizeLimit = 0;
                    mySearcher.PageSize = 250;
                    mySearcher.PropertiesToLoad.Add("name");

                    foreach(SearchResult resEnt in mySearcher.FindAll())
                    {
                        if (resEnt.Properties["name"].Count > 0)
                        {
                            string computerName = (string)resEnt.Properties["name"][0];
                            hostnames.Add(new Host(computerName, "LDAP"));
                        }
                    }
                }
            }

            PingHosts();
            PrintTable();
        }

        private static void PingHosts()
        {
            if (hostnames.Count > 0)
            {
                foreach(var host in hostnames)
                {
                    host.TestOnline();
                }
            }
        }

        private static void PrintTable()
        {
            var table = new ConsoleTable("Hostname", "Status", "Address", "Origin");
                
            foreach(var host in hostnames)
            {
                table.AddRow(host.Hostname, host.Status, host.Address, host.Origin);
            }

            table.Write();
        }
    }
}
