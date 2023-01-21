using System.Reflection;
using File.Aging.Cmd.Options;
using Toolbox.CommandLine;

namespace File.Aging.Cmd
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var rc = 0;
            try
            {
                var parser = new Parser(
                                    typeof(AddOptions), 
                                    typeof(ListOptions), 
                                    typeof(RemoveOptions), 
                                    typeof(ClearOptions), 
                                    typeof(RunOptions)
                                    );

                var result = parser.Parse(args);
                rc = result
                            .OnError(OnError)
                            .OnHelp(OnHelp)
                            .On<AddOptions>(OnAdd)
                            .On<ListOptions>(OnList)
                            .On<RemoveOptions>(OnRemove)
                            .On<ClearOptions>(OnClear)
                            .On<RunOptions>(OnRun)
                            .Return;
            }
            catch (Exception exception)
            {
                while (exception is TargetInvocationException targetInvocationException && targetInvocationException.InnerException!=null)
                    exception = targetInvocationException.InnerException;

                Console.WriteLine($"{exception.GetType().Name} - {exception.Message}");
                rc = 2;
            }

            return rc;
        }

        private static int OnError(ParseResult result)
        {
            Console.WriteLine(result.Text);
            return 1;
        }

        private static int OnHelp(ParseResult result)
        {
            Console.WriteLine(result.GetHelpText(Console.WindowWidth));
            return 0;
        }

        private static int OnAdd(AddOptions options)
        {
            var config = AgingConfig.Load(options.Folder);

            var rule = new AgingRule 
            { 
                Pattern = options.Pattern,
                Expire = options.Expire,
                Keep = options.Keep
            };

            config.Rules.Insert(options.Position, rule);            
            config.Save();

            Console.WriteLine($"{config.Directory.FullName}: rule '{rule.Pattern}' add at position {options.Position}.");

            return 0;
        }

        private static int OnList(ListOptions options)
        {
            var config = AgingConfig.Load(options.Folder);

            var formatted = new FormattedCollection
            {
                { "#", FormattedAlignment.Right },
                { "Pattern", FormattedAlignment.Left },
                { "Expire", FormattedAlignment.Left },
                { "Keep", FormattedAlignment.Left }
            };

            if (options.NoParent)
            {
                if (!config.Exists)
                {
                    Console.WriteLine($"{config.Directory.FullName}: Configuration does not exist.");
                    return 1;
                }

                Console.WriteLine($"Exp/Keep: {config.Expire?.ToString() ?? "<not set>"} {config.Keep?.ToString() ?? "<not set>"}");

                var i = 0;
                foreach (var rule in config.Rules)
                {
                    formatted.Add(GetFormattedValues(i++, rule, options).ToArray());                        
                }
            }
            else
            {
                if (!config.EffectiveExists)
                {
                    Console.WriteLine($"{config.Directory.FullName}: Configuration does not exist.");
                    return 1;
                }

                Console.WriteLine($"Exp/Keep: {config.EffectiveExpire} {config.EffectiveKeep}");

                formatted.Add("From", FormattedAlignment.Left);

                var i = 0;
                foreach (var rule in config.EffectiveRules)
                {
                    var values = GetFormattedValues(i++, rule, options);
                    if (rule.Config != config)
                        values.Add(rule.Config?.Directory.FullName ?? "");
                    else
                        values.Add("");

                    formatted.Add(values.ToArray());
                }
            }

            Console.WriteLine(formatted.ToFormatted());

            return 0;
        }

        private static List<string> GetFormattedValues(int index, AgingRule rule, ListOptions options)
        {
            var values = new List<string>
            {
                index.ToString(),
                rule.Pattern
            };

            if (options.NoParent)
            {
                values.Add(rule.Expire?.ToString() ?? "<not set>");
                values.Add(rule.Keep.ToString() ?? "<not set>");
            }
            else
            {
                values.Add(rule.EffectiveExpire.ToString());
                values.Add(rule.EffectiveKeep.ToString());
            }

            return values;
        }

        private static int OnRemove(RemoveOptions options)
        {
            var config = AgingConfig.Load(options.Folder);

            if (config.Exists)
            {
                var rules = options.Postions?.Select(i =>
                {
                    if (i < 0 || i >= config.Rules.Count)
                        throw new InvalidOperationException($"Rule #{i} does not exist.");

                    return config.Rules[i];
                }).ToList() ?? new List<AgingRule>();

                rules.ForEach(r => config.Rules.Remove(r));

                config.Save();

                Console.WriteLine($"{config.Directory.FullName}: removed rules - {string.Join(", ", rules.Select(r => $"'{r.Pattern}'"))}");
                return 0;
            }
            else
            {
                Console.WriteLine($"{config.Directory.FullName}: Configuration does not exist.");
                return 1;
            }
        }

        private static int OnClear(ClearOptions options)
        {
            var config = AgingConfig.Load(options.Folder);

            if (config.Exists)
            {
                if (options.Level.HasFlag(ClearLevel.Log))
                {
                    Console.WriteLine($"{config.Directory.FullName}: cleared log");
                }
                if (options.Level.HasFlag(ClearLevel.Archive))
                {
                    Console.WriteLine($"{config.Directory.FullName}: cleared archive");
                }
                if (options.Level.HasFlag(ClearLevel.Rules))
                {
                    config.Rules.Clear();
                    config.Save();

                    Console.WriteLine($"{config.Directory.FullName}: cleared rules");
                }
                if (options.Level.HasFlag(ClearLevel.All))
                {
                    config.Delete();

                    Console.WriteLine($"{config.Directory.FullName}: cleared all");
                }

                return 0;
            }
            else
            {
                Console.WriteLine($"{config.Directory.FullName}: Configuration does not exist.");
                return 1;
            }            
        }

        private static int OnRun(RunOptions options)
        {
            throw new NotImplementedException();
        }
    }
}