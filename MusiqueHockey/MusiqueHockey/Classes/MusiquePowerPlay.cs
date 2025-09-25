using MusiqueHockey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusiqueHockeye.Classes
{
    public class MusiquePowerPlay : Musique
    {
        public MusiquePowerPlay(string filePath) : base("PowerPlay", filePath) { }
        public override void PlayMusique()
        {
            Console.WriteLine("🎸 PowerPlay activé !");
            base.PlayMusique();
        }
    }
}
