using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace CetonCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            string ipAddress = args.Length < 1 ? "192.168.200.1" : args[0];
            string filePath = args.Length < 2 ? "ceton.csv" : args[1];

            Ceton.CetonInfiniTV4 mgr = new Ceton.CetonInfiniTV4(ipAddress);

            using (TextWriter w = new StreamWriter(filePath, true))
            {
                w.WriteLine("DateTime,Tuner,Temperature in C,SignalLevel dBmV,SignalSNR dB,Frequency,ChannelNumber,TransportState,Modulation,CopyProtectionSatus");

                while (true)
                {
                    var stats = mgr.Update();

                    foreach (var stat in stats)
                    {
                        w.WriteLine("{0:s},{1},{2},{3},{4},{5},{6},{7},{8},{9}", DateTime.Now, stat.TunerIndex, stat.Temperature, stat.SignalLevel, stat.SignalSNR, stat.Frequency, stat.ChannelNumber, stat.TransportState, stat.Modulation, stat.CopyProtectionStatus);
                    }

                    w.Flush();

                    System.Threading.Thread.Sleep(30000);
                }
            }
        }
    }
}
