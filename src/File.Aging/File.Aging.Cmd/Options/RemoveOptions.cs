using System.ComponentModel;
using Toolbox.CommandLine;

namespace File.Aging.Cmd.Options
{
    [Verb("remove"), Description("Remove rules from a folder")]
    internal class RemoveOptions : BaseOptions
    {
        [Option("position"), Mandatory, Position(1), Description("Postions to remove")]
        public int[]? Postions { get; set; }
    }
}
