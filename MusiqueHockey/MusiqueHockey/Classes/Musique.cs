using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusiqueHockey
{
    public class Musique
    {
        public string Nom { get; }
        public string Fichier { get; }
        public bool IsPlaying { get; private set; } = false;
        private WaveOutEvent waveOut;
        private AudioFileReader reader;

        public Musique(string nom, string fichier)
        {
            Nom = nom;
            Fichier = fichier;
        }

        public virtual void PlayMusique()
        {
            if (!File.Exists(Fichier))
            {
                MessageBox.Show($"Fichier introuvable : {Fichier}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Stop(); // Arrêter toute musique en cours avant d'en jouer une nouvelle

            reader = new AudioFileReader(Fichier);
            waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            waveOut.Play();
            IsPlaying = true;

            waveOut.PlaybackStopped += (sender, args) =>
            {
                IsPlaying = false;
                reader.Dispose();
                waveOut.Dispose();
            };
        }

        public void Stop()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                IsPlaying = false;
                reader?.Dispose();
                waveOut?.Dispose();
            }
        }
    }
}
