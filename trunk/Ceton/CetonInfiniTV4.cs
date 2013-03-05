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
    }

    [Flags]
    public enum InfiniTV4TunerItems : uint
    {
        Temperature = 1,
        SignalLevel = 2,
        SignalSNR = 4,
        Frequecy = 8,
        ChannelNumber = 16,

        All = Temperature | SignalLevel | SignalSNR | Frequecy | ChannelNumber,
    }

    public class CetonInfiniTV4
    {
        private class CetonResponse
        {
            public string result { get; set; }
        }

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

            return stats;
        }

        private decimal? ParseDecimal(CetonResponse response, string suffix)
        {
            if (response.result.EndsWith(suffix))
            {
                decimal d = 0.0M;

                if (decimal.TryParse(response.result.Substring(0, response.result.Length - suffix.Length), out d))
                {
                    return d;
                }
            }

            return null;
        }

        private CetonResponse GetValue(string hostname, int tuner, string s, string counter)
        {
            using (WebClient client = new WebClient())
            {
                Uri uri = new Uri(string.Format(URL_FORMAT, hostname, tuner, s, counter));

                string raw = client.DownloadString(uri);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<CetonResponse>(raw);
            }
        }
    }
}
