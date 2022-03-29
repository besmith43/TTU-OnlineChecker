#nullable enable

namespace Onlinechecker
{
    public class Options
    {
        public bool help { get; set; }
        public string helpText { get; set; }
        public bool version { get; set; }
        public bool verbose { get; set; }
        public string? inputfile { get; set; }

        public Options()
        {
            help = false;
            version = false;
            verbose = false;
            inputfile = null;
            helpText = "Online Checker Help Splash Screen";
        }
    }
}