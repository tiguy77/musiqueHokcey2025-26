using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MusiqueHockey.Classes;

namespace MusiqueHockey;

public partial class Form1
{
    private ArenaLibrary? arenaLibrary;
    private Dictionary<string, Equipe>? originalTeams;

    private void ConfigurePersonalization()
    {
        SettingsButton.Click += SettingsButton_Click;
        Shown += (_, _) =>
        {
            try
            {
                arenaLibrary = new ArenaLibrary(basePath);
                arenaLibrary.Load();
                originalTeams = equipes.ToDictionary(
                    item => item.Key,
                    item => new Equipe(item.Value.Nom, item.Value.Logo, item.Value.MusiqueBut, item.Value.musiqueWarmup));
                ApplyPersonalization();
            }
            catch (Exception error)
            {
                MessageBox.Show(this, $"Impossible de lire les préférences locales : {error.Message}", "Personnalisation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        };
        // Reset reloads the old library first; add imported music and custom teams afterwards.
        ResetButton.Click += (_, _) => ApplyPersonalization();
        // The cloud handler refreshes the old lists before reporting completion.
        StatusLabel.TextChanged += (_, _) =>
        {
            if (StatusLabel.Text.StartsWith("À jour", StringComparison.Ordinal)) ApplyPersonalization();
        };
    }

    private void SettingsButton_Click(object? sender, EventArgs e)
    {
        if (arenaLibrary is null || originalTeams is null)
        {
            MessageBox.Show(this, "Les préférences ne sont pas encore chargées.");
            return;
        }
        if (AllKnownTracks().Any(track => track.IsPlaying))
        {
            var answer = MessageBox.Show(this,
                "Ouvrir Personnaliser arrêtera la musique et réinitialisera le match en cours. Continuer ?",
                "Personnaliser", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;
        }
        // Release audio file handles before moving or deleting imported tracks.
        ResetButton.PerformClick();
        using (var editor = new PersonalizationForm(arenaLibrary, originalTeams))
            editor.ShowDialog(this);
        ApplyPersonalization();
        StatusLabel.Text = "Configuration locale mise à jour";
    }

    private List<Musique> BuildLibraryPlaylist(string category)
    {
        return arenaLibrary!.Tracks(category)
            .Select(track => new Musique(track.Title, track.Path)).ToList();
    }

    private void ApplyPersonalization()
    {
        if (arenaLibrary is null || originalTeams is null) return;

        // Rebuild all playlists so the assigned category controls the actual button.
        playlist = BuildLibraryPlaylist("all");
        entractePlaylist = BuildLibraryPlaylist("entracte");
        powerPlayPlaylist = BuildLibraryPlaylist("PPLocal");
        pkPlaylist = BuildLibraryPlaylist("PPvis");
        finPartiePlaylist = BuildLibraryPlaylist("finGame");
        if (playlist.Count == 0) currentTrackIndex = 0;
        else currentTrackIndex %= playlist.Count;
        if (finPartiePlaylist.Count == 0) nextFinPartieIndex = 0;
        else nextFinPartieIndex %= finPartiePlaylist.Count;

        var previousLocal = LocalBox.SelectedItem?.ToString();
        var previousVisitor = VisiteurBox.SelectedItem?.ToString();
        var updated = new Dictionary<string, Equipe>(StringComparer.CurrentCultureIgnoreCase);
        foreach (var (identity, original) in originalTeams)
        {
            arenaLibrary.Preferences.Teams.TryGetValue(identity, out var selection);
            var name = string.IsNullOrWhiteSpace(selection?.Name) ? original.Nom : selection.Name.Trim();
            // Defensive fallback in case someone edited the JSON settings file by hand.
            if (updated.ContainsKey(name)) name = $"{name} ({identity})";
            var goalPath = !string.IsNullOrWhiteSpace(selection?.GoalPath) && File.Exists(selection.GoalPath)
                ? selection.GoalPath : original.MusiqueBut.Fichier;
            var warmupPath = !string.IsNullOrWhiteSpace(selection?.WarmupPath) && File.Exists(selection.WarmupPath)
                ? selection.WarmupPath : original.musiqueWarmup.Fichier;
            var team = new Equipe(name, original.Logo,
                new Musique($"But {name} — {DisplayName(goalPath)}", goalPath),
                new Musique($"Échauffement {name} — {DisplayName(warmupPath)}", warmupPath));
            updated[name] = team;
        }

        equipes = updated;
        LocalBox.Items.Clear();
        VisiteurBox.Items.Clear();
        foreach (var name in equipes.Keys)
        {
            LocalBox.Items.Add(name);
            VisiteurBox.Items.Add(name);
        }
        if (previousLocal is not null && equipes.ContainsKey(previousLocal)) LocalBox.SelectedItem = previousLocal;
        if (previousVisitor is not null && equipes.ContainsKey(previousVisitor)) VisiteurBox.SelectedItem = previousVisitor;
        UpdatePlaybackDisplay();
    }

    private string DisplayName(string path)
    {
        if (arenaLibrary?.Preferences.Titles.TryGetValue(path, out var title) == true && !string.IsNullOrWhiteSpace(title))
            return title;
        return Path.GetFileNameWithoutExtension(path);
    }
}
