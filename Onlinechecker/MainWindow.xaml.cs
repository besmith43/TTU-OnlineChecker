using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.DirectoryServices;

namespace Onlinechecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport( "kernel32.dll" )]   // needed for console.writeline
        static extern bool AttachConsole( int dwProcessId );
        private const int ATTACH_PARENT_PROCESS = -1;

        public Options cmdFlags;

        public ObservableCollection<Host> hostnames;

        public DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            AttachConsole( ATTACH_PARENT_PROCESS );
            
            String[] args = App.Args;

            hostnames = new();

            cmdParser cmdP = new(args);

            cmdFlags = cmdP.Parse();  

            if (cmdFlags.version)
            {
                Console.WriteLine("OnlineChecker Version 0.1.0");
                return;
            }

            if (cmdFlags.help)
            {
                Console.WriteLine(cmdFlags.helpText);
                return;
            }

            Console.WriteLine("Starting Main Window");

            InitializeComponent();     

            if (cmdFlags.inputfile != null)
            {
                readInputFile(cmdFlags.inputfile, "Manual");
            }
            else
            {
                // this needs to look at the db folder and read in contents from all text files there
                //readFile($"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\db\\default.txt");

                string[] defaultFiles = Directory.GetFiles($"{ System.AppContext.BaseDirectory }\\db");
                
                foreach (var defaultFile in defaultFiles)
                {
                    readDBFile(defaultFile);
                }
            }

            DG1.DataContext = hostnames;

            foreach(var host in hostnames)
            {
                host.TestOnline();
            }
            
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(timerTest);
            dispatcherTimer.Interval = new TimeSpan(0,0,10);
            dispatcherTimer.Start();
        }

        private void onExit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // delete all files in the db folder
            string[] oldDBFiles = Directory.GetFiles($"{ System.AppContext.BaseDirectory }\\db");

            foreach(var file in oldDBFiles)
            {
                File.Delete(file);
            }

            // write all the contents of the hostnames variable to new db files

            saveDB();
        }

        // helper functions

        private void timerTest(object sender, EventArgs e)
        {
            Console.WriteLine("Starting timed Test");

            for(int i = 0; i < hostnames.Count; i++)
            {
                Console.WriteLine($"Testing { hostnames[i].Name }");
                hostnames[i].TestOnline();
                Console.WriteLine($"Finished testing { hostnames[i].Name } \nResult: { hostnames[i].Status }");
            }
        }

        private void readInputFile(string filename, string origin)
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

        private void readDBFile(string filename)
        {
            if (File.Exists(filename))
            {
                string[] lines = System.IO.File.ReadAllLines(filename);

                string origin = lines[0];

                Console.WriteLine($"Origin: { origin }");

                Console.WriteLine("Contents of database file");

                for(int i = 1; i < lines.Length; i++)
                {
                    Console.WriteLine($"{ lines[i] }");
                    hostnames.Add(new Host(lines[i], origin));
                }
            }
        }

        private void writeFile(string filename)
        {
            if (File.Exists(filename))
            {
                // read in the first line to get the origin string
                string[] lines = System.IO.File.ReadAllLines(filename);

                string fileOrigin = lines[0];

                // append to existing file
                using (StreamWriter sw = File.AppendText(filename))
                {
                    foreach(var host in hostnames)
                    {
                        if (fileOrigin == host.Origin)
                        {
                            sw.WriteLine(host.Name);
                        }
                    }
                }
            }
            else
            {
                // get origin based on file name
                int indexOfSlash = filename.LastIndexOf('\\');
                
                string origin = filename.Substring(indexOfSlash+1, filename.Length - 3);

                // create new file
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine(origin);

                    foreach(var host in hostnames)
                    {
                        if (origin == host.Origin)
                        {
                            sw.WriteLine(host.Name);
                        }
                    }
                }
            }
        }

        private void saveDB()
        {
            string dbRoot = $"{ System.AppContext.BaseDirectory }\\db";

            foreach(var host in hostnames)
            {
                string dbFilename = $"{ dbRoot }\\{ host.Origin }.txt";

                if (File.Exists(dbFilename))
                {
                    using (StreamWriter sw = File.AppendText(dbFilename))
                    {
                        sw.WriteLine($"{ host.Name }");
                    }
                }
                else
                {
                    using (StreamWriter sw = File.CreateText(dbFilename))
                    {
                        sw.WriteLine($"{ host.Origin }");
                        sw.WriteLine($"{ host.Name }");
                    }
                }
            }
        }

        // Menu Functions

        private void menuNew_Click(object sender, RoutedEventArgs e)
        {
        	/*NewMsgBox nmb = new("Input a new hostname");

            nmb.Owner = this;

            nmb.ShowDialog();*/

            string inputRead = new InputBox("Insert a new host", "New Host", "Mono-Regular", 24).ShowDialog();

            hostnames.Add(new Host(inputRead, "Manual"));
        }

        private void menuAddOU_Click(object sender, RoutedEventArgs e)
        {

        }

#nullable enable

        private void menuRemove_Click(object sender, RoutedEventArgs e)
        {
            string inputRead = new InputBox("Remove a Host", "Remove Host", "Mono-Regular", 24).ShowDialog();

            Host? tobeRemoved = null;

            foreach(var host in hostnames)
            {
                if (inputRead == host.Name)
                {
                    tobeRemoved = host;
                }
            }

            if (tobeRemoved != null)
            {
                hostnames.Remove(tobeRemoved);
            }
        }

#nullable disable

        private void menuDeleteOU_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
