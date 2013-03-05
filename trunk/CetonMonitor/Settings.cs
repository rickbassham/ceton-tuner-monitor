using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CetonMonitor
{
    public class Settings
    {
        private static Lazy<Settings> _instance = new Lazy<Settings>(Initialize);

        private Settings()
        {
        }

        private static Settings Initialize()
        {
            Settings s = new Settings();

            s.CetonTuners = new List<string>(ConfigurationManager.AppSettings["tuners"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            return s;
        }

        public void Save()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["tuners"].Value = string.Join(",", this.CetonTuners.Where(t => t.Length > 0));

            config.Save(ConfigurationSaveMode.Modified);
        }

        public List<string> CetonTuners { get; set; }

        public static Settings Default
        {
            get
            {
                return _instance.Value;
            }
        }
    }
}
