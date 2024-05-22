using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLab12___ClientServer
{
    internal class Computer
    {
        public string Processor { get; set; }
        public int Ram { get; set; }
        public int Hdd { get; set; }

        public Computer(string processor, int ram, int hdd)
        {
            Processor = processor;
            Ram = ram;
            Hdd = hdd;
        }

        public override string ToString()
        {
            return $"Processor: {Processor}, Ram: {Ram} GB, Hdd: {Hdd} TB";
        }


    }
}
