using CommandLine;

namespace AmbientOTron
{
    public class CommandLineOptions
    {
        [Option("DebugMef", DefaultValue = false, HelpText="Opens a mef dependency debug window instead of the main window", Required = false)]
        public bool DebugMef { get; set; }
    }
}