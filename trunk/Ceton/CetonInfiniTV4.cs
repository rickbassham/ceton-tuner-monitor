using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Ceton
{
    public class CetonInfiniTV4TunerStats
    {
        public int TunerIndex { get; set; }

        public decimal Temperature { get; set; }
        public decimal? SignalLevel { get; set; }
        public decimal? SignalSNR { get; set; }
        public int? Frequency { get; set; }
        public int? ChannelNumber { get; set; }

        public string TransportState { get; set; }
        public string Modulation { get; set; }
        public string CopyProtectionStatus { get; set; }
    }

    [Flags]
    public enum InfiniTV4TunerItems : uint
    {
        Temperature = 1,
        SignalLevel = 2,
        SignalSNR = 4,
        Frequecy = 8,
        ChannelNumber = 16,
        TransportState = 32,
        Modulation = 64,
        CopyProtectionStatus = 128,

        All = Temperature | SignalLevel | SignalSNR | Frequecy | ChannelNumber | TransportState | Modulation | CopyProtectionStatus,
    }

    public class CetonInfiniTV4
    {
        private const string URL_FORMAT = "http://{0}/get_var.json?i={1}&s={2}&v={3}";

        private string _hostname;
        private InfiniTV4TunerItems _items;

        public CetonInfiniTV4(string host)
            : this(host, InfiniTV4TunerItems.All)
        {
        }

        public CetonInfiniTV4(string host, InfiniTV4TunerItems items)
        {
            _hostname = host;
            _items = items;
        }

        public List<CetonInfiniTV4TunerStats> TunerStats { get; set; }

        public List<CetonInfiniTV4TunerStats> Update()
        {
            Task<CetonInfiniTV4TunerStats>[] tasks = new Task<CetonInfiniTV4TunerStats>[] {
                Task<CetonInfiniTV4TunerStats>.Factory.StartNew(GetStats, 0),
                Task<CetonInfiniTV4TunerStats>.Factory.StartNew(GetStats, 1),
                Task<CetonInfiniTV4TunerStats>.Factory.StartNew(GetStats, 2),
                Task<CetonInfiniTV4TunerStats>.Factory.StartNew(GetStats, 3),
            };

            TunerStats = tasks.Select(x => x.Result).OrderBy(x => x.TunerIndex).ToList();

            return TunerStats;
        }

        private CetonInfiniTV4TunerStats GetStats(object o)
        {
            CetonInfiniTV4TunerStats stats = new CetonInfiniTV4TunerStats
            {
                TunerIndex = (int)o
            };

            decimal? val = null;

            if (_items.HasFlag(InfiniTV4TunerItems.Temperature))
            {
                val = ParseDecimal(GetValue(_hostname, stats.TunerIndex, "diag", "Temperature"), " C");
                stats.Temperature = Convert.ToInt32(val.Value);
            }

            if (_items.HasFlag(InfiniTV4TunerItems.SignalLevel))
            {
                val = ParseDecimal(GetValue(_hostname, stats.TunerIndex, "diag", "Signal_Level"), " dBmV");
                stats.SignalLevel = val;
            }

            if (_items.HasFlag(InfiniTV4TunerItems.SignalSNR))
            {
                val = ParseDecimal(GetValue(_hostname, stats.TunerIndex, "diag", "Signal_SNR"), " dB");
                stats.SignalSNR = val;
            }

            if (_items.HasFlag(InfiniTV4TunerItems.Frequecy))
            {
                val = ParseDecimal(GetValue(_hostname, stats.TunerIndex, "tuner", "Frequency"), "");
                stats.Frequency = (int?)val;
            }

            if (_items.HasFlag(InfiniTV4TunerItems.ChannelNumber))
            {
                val = ParseDecimal(GetValue(_hostname, stats.TunerIndex, "cas", "VirtualChannelNumber"), "");
                stats.ChannelNumber = (int?)val;
            }

            if (_items.HasFlag(InfiniTV4TunerItems.TransportState))
            {
                stats.TransportState = GetValue(_hostname, stats.TunerIndex, "av", "TransportState");
            }

            if (_items.HasFlag(InfiniTV4TunerItems.Modulation))
            {
                stats.Modulation = GetValue(_hostname, stats.TunerIndex, "tuner", "Modulation");
            }

            if (_items.HasFlag(InfiniTV4TunerItems.CopyProtectionStatus))
            {
                stats.CopyProtectionStatus = GetValue(_hostname, stats.TunerIndex, "diag", "CopyProtectionStatus");
            }

            return stats;
        }

        private decimal? ParseDecimal(string response, string suffix)
        {
            try
            {
                if (response.EndsWith(suffix))
                {
                    decimal d = 0.0M;

                    if (decimal.TryParse(response.Substring(0, response.Length - suffix.Length), out d))
                    {
                        return d;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing decimal: {0} {1}", response, suffix);
                Console.WriteLine(ex);
            }

            return null;
        }

        private string GetValue(string hostname, int tuner, string s, string counter)
        {
            string response = "";

            try
            {
                using (WebClient client = new WebClient())
                {
                    Uri uri = new Uri(string.Format(URL_FORMAT, hostname, tuner, s, counter));

                    string raw = client.DownloadString(uri);

                    response = raw.Replace("{ \"result\": \"", "").Replace("\" }", "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting value: {0} {1} {2} {3}", hostname, tuner, s, counter);
                Console.WriteLine(ex);
            }

            return response;
        }

        public string Hostname
        {
            get
            {
                return _hostname;
            }
        }
    }
}
