using System.ComponentModel;
using Toolbox.CommandLine;

namespace File.Aging.Cmd.Options
{
    [Verb("add"), Description("Add a rule to a folder")]
    internal class AddOptions : BaseOptions
    {
        [Option("pattern"), Mandatory, Position(1), Description("Pattern to match files")]
        public string Pattern { get; set; } = "";

        [Option("at"), Position(2), Description("Position to insert the rule at"), DefaultValue(0)]
        public int Position { get; set; }

        [Option("expire"), Position(3), Description("Timespan after what files expire.")]
        public TimeSpan? Expire { get; set; }

        [Option("keep"), Position(4), Description("Timespan how long files are kept after they expire.")]
        public TimeSpan? Keep { get; set; }        
    }
}
