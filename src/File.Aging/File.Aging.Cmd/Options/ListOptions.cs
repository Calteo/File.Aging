using System.ComponentModel;
using Toolbox.CommandLine;

namespace File.Aging.Cmd.Options
{
    [Verb("list")]
    [Description("list options of a aging folder")]
    internal class ListOptions : BaseOptions
    {
        [Option("noparent"), Description("do not include parent folder configuration.")]
        public bool NoParent { get; set; }
    }
}
