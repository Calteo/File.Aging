using System.ComponentModel;
using Toolbox.CommandLine;

namespace File.Aging.Cmd.Options
{
    [Verb("clear"), Description("Clear the file aging of a folder")]
    internal class ClearOptions : BaseOptions
    {
        [Option("level"), Mandatory, Position(1), Description("Level of deletion.")]
        public ClearLevel Level { get; set; }
    }

    [Flags]
    internal enum ClearLevel 
    {
        [Description("Aging rules")]
        Rules = 0x01,
        [Description("Archived files")]
        Archive = 0x02,
        [Description("Log files")]
        Log = 0x04,
        [Description("Complete configuration")]
        All = 0x0F
    }
}
