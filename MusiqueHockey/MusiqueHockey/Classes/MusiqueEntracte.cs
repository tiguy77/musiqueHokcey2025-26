using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusiqueHockey.Classes
{
    public class MusiqueEntracte : Musique
    {
        public MusiqueEntracte(string filePath) : base("Entracte", filePath) { }
        public override void PlayMusique()
        {
            Console.WriteLine("🎭 Entracte !");
            base.PlayMusique();
        }
    }
}
