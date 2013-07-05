using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Ceton;

namespace CetonRRD
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToRRDTOOL = @"C:\Users\rbassham.CAPSHER\Desktop\rrdtool-1.4.8\win32\Debug\rrdtool.exe";
            string pathTORRDDB = @"ceton.rrd";

            string ipAddress = args.Length < 1 ? "192.168.200.1" : args[0];
            string filePath = args.Length < 2 ? "ceton.csv" : args[1];

            Ceton.CetonInfiniTV4 mgr = null;
            List<CetonInfiniTV4TunerStats> stats = null;

            var useRealValues = true;

            if (useRealValues)
            {
                mgr = new Ceton.CetonInfiniTV4(ipAddress);
            }
            else
            {
                stats = new List<CetonInfiniTV4TunerStats>()
                {
                    new CetonInfiniTV4TunerStats{ TunerIndex = 0 },
                    new CetonInfiniTV4TunerStats{ TunerIndex = 1 },
                    new CetonInfiniTV4TunerStats{ TunerIndex = 2 },
                    new CetonInfiniTV4TunerStats{ TunerIndex = 3 },
                };
            }

            var rand = new Random();

            while (true)
            {
                string commandFormat = null;

                if (!File.Exists(pathTORRDDB))
                {
                    List<string> guauges = new List<string>()
                    {
                        " DS:temp{0}:GAUGE:120:0:100 ",
                        " DS:level{0}:GAUGE:120:-75:75 ",
                        " DS:snr{0}:GAUGE:120:-100:100 "
                    };

                    commandFormat = @"create {0} --step 60 ";

                    foreach (var guageFormat in guauges)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            commandFormat += string.Format(guageFormat, i);
                        }
                    }

                    commandFormat += " RRA:AVERAGE:0.5:1:1440 RRA:AVERAGE:0.5:60:168 RRA:AVERAGE:0.5:1440:180 ";

                    Execute(pathToRRDTOOL, string.Format(commandFormat, pathTORRDDB));
                }

                if (useRealValues)
                {
                    stats = mgr.Update();
                }
                else
                {
                    foreach (var stat in stats)
                    {
                        stat.Temperature = rand.Next(0, 100);
                        stat.SignalLevel = Scale((decimal)rand.NextDouble(), 0, 1, -75, 75);
                        stat.SignalSNR = Scale((decimal)rand.NextDouble(), 0, 1, -100, 100);
                    }
                }

                commandFormat = @"update {0} N:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}:{12}";

                var tuner0 = stats.Find(x => x.TunerIndex == 0);
                var tuner1 = stats.Find(x => x.TunerIndex == 1);
                var tuner2 = stats.Find(x => x.TunerIndex == 2);
                var tuner3 = stats.Find(x => x.TunerIndex == 3);

                List<string> values = new List<string>();

                values.Add(pathTORRDDB);

                values.Add(tuner0 == null ? "U" : tuner0.Temperature.ToString());
                values.Add(tuner1 == null ? "U" : tuner1.Temperature.ToString());
                values.Add(tuner2 == null ? "U" : tuner2.Temperature.ToString());
                values.Add(tuner3 == null ? "U" : tuner3.Temperature.ToString());

                values.Add(tuner0 == null || tuner0.SignalLevel == null ? "U" : tuner0.SignalLevel.ToString());
                values.Add(tuner1 == null || tuner1.SignalLevel == null ? "U" : tuner1.SignalLevel.ToString());
                values.Add(tuner2 == null || tuner2.SignalLevel == null ? "U" : tuner2.SignalLevel.ToString());
                values.Add(tuner3 == null || tuner3.SignalLevel == null ? "U" : tuner3.SignalLevel.ToString());

                values.Add(tuner0 == null || tuner0.SignalSNR == null ? "U" : tuner0.SignalSNR.ToString());
                values.Add(tuner1 == null || tuner1.SignalSNR == null ? "U" : tuner1.SignalSNR.ToString());
                values.Add(tuner2 == null || tuner2.SignalSNR == null ? "U" : tuner2.SignalSNR.ToString());
                values.Add(tuner3 == null || tuner3.SignalSNR == null ? "U" : tuner3.SignalSNR.ToString());

                Execute(pathToRRDTOOL, string.Format(commandFormat, values.ToArray()));

                commandFormat = @"graph temps.png --end now --start end-3600s DEF:temp0a={0}:temp0:AVERAGE DEF:temp1a={0}:temp1:AVERAGE DEF:temp2a={0}:temp2:AVERAGE DEF:temp3a={0}:temp3:AVERAGE LINE1:temp0a#edc240:""Tuner 1 Temp"" LINE1:temp1a#afd8f8:""Tuner 2 Temp"" LINE1:temp2a#cb4b4b:""Tuner 3 Temp"" LINE1:temp3a#4da74d:""Tuner 4 Temp""";
                string command = string.Format(commandFormat, pathTORRDDB);
                Execute(pathToRRDTOOL, command);


                commandFormat = @"graph levels.png --end now --start end-3600s DEF:level0a={0}:level0:AVERAGE DEF:level1a={0}:level1:AVERAGE DEF:level2a={0}:level2:AVERAGE DEF:level3a={0}:level3:AVERAGE LINE1:level0a#edc240:""Tuner 1 Signal Level"" LINE1:level1a#afd8f8:""Tuner 2 Signal Level"" LINE1:level2a#cb4b4b:""Tuner 3 Signal Level"" LINE1:level3a#4da74d:""Tuner 4 Signal Level""";
                command = string.Format(commandFormat, pathTORRDDB);
                Execute(pathToRRDTOOL, command);


                commandFormat = @"graph snrs.png --end now --start end-3600s DEF:snr0a={0}:snr0:AVERAGE DEF:snr1a={0}:snr1:AVERAGE DEF:snr2a={0}:snr2:AVERAGE DEF:snr3a={0}:snr3:AVERAGE LINE1:snr0a#edc240:""Tuner 1 Signal SNR"" LINE1:snr1a#afd8f8:""Tuner 2 Signal SNR"" LINE1:snr2a#cb4b4b:""Tuner 3 Signal SNR"" LINE1:snr3a#4da74d:""Tuner 4 Signal SNR""";
                command = string.Format(commandFormat, pathTORRDDB);
                Execute(pathToRRDTOOL, command);

                System.Threading.Thread.Sleep(60000);
            }
        }

        static void Execute(string app, string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo(app, args);

            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            Process p = Process.Start(psi);

            p.WaitForExit();
        }

        static decimal Scale(decimal value, decimal originalMin, decimal originalMax, decimal newMin, decimal newMax)
        {
            return (((value - originalMin) * (newMax - newMin)) / (originalMax - originalMin)) + newMin;
        }
    }
}