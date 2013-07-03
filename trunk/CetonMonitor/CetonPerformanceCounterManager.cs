using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

namespace CetonMonitor
{
    // http://experts.windows.com/frms/windows_entertainment_and_connected_home/f/115/p/103010/551875.aspx

    public class CetonPerformanceCounterManager : IDisposable
    {
        private class CounterDescription
        {
            public string NameFormat { get; private set; }
            public string Description { get; private set; }
            public PerformanceCounterType Type { get; private set; }
            public long InitialValue { get; private set; }

            public static CounterDescription Temperature
            {
                get
                {
                    return new CounterDescription
                    {
                        NameFormat = "Tuner {0} Temperature",
                        Description = "The temperature of the tuner in Celcius",
                        Type = PerformanceCounterType.RawFraction,
                        InitialValue = 0,
                    };
                }
            }

            public static CounterDescription TemperatureBase
            {
                get
                {
                    return new CounterDescription
                    {
                        NameFormat = "Tuner {0} Temperature Base",
                        Description = "The temperature of the tuner in Celcius",
                        Type = PerformanceCounterType.RawBase,
                        InitialValue = 1000,
                    };
                }
            }
        }

        private const string CATEGORY = "Ceton infiniTV 4";

        private Timer _timer;

        private Dictionary<string, List<PerformanceCounter>> _counters;

        private List<Ceton.CetonInfiniTV4> _mgrs;

        public CetonPerformanceCounterManager()
        {
            _mgrs = new List<Ceton.CetonInfiniTV4>();

            foreach (var ip in Settings.Default.CetonTuners)
            {
                _mgrs.Add(new Ceton.CetonInfiniTV4(ip, Ceton.InfiniTV4TunerItems.Temperature));
            }
        }

        public void Start()
        {
            InitializePerformanceCounters();

            StartTimer();
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        public static void CreatePerformanceCounters()
        {
            DeletePerformanceCounters();

            var counterCreationData = new CounterCreationDataCollection();

            for (var i = 1; i < 5; i++)
            {
                counterCreationData.AddRange(GetCounterCreationData(i).ToArray());
            }

            PerformanceCounterCategory.Create(CATEGORY, CATEGORY, PerformanceCounterCategoryType.MultiInstance, counterCreationData);
        }

        public static void DeletePerformanceCounters()
        {
            if (PerformanceCounterCategory.Exists(CATEGORY))
            {
                PerformanceCounterCategory.Delete(CATEGORY);
            }
        }

        private static List<CounterCreationData> GetCounterCreationData(int tuner)
        {
            var counterCreationData = new List<CounterCreationData>();

            var tunerTemp = new CounterCreationData
            {
                CounterName = string.Format(CounterDescription.Temperature.NameFormat, tuner),
                CounterHelp = CounterDescription.Temperature.Description,
                CounterType = CounterDescription.Temperature.Type
            };
            counterCreationData.Add(tunerTemp);

            var tunerTempBase = new CounterCreationData
            {
                CounterName = string.Format(CounterDescription.TemperatureBase.NameFormat, tuner),
                CounterHelp = CounterDescription.TemperatureBase.Description,
                CounterType = CounterDescription.TemperatureBase.Type
            };
            counterCreationData.Add(tunerTempBase);

            return counterCreationData;
        }

        private void InitializePerformanceCounters()
        {
            _counters = new Dictionary<string, List<PerformanceCounter>>();

            foreach (var ip in Settings.Default.CetonTuners)
            {
                List<PerformanceCounter> counters = new List<PerformanceCounter>();

                for (var i = 1; i < 5; i++)
                {
                    counters.Add(CreateCounter(ip, i, CounterDescription.Temperature));
                    counters.Add(CreateCounter(ip, i, CounterDescription.TemperatureBase));
                }

                _counters.Add(ip, counters);
            }
        }

        private PerformanceCounter CreateCounter(string ip, int tuner, CounterDescription counterDesc)
        {
            PerformanceCounter counter = new PerformanceCounter(CATEGORY, string.Format(counterDesc.NameFormat, tuner), ip, false);

            counter.RawValue = counterDesc.InitialValue;

            return counter;
        }

        private void StartTimer()
        {
            _timer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);

            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);

            _timer.Start();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateCounters();
        }

        private void UpdateCounters()
        {
            Parallel.ForEach(_mgrs, mgr =>
            {
                mgr.Update();

                var stats = mgr.TunerStats;

                foreach (var stat in stats)
                {
                    _counters[mgr.Hostname].Find(pc => pc.CounterName == string.Format(CounterDescription.Temperature.NameFormat, stat.TunerIndex + 1)).RawValue = (long)stat.Temperature;
                }
            });
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }

                if (_counters != null)
                {
                    foreach (var item in _counters.Keys)
                    {
                        foreach (var counter in _counters[item])
                        {
                            counter.Dispose();
                        }
                    }

                    _counters = null;
                }
            }
        }

        #endregion
    }
}
