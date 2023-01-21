using System.Text.RegularExpressions;
using Toolbox.Xml.Serialization;

namespace File.Aging
{
    public class AgingRule
    {
        [NotSerialized]
        public AgingConfig? Config { get; set; }

        #region Pattern
        private string pattern = "*";
        public string Pattern
        {
            get => pattern;
            set
            {
                MatchingPattern = new Regex(ToRegex(value));
                pattern = value;
            }
        }
        #endregion

        private Regex MatchingPattern { get; set; } = new Regex(".*");
        private string ToRegex(string pattern)
        {
            // replace '.' before inserting other '.'
            pattern = pattern.Replace(".", @"\.").Replace("*", ".*");

            foreach (var c in "<>()[]?")
            {
                pattern = pattern.Replace(c.ToString(), $@"\{c}");
            }
            return pattern;
        }

        #region Expire
        public TimeSpan? Expire { get; set; }
        public TimeSpan EffectiveExpire
        {
            get
            {
                return Expire ?? Config?.EffectiveExpire ?? AgingConfig.DefaultExpire;
            }
        }
        #endregion
        #region Keep
        public TimeSpan? Keep { get; set; }
        public TimeSpan EffectiveKeep
        {
            get
            {
                return Keep ?? Config?.EffectiveKeep ?? AgingConfig.DefaultKeep;
            }
        }
        #endregion
    }
}