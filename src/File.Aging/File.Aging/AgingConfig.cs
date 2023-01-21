using System.Runtime.Serialization;
using Toolbox.Xml.Serialization;

namespace File.Aging
{
    public class AgingConfig
    {
        #region Serialization

        public static AgingConfig Load(string folder)
        {
            return Load(new DirectoryInfo(folder));
        }
       
        public static AgingConfig Load(DirectoryInfo directory)
        {
            if (!directory.Exists) throw new DirectoryNotFoundException(directory.FullName);

            var configInfo = GetConfigFileInfo(directory);

            AgingConfig config;
            if (configInfo.Exists)
            {
                using var stream = new FileStream(configInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                config = new XmlFormatter<AgingConfig>().Deserialize(stream);
            }
            else
            {
                config = new AgingConfig();
            }
            
            config.Directory = directory;

            return config;
        }

        private static DirectoryInfo GetConfigDirectoryInfo(DirectoryInfo directory)
            => new(Path.Combine(directory.FullName, ".aging"));

        private static FileInfo GetConfigFileInfo(DirectoryInfo directory)
                    => new(Path.Combine(GetConfigDirectoryInfo(directory).FullName, "Config.xml"));

        public void Save()
        {
            var configInfo = GetConfigFileInfo(Directory);
            var root = configInfo.Directory ?? throw new DirectoryNotFoundException($"No directory for {configInfo.FullName}.");
            if (!root.Exists)
            {
                root.Create();
            }

            using var stream = new FileStream(configInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.None);
            new XmlFormatter<AgingConfig>().Serialize(this, stream);
        }

        [OnDeserialized]
        private void OnDeserialized(Dictionary<string, string> data)
        {
            Exists = true;
            Rules.ForEach(r => r.Config = this);
        }

        #endregion

        public void Delete()
        {
            var configInfo = GetConfigDirectoryInfo(Directory);
            configInfo.Delete(true);
        }

        #region
        [NotSerialized]
        public bool Exists { get; private set; }
        public bool EffectiveExists => Exists || (Parent?.EffectiveExists ?? false);
        #endregion

        #region Expire
        internal static TimeSpan DefaultExpire { get; } = TimeSpan.FromDays(2*365);
        public TimeSpan? Expire { get; set; }
        public TimeSpan EffectiveExpire
        {
            get
            {
                return Expire ?? Parent?.Expire ?? DefaultExpire;
            }
        }
        #endregion
        #region Keep
        internal static TimeSpan DefaultKeep { get; } = TimeSpan.FromDays(365);
        public TimeSpan? Keep { get; set; }
        public TimeSpan EffectiveKeep
        {
            get
            {
                return Keep ?? Parent?.Keep ?? DefaultKeep;
            }
        }
        #endregion
        #region Parent
        public AgingConfig? _parent;
        public AgingConfig? Parent
        {
            get
            {
                if (_parent == null && Directory.Parent!=null)
                {
                    _parent = Load(Directory.Parent);
                }
                return _parent;
            }
        }
        #endregion

        public DirectoryInfo Directory { get; internal set; } = new DirectoryInfo(".");
        public List<AgingRule> Rules { get; internal set; } = new List<AgingRule>();
        public IEnumerable<AgingRule> EffectiveRules => Parent != null 
                                            ? Rules.Concat(Parent.EffectiveRules) 
                                            : Rules;
    }
}