using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusiqueHockey.Classes
{
    public class MusiqueWarmup : Musique
    {
        public MusiqueWarmup(string filePath) : base("Warmup", filePath) { }
        public override void PlayMusique()
        {
            Console.WriteLine("🔥 Warmup en cours !");
            base.PlayMusique();
        }
    }
}
