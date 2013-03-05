using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ceton_plugin
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "config":
                        Config();
                        return;
                }
            }
            else
            {
                Ceton.CetonInfiniTV4 monitor = new Ceton.CetonInfiniTV4("192.168.200.1", Ceton.InfiniTV4TunerItems.Temperature);

                var stats = monitor.Update();

                foreach (var tuner in stats)
                {
                    Console.WriteLine("tuner{0}temp.value {1}", tuner.TunerIndex + 1, tuner.Temperature);
                }
            }
        }

        static void Config()
        {
            Console.WriteLine("graph_title Ceton Tuner Temperature");
            Console.WriteLine("graph_vlabel degrees C");
            Console.WriteLine("graph_category Ceton");
            Console.WriteLine("tuner1temp.label Tuner 1");
            Console.WriteLine("tuner2temp.label Tuner 2");
            Console.WriteLine("tuner3temp.label Tuner 3");
            Console.WriteLine("tuner4temp.label Tuner 4");
        }
    }
}
