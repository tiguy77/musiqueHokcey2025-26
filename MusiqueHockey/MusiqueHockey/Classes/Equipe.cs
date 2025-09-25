using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusiqueHockey.Classes
{
    public class Equipe
    {
        public string Nom { get; set; }
        public string Logo { get; set; } // Stocker le chemin d'une image
        public Musique MusiqueBut { get; set; }
        public Musique musiqueWarmup { get; set; }

        public Equipe(string nom, string logo, Musique musiqueBut, Musique musiqueWarmup)
        {
            Nom = nom;
            Logo = logo;
            MusiqueBut = musiqueBut;
            this.musiqueWarmup = musiqueWarmup;
        }

        public void PlayMusiqueBut()
        {
            Console.WriteLine($"{Nom} a marqué ! Lecture de la musique de but !");
            MusiqueBut.PlayMusique();
        }
    }
}
