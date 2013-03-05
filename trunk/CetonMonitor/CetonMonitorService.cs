using System.ServiceProcess;

namespace CetonMonitor
{
    partial class CetonMonitorService : ServiceBase
    {
        CetonPerformanceCounterManager _mgr;

        public CetonMonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _mgr = new CetonPerformanceCounterManager();

            _mgr.Start();
        }

        protected override void OnStop()
        {
            _mgr.Stop();
            _mgr.Dispose();
        }
    }
}
