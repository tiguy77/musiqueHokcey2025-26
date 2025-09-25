using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusiqueHockey.Classes;

namespace MusiqueHockey
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        private Dictionary<string, Equipe> equipes;
        private Equipe equipeVisiteur;
        private string basePath;
        private string butPath;
        private string allPath;
        private string warmupPath;
        private string PowerPlayPath;
        private string pkPath;
        private string entractePath;
        private List<Musique> playlist;
        private List<Musique> warmup1Playlist;
        private List<Musique> warmup2Playlist;
        private List<Musique> powerPlayPlaylist;
        private List<Musique> pkPlaylist;
        private List<Musique> entractePlaylist;
        private int currentTrackIndex = 0;
        private int warmupTrackIndex = 0;
        private Musique musiqueEnCours;
        public Form1()
        {
            InitializeComponent();

            AllocConsole();
            basePath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("Chemin de base : " + AppDomain.CurrentDomain.BaseDirectory);
            butPath = Path.Combine(basePath, "netX\\musiques\\");
            allPath = Path.Combine(basePath, "netX\\musiques\\all\\");
            warmupPath = Path.Combine(basePath, "netX\\musiques\\warmup\\");
            PowerPlayPath = Path.Combine(basePath, "netX\\musiques\\PPLocal\\");
            pkPath = Path.Combine(basePath, "netX\\musiques\\PPvis\\");
            entractePath = Path.Combine(basePath, "netX\\musiques\\entracte\\");

            InitEquipes();
            InitPlaylist();
            InitPowerPlayPlaylist();
            InitPKPlaylist();
            InitEntracte();

        }
        private void InitEquipes()
        {
            equipes = new Dictionary<string, Equipe>
       {
               { "Assurance S�guin", new Equipe("Assurance S�guin", "seguin_logo.png", new Musique("But Assurance S�guin", Path.Combine(butPath, "buts\\seguin_music.mp3")),new Musique("Warmup Assurance S�guin", Path.Combine(warmupPath, "seguin_music.mp3"))) },
               { "**Seguin Special", new Equipe("**Seguin Special", "seguin_logo.png", new Musique("But Assurance S�guin", Path.Combine(butPath, "buts\\seguinSP_music.mp3")),new Musique("Warmup Assurance S�guin", Path.Combine(warmupPath, "seguin_music.mp3"))) },
               { "Coffrages Thibault", new Equipe("Coffrages Thibault", "thibault_logo.png", new Musique("But Coffrages Thibault", Path.Combine(butPath, "buts\\thibault_music.mp3")),new Musique("Warmup Coffrages Thibault", Path.Combine(warmupPath, "thibault_music.mp3"))) },
               { "Concept Ph�nix", new Equipe("Concept Ph�nix", "phenix_logo.png", new Musique("But Concept Ph�nix", Path.Combine(butPath, "buts\\phenix_music.mp3")),new Musique("Warmup Concept Ph�nix", Path.Combine(warmupPath, "phenix_music.mp3"))) },
               { "Laporte&Fils", new Equipe("Laporte&Fils", "Laporte_logo.png", new Musique("But Laporte", Path.Combine(butPath, "buts\\laporte_music.mp3")),new Musique("Warmup Laporte", Path.Combine(warmupPath, "laporte_music.mp3"))) },
               { "Fauteux Mini-Moteur", new Equipe("Fauteux Mini-Moteur", "fauteux_logo.png", new Musique("But Fauteux Mini-Moteur", Path.Combine(butPath, "buts\\fauteux_music.mp3")),new Musique("Warmup Fauteux Mini-Moteur", Path.Combine(warmupPath, "fauteux_music.mp3"))) },
               { "Hubby Mike", new Equipe("Hubby Mike", "hubby_logo.png", new Musique("But Hubby Mike", Path.Combine(butPath, "buts\\hubby_music.mp3")),new Musique("Warmup Hubby Mike", Path.Combine(warmupPath, "hubby_music.mp3"))) },
               { "Orlando", new Equipe("Orlando", "orlando_logo.png", new Musique("But Orlando", Path.Combine(butPath, "buts\\orlando_music.mp3")),new Musique("Warmup Orlando", Path.Combine(warmupPath, "orlando_music.mp3"))) },
               { "V�ris�cur", new Equipe("V�ris�cur", "verisecur_logo.png", new Musique("But V�ris�cur", Path.Combine(butPath, "buts\\verisecur_music.mp3")),new Musique("Warmup V�ris�cur", Path.Combine(warmupPath, "verisecur_music.mp3"))) }
            };
            foreach (var equipe in equipes.Keys)
            {
                LocalBox.Items.Add(equipe);
            }
        }
        private void InitPlaylist()
        {
            playlist = new List<Musique>();
            if (Directory.Exists(allPath))
            {
                foreach (var file in Directory.GetFiles(allPath, "*.mp3"))
                {
                    playlist.Add(new Musique(Path.GetFileNameWithoutExtension(file), file));
                }
            }
        }
        private void InitEntracte()
        {
            entractePlaylist = new List<Musique>();
            if (Directory.Exists(entractePath))
            {
                foreach (var file in Directory.GetFiles(entractePath, "*.mp3"))
                {
                    entractePlaylist.Add(new Musique(Path.GetFileNameWithoutExtension(file), file));
                }
            }
        }
        private void InitPowerPlayPlaylist()
        {
            powerPlayPlaylist = new List<Musique>();
            if (Directory.Exists(PowerPlayPath))
            {
                foreach (var file in Directory.GetFiles(PowerPlayPath, "*.mp3"))
                {
                    powerPlayPlaylist.Add(new Musique(Path.GetFileNameWithoutExtension(file), file));
                }
            }
        }
        private void InitPKPlaylist()
        {
            pkPlaylist = new List<Musique>();
            if (Directory.Exists(pkPath))
            {
                foreach (var file in Directory.GetFiles(pkPath, "*.mp3"))
                {
                    pkPlaylist.Add(new Musique(Path.GetFileNameWithoutExtension(file), file));
                }
            }
        }
        private void PlayMusiqueToggle(Musique musique)
        {
            if (musique.IsPlaying)
            {
                musique.Stop();
            }
            else
            {
                musique.PlayMusique();
            }
        }
        private bool musiqueStoppee = false;
        private void PlayMusiqueBtn_Click(object sender, EventArgs e)
        {
            if (playlist.Count == 0) return;
            // V�rifier si une musique joue et l'arr�ter si n�cessaire
            if (musiqueEnCours != null && musiqueEnCours.IsPlaying)
            {
                musiqueEnCours.Stop();
                musiqueEnCours = null;
                musiqueStoppee = true; // On note que la musique a �t� stopp�e
                EntracteBtn.Enabled = true;
                LocalButBtn.Enabled = true;
                VisiteurButBtn.Enabled = true;
                WarmUpbtn.Enabled = true;
                PenLocalBtn.Enabled = true;
                PenVisBtn.Enabled = true;
                return; // On sort de la m�thode sans changer de musique
            }
            // Si la musique a �t� stopp�e, on passe � la suivante
            if (musiqueStoppee)
            {
                currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;
                musiqueStoppee = false; // R�initialisation du flag apr�s changement de musique
            }
            // Jouer la nouvelle musique
            musiqueEnCours = playlist[currentTrackIndex];
            musiqueEnCours.PlayMusique();
            EntracteBtn.Enabled = false;
            LocalButBtn.Enabled = false;
            VisiteurButBtn.Enabled = false;
            WarmUpbtn.Enabled = false;
            PenLocalBtn.Enabled = false;
            PenVisBtn.Enabled = false;
        }
        private void LocalButBtn_Click(object sender, EventArgs e)
        {
            if (LocalBox.SelectedItem != null && equipes.TryGetValue(LocalBox.SelectedItem.ToString(), out var equipe))
            {
                PlayMusiqueToggle(equipe.MusiqueBut);
            }
            else
            {
                MessageBox.Show("Veuillez s�lectionner une �quipe valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void VisiteurButBtn_Click(object sender, EventArgs e)
        {
            if (VisiteurBox.SelectedItem != null && equipes.TryGetValue(VisiteurBox.SelectedItem.ToString(), out var equipe))
            {
                PlayMusiqueToggle(equipe.MusiqueBut);
            }
            else
            {
                MessageBox.Show("Aucune �quipe visiteuse d�finie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void WarmUpBtn_Click(object sender, EventArgs e)
        {
            if (LocalBox.SelectedItem != null && equipes.TryGetValue(LocalBox.SelectedItem.ToString(), out var equipe))
            {
                PlayMusiqueToggle(equipe.musiqueWarmup);
            }
            else
            {
                MessageBox.Show("Veuillez s�lectionner une �quipe valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void EntracteBtn_Click(object sender, EventArgs e)
        {
            if (entractePlaylist.Count == 0) return;
            // V�rifier si une musique joue et l'arr�ter si n�cessaire
            if (musiqueEnCours != null && musiqueEnCours.IsPlaying)
            {
                musiqueEnCours.Stop();
                musiqueEnCours = null;
                musiqueStoppee = true; // On note que la musique a �t� stopp�e
                return; // On sort de la m�thode sans changer de musique
            }
            // Si la musique a �t� stopp�e, on passe � la suivante
            if (musiqueStoppee)
            {
                currentTrackIndex = (currentTrackIndex + 1) % entractePlaylist.Count;
                musiqueStoppee = false; // R�initialisation du flag apr�s changement de musique
            }
            // Jouer la nouvelle musique
            musiqueEnCours = entractePlaylist[currentTrackIndex];
            musiqueEnCours.PlayMusique();
        }
        private void PenLocalBtn_Click(object sender, EventArgs e)
        {
            if (powerPlayPlaylist.Count == 0) return;
            // V�rifier si une musique joue et l'arr�ter si n�cessaire
            if (musiqueEnCours != null && musiqueEnCours.IsPlaying)
            {
                musiqueEnCours.Stop();
                musiqueEnCours = null;
                musiqueStoppee = true; // On note que la musique a �t� stopp�e
                return; // On sort de la m�thode sans changer de musique
            }
            // Si la musique a �t� stopp�e, on passe � la suivante
            if (musiqueStoppee)
            {
                currentTrackIndex = (currentTrackIndex + 1) % powerPlayPlaylist.Count;
                musiqueStoppee = false; // R�initialisation du flag apr�s changement de musique
            }
            // Jouer la nouvelle musique
            musiqueEnCours = powerPlayPlaylist[currentTrackIndex];
            musiqueEnCours.PlayMusique();
        }
        private void PenVisBtn_Click(object sender, EventArgs e)
        {
            if (pkPlaylist.Count == 0) return;
            // V�rifier si une musique joue et l'arr�ter si n�cessaire
            if (musiqueEnCours != null && musiqueEnCours.IsPlaying)
            {
                musiqueEnCours.Stop();
                musiqueEnCours = null;
                musiqueStoppee = true; // On note que la musique a �t� stopp�e
                return; // On sort de la m�thode sans changer de musique
            }
            // Si la musique a �t� stopp�e, on passe � la suivante
            if (musiqueStoppee)
            {
                currentTrackIndex = (currentTrackIndex + 1) % pkPlaylist.Count;
                musiqueStoppee = false; // R�initialisation du flag apr�s changement de musique
            }
            // Jouer la nouvelle musique
            musiqueEnCours = pkPlaylist[currentTrackIndex];
            musiqueEnCours.PlayMusique();
        }
    }
}
