using System;

namespace OnlineCheckerConsole
{
    public class cmdParser
    {
        private string[] _args;
        private Options opts;

        public cmdParser(string[] Args)
        {
            _args = Args;
        }

        public Options Parse()
        {
            opts = new();

            if (_args != null)
            {
                for (int i = 0; i < _args.Length; i++)
                {
                    if (_args[i] == "-h" || _args[i] == "--help")
                    {
                        opts.help = true;
                    }
                    else if (_args[i] == "-i" || _args[i] == "--input-file")
                    {
                        opts.inputfile = _args[i + 1];
                    }
                    else if (_args[i] == "-l" || _args[i] == "--ldap")
                    {
                        opts.ldapstring = _args[i + 1];
                    }
                    else if (_args[i] == "-V" || _args[i] == "--version")
                    {
                        opts.version = true;
                    }
                    else if (_args[i] == "-v" || _args[i] == "--verbose")
                    {
                        opts.verbose = true;
                    }
                    else
                    {
                        // place any unnamed or positional value checks
                    }
                }
            }

            return opts;
        }
    }
}