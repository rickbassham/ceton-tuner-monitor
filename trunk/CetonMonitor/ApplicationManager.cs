using System.Collections;
using System.Configuration.Install;

namespace CetonMonitor
{
    internal class ApplicationManager
    {
        public static void Install()
        {
            AssemblyInstaller installer = new AssemblyInstaller(typeof(CetonMonitorService).Assembly, null);
            installer.UseNewContext = true;

            IDictionary state = new Hashtable();

            installer.Install(state);
            installer.Commit(state);

            CetonPerformanceCounterManager.CreatePerformanceCounters();
        }

        public static void Uninstall()
        {
            AssemblyInstaller installer = new AssemblyInstaller(typeof(CetonMonitorService).Assembly, null);
            installer.UseNewContext = true;

            IDictionary state = new Hashtable();

            installer.Uninstall(state);

            CetonPerformanceCounterManager.DeletePerformanceCounters();
        }
    }
}
