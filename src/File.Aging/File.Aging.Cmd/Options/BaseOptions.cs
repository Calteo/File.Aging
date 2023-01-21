using System.ComponentModel;
using Toolbox.CommandLine;

namespace File.Aging.Cmd.Options
{
    internal class BaseOptions
    {
        [Option("folder"), Mandatory, Position(0), Description("Folder to act on")]
        public string Folder { get; set; } = "";
    }
}
