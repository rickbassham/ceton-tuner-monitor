using System;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;

namespace CetonMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                CetonPerformanceCounterManager.CreatePerformanceCounters();

                if (args.Length == 0)
                {
                    Application.EnableVisualStyles();
                    Application.Run(new ControlForm());
                }
                else
                {
                    GuiConsole.CreateConsole();

                    Console.WriteLine("CetonMonitor - Creates and updates Performance Counters for the Ceton infiniTV 4.");
                    Console.WriteLine("Copyright (C) 2012 Brodrick E. Bassham, Jr.");
                    Console.WriteLine();

                    if (args.FirstOrDefault() == "--debug")
                    {
                        using (CetonPerformanceCounterManager mgr = new CetonPerformanceCounterManager())
                        {
                            mgr.Start();

                            Console.WriteLine("Press any key to stop monitoring...");
                            Console.ReadKey(true);
                        }
                    }
                    else if (args.FirstOrDefault() == "--install")
                    {
                        ApplicationManager.Install();
                    }
                    else if (args.FirstOrDefault() == "--uninstall")
                    {
                        ApplicationManager.Uninstall();
                    }

                    GuiConsole.ReleaseConsole();
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;

                ServicesToRun = new ServiceBase[] { new CetonMonitorService() };

                ServiceBase.Run(ServicesToRun);
            }

        }
    }
}
