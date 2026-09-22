using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MusiqueHockey;

public partial class Form1
{
    private readonly System.Windows.Forms.Timer playbackDisplayTimer = new() { Interval = 250 };
    private List<Musique> finPartiePlaylist = new();
    private Musique? lastFinPartieTrack;
    private int nextFinPartieIndex;

    // Called by InitializeComponent; the playlist fields in Form1.cs are ready by Shown.
    private void ConfigurePlaybackExtras()
    {
        FinPartieBtn.Click += FinPartieBtn_Click;
        ResetButton.Click += ResetButton_Click;
        playbackDisplayTimer.Tick += (_, _) => UpdatePlaybackDisplay();
        Shown += (_, _) =>
        {
            ReloadFinPartiePlaylist();
            UpdatePlaybackDisplay();
            playbackDisplayTimer.Start();
        };
        FormClosed += (_, _) => playbackDisplayTimer.Dispose();
    }

    private void ReloadFinPartiePlaylist()
    {
        var folder = Path.Combine(basePath, "netX", "musiques", "finGame");
        finPartiePlaylist = Directory.Exists(folder)
            ? Directory.GetFiles(folder, "*.mp3")
                .OrderBy(path => Path.GetFileName(path), StringComparer.CurrentCultureIgnoreCase)
                .Select(path => new Musique(Path.GetFileNameWithoutExtension(path), path))
                .ToList()
            : new List<Musique>();
        if (nextFinPartieIndex >= finPartiePlaylist.Count)
            nextFinPartieIndex = 0;
    }

    private void FinPartieBtn_Click(object? sender, EventArgs e)
    {
        // A second press stops the current finale; the next press plays the next file.
        if (lastFinPartieTrack?.IsPlaying == true)
        {
            lastFinPartieTrack.Stop();
            if (ReferenceEquals(musiqueEnCours, lastFinPartieTrack))
                musiqueEnCours = null!;
            UpdatePlaybackDisplay();
            return;
        }

        ReloadFinPartiePlaylist();
        if (finPartiePlaylist.Count == 0)
        {
            MessageBox.Show(
                "Aucune musique de fin de partie trouvée. Ajoutez un MP3 dans netX\\musiques\\finGame, puis réessayez.",
                "Fin de partie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdatePlaybackDisplay();
            return;
        }

        StopActiveTracks();
        musiqueEnCours = null!;
        musiqueStoppee = false;
        var selected = finPartiePlaylist[nextFinPartieIndex];
        selected.PlayMusique();
        if (selected.IsPlaying)
        {
            lastFinPartieTrack = selected;
            musiqueEnCours = selected;
            nextFinPartieIndex = (nextFinPartieIndex + 1) % finPartiePlaylist.Count;
        }
        UpdatePlaybackDisplay();
    }

    private void ResetButton_Click(object? sender, EventArgs e)
    {
        StopActiveTracks();
        musiqueEnCours = null!;
        lastFinPartieTrack = null;
        musiqueStoppee = false;
        currentTrackIndex = 0;
        warmupTrackIndex = 0;
        nextFinPartieIndex = 0;

        LocalBox.SelectedIndex = -1;
        VisiteurBox.SelectedIndex = -1;
        PlayMusiqueBtn.Enabled = true;
        LocalButBtn.Enabled = true;
        VisiteurButBtn.Enabled = true;
        WarmUpbtn.Enabled = true;
        EntracteBtn.Enabled = true;
        PenLocalBtn.Enabled = true;
        PenVisBtn.Enabled = true;
        FinPartieBtn.Enabled = true;

        // Reset does not delete downloaded music or log the user out.
        InitPlaylist();
        InitPowerPlayPlaylist();
        InitPKPlaylist();
        InitEntracte();
        ReloadFinPartiePlaylist();
        StatusLabel.Text = cloudMusicService is null
            ? $"Mode hors ligne • {playlist.Count} pistes locales"
            : $"Prêt • {playlist.Count} pistes locales";
        UpdatePlaybackDisplay();
    }

    private IEnumerable<Musique> AllKnownTracks()
    {
        if (musiqueEnCours is not null) yield return musiqueEnCours;
        if (lastFinPartieTrack is not null) yield return lastFinPartieTrack;
        if (playlist is not null)
            foreach (var track in playlist) yield return track;
        if (entractePlaylist is not null)
            foreach (var track in entractePlaylist) yield return track;
        if (powerPlayPlaylist is not null)
            foreach (var track in powerPlayPlaylist) yield return track;
        if (pkPlaylist is not null)
            foreach (var track in pkPlaylist) yield return track;
        foreach (var track in finPartiePlaylist) yield return track;
        if (equipes is not null)
            foreach (var team in equipes.Values)
            {
                yield return team.MusiqueBut;
                yield return team.musiqueWarmup;
            }
    }

    private void StopActiveTracks()
    {
        foreach (var track in AllKnownTracks().Distinct())
            if (track.IsPlaying) track.Stop();
    }

    private void UpdatePlaybackDisplay()
    {
        var playing = AllKnownTracks().FirstOrDefault(track => track.IsPlaying);
        CurrentTrackLabel.Text = playing?.Nom ?? "Aucune musique";
        Musique? next = null;
        if (playing is not null)
        {
            if (finPartiePlaylist.Contains(playing))
                next = finPartiePlaylist.Count > 0 ? finPartiePlaylist[nextFinPartieIndex] : null;
            else if (playlist is not null && playlist.Contains(playing))
                next = NextInList(playlist, playing);
            else if (entractePlaylist is not null && entractePlaylist.Contains(playing))
                next = NextInList(entractePlaylist, playing);
            else if (powerPlayPlaylist is not null && powerPlayPlaylist.Contains(playing))
                next = NextInList(powerPlayPlaylist, playing);
            else if (pkPlaylist is not null && pkPlaylist.Contains(playing))
                next = NextInList(pkPlaylist, playing);
            else if (playlist is { Count: > 0 })
                next = playlist[currentTrackIndex % playlist.Count];
        }
        else if (lastFinPartieTrack is not null && finPartiePlaylist.Count > 0)
            next = finPartiePlaylist[nextFinPartieIndex];
        else if (playlist is { Count: > 0 })
            next = playlist[(currentTrackIndex + (musiqueStoppee ? 1 : 0)) % playlist.Count];

        NextTrackLabel.Text = next?.Nom ?? "—";
    }

    private static Musique? NextInList(List<Musique> tracks, Musique playing)
    {
        var index = tracks.IndexOf(playing);
        return index >= 0 && tracks.Count > 0 ? tracks[(index + 1) % tracks.Count] : null;
    }
}
