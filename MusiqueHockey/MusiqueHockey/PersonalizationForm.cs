using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MusiqueHockey.Classes;

namespace MusiqueHockey;

public sealed class PersonalizationForm : Form
{
    private sealed record Option(string Key, string Label)
    {
        public override string ToString() => Label;
    }
    private sealed record SongOption(string? Path, string Label)
    {
        public override string ToString() => Label;
    }

    private readonly ArenaLibrary library;
    private readonly Dictionary<string, Equipe> originalTeams;
    private readonly ComboBox categoryBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 230 };
    private readonly ComboBox destinationBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
    private readonly ListBox songList = new() { Dock = DockStyle.Fill, IntegralHeight = false };
    private readonly Label songsInfo = new() { Dock = DockStyle.Fill, AutoSize = false };
    private readonly ComboBox teamBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 350 };
    private readonly TextBox teamName = new() { Width = 440, MaxLength = 100 };
    private readonly ComboBox goalBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 600 };
    private readonly ComboBox warmupBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 600 };

    public PersonalizationForm(ArenaLibrary library, Dictionary<string, Equipe> originalTeams)
    {
        this.library = library;
        this.originalTeams = originalTeams;
        Text = "Aréna DJ — Personnaliser";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 550);
        ClientSize = new Size(960, 670);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(15, 23, 42);
        ForeColor = Color.White;

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(MakeMusicTab());
        tabs.TabPages.Add(MakeTeamTab());
        Controls.Add(tabs);
        tabs.SelectedIndexChanged += (_, _) => { if (tabs.SelectedIndex == 1) LoadTeam(); };
        foreach (var (key, label) in ArenaLibrary.Categories)
        {
            categoryBox.Items.Add(new Option(key, label));
            destinationBox.Items.Add(new Option(key, label));
        }
        categoryBox.SelectedIndexChanged += (_, _) => LoadTracks();
        categoryBox.SelectedIndex = 0;
        destinationBox.SelectedIndex = 0;
        foreach (var pair in originalTeams)
            teamBox.Items.Add(new Option(pair.Key, pair.Value.Nom));
        teamBox.SelectedIndexChanged += (_, _) => LoadTeam();
        if (teamBox.Items.Count > 0) teamBox.SelectedIndex = 0;
    }

    private static Label Hint(string text) => new()
    {
        Text = text, AutoSize = true, ForeColor = Color.FromArgb(148, 163, 184), Padding = new Padding(0, 5, 0, 5)
    };
    private static Button ActionButton(string text) => new()
    {
        Text = text, AutoSize = true, Padding = new Padding(12, 6, 12, 6),
        BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat, Margin = new Padding(5)
    };

    private TabPage MakeMusicTab()
    {
        var page = new TabPage("Bibliothèque musicale") { BackColor = Color.FromArgb(15, 23, 42), ForeColor = Color.White, Padding = new Padding(18) };
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 5, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));

        var selector = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        selector.Controls.Add(Hint("Section liée au bouton :"));
        selector.Controls.Add(categoryBox);
        selector.Controls.Add(Hint("Fichiers MP3 locaux, y compris la bibliothèque existante"));
        root.Controls.Add(selector, 0, 0);
        songList.BackColor = Color.FromArgb(30, 41, 59);
        songList.ForeColor = Color.White;
        songList.Font = new Font("Segoe UI", 11F);
        root.Controls.Add(songList, 0, 1);
        root.Controls.Add(songsInfo, 0, 2);

        var operations = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
        var import = ActionButton("Importer des MP3");
        var rename = ActionButton("Renommer le titre");
        var remove = ActionButton("Supprimer l'import");
        import.Click += (_, _) => Perform(ImportFiles);
        rename.Click += (_, _) => Perform(RenameSong);
        remove.Click += (_, _) => Perform(RemoveSong);
        operations.Controls.AddRange(new Control[] { import, rename, remove });
        root.Controls.Add(operations, 0, 3);

        var move = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
        move.Controls.Add(Hint("Changer de section :"));
        move.Controls.Add(destinationBox);
        var moveButton = ActionButton("Déplacer la musique sélectionnée");
        moveButton.Click += (_, _) => Perform(MoveSong);
        move.Controls.Add(moveButton);
        root.Controls.Add(move, 0, 4);
        page.Controls.Add(root);
        return page;
    }

    private TabPage MakeTeamTab()
    {
        var page = new TabPage("Équipes et musiques de but") { BackColor = Color.FromArgb(15, 23, 42), ForeColor = Color.White, Padding = new Padding(24) };
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 9 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(Hint("Choisir l'équipe à personnaliser"), 0, 0);
        root.Controls.Add(teamBox, 0, 1);
        root.Controls.Add(Hint("Nom affiché pendant le match"), 0, 2);
        root.Controls.Add(teamName, 0, 3);
        root.Controls.Add(Hint("Musique du bouton BUT (catégorie Musiques de but)"), 0, 4);
        root.Controls.Add(goalBox, 0, 5);
        root.Controls.Add(Hint("Musique du bouton ÉCHAUFFEMENT (catégorie Échauffement)"), 0, 6);
        root.Controls.Add(warmupBox, 0, 7);
        var footer = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        var save = ActionButton("Enregistrer cette équipe");
        save.Click += (_, _) => Perform(SaveTeam);
        footer.Controls.Add(save);
        footer.Controls.Add(Hint("Importez les MP3 dans les catégories But ou Échauffement depuis l'onglet Bibliothèque."));
        footer.Controls.Add(Hint("Les autres équipes et les fichiers d'origine restent inchangés."));
        root.Controls.Add(footer, 0, 8);
        page.Controls.Add(root);
        return page;
    }

    private void LoadTracks()
    {
        var previous = (songList.SelectedItem as LibraryTrack)?.Path;
        songList.Items.Clear();
        if (categoryBox.SelectedItem is not Option category) return;
        foreach (var song in library.Tracks(category.Key)) songList.Items.Add(song);
        var match = songList.Items.Cast<LibraryTrack>().FirstOrDefault(x => x.Path == previous);
        if (match is not null) songList.SelectedItem = match;
        songsInfo.Text = $"{songList.Items.Count} musique(s) — les titres marqués « ancienne bibliothèque » ne sont pas supprimés par cet écran.";
        if (destinationBox.SelectedIndex < 0) destinationBox.SelectedIndex = categoryBox.SelectedIndex;
    }

    private void ImportFiles()
    {
        if (categoryBox.SelectedItem is not Option category) return;
        using var picker = new OpenFileDialog
        {
            Title = $"Importer dans : {category.Label}",
            Filter = "Fichiers MP3 (*.mp3)|*.mp3",
            Multiselect = true
        };
        if (picker.ShowDialog(this) != DialogResult.OK) return;
        var count = 0;
        foreach (var path in picker.FileNames) { library.Add(path, category.Key); count++; }
        LoadTracks();
        MessageBox.Show(this, $"{count} fichier(s) importé(s) dans {category.Label}.", "Importation", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void RenameSong()
    {
        if (songList.SelectedItem is not LibraryTrack song) return;
        var answer = AskTitle(song.Title);
        if (answer is null) return;
        library.Preferences.Titles[song.Path] = answer;
        library.Save();
        LoadTracks();
    }

    private void MoveSong()
    {
        if (songList.SelectedItem is not LibraryTrack song || destinationBox.SelectedItem is not Option target) return;
        if (!song.Imported)
        {
            MessageBox.Show(this, "Cette musique provient de l'ancienne bibliothèque. Importez une copie dans la catégorie souhaitée pour conserver l'original.");
            return;
        }
        library.Move(song, target.Key);
        LoadTracks();
    }

    private void RemoveSong()
    {
        if (songList.SelectedItem is not LibraryTrack song) return;
        if (!song.Imported)
        {
            MessageBox.Show(this, "Les musiques de l'ancienne bibliothèque ne sont pas effacées. Vous pouvez supprimer uniquement les fichiers importés depuis cet écran.");
            return;
        }
        if (MessageBox.Show(this, $"Supprimer définitivement le fichier importé « {song.Title} » ?", "Confirmer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        library.Remove(song);
        LoadTracks();
    }

    private void LoadTeam()
    {
        if (teamBox.SelectedItem is not Option team || !originalTeams.TryGetValue(team.Key, out var original)) return;
        library.Preferences.Teams.TryGetValue(team.Key, out var settings);
        teamName.Text = string.IsNullOrWhiteSpace(settings?.Name) ? original.Nom : settings.Name;
        FillMusicBox(goalBox, "buts", original.MusiqueBut.Fichier, settings?.GoalPath);
        FillMusicBox(warmupBox, "warmup", original.musiqueWarmup.Fichier, settings?.WarmupPath);
    }

    private void FillMusicBox(ComboBox box, string category, string defaultPath, string? chosenPath)
    {
        box.Items.Clear();
        box.Items.Add(new SongOption(null, "Musique d'origine de cette équipe"));
        foreach (var song in library.Tracks(category))
            box.Items.Add(new SongOption(song.Path, song.Title));
        if (!string.IsNullOrEmpty(chosenPath) && !box.Items.Cast<SongOption>().Any(item => item.Path == chosenPath))
            box.Items.Add(new SongOption(chosenPath, $"Fichier introuvable : {Path.GetFileName(chosenPath)}"));
        box.SelectedItem = box.Items.Cast<SongOption>().FirstOrDefault(item => item.Path == chosenPath) ?? box.Items[0];
    }

    private void SaveTeam()
    {
        if (teamBox.SelectedItem is not Option team || !originalTeams.ContainsKey(team.Key)) return;
        var name = teamName.Text.Trim();
        if (name.Length == 0) { MessageBox.Show(this, "Entrez un nom d'équipe."); return; }
        if (originalTeams.Keys.Where(key => key != team.Key).Any(key =>
            string.Equals(library.Preferences.Teams.TryGetValue(key, out var existing) && !string.IsNullOrWhiteSpace(existing.Name)
                ? existing.Name : originalTeams[key].Nom, name, StringComparison.CurrentCultureIgnoreCase)))
        {
            MessageBox.Show(this, "Deux équipes ne peuvent pas porter le même nom.");
            return;
        }
        library.Preferences.Teams[team.Key] = new TeamPreference
        {
            Name = name,
            GoalPath = (goalBox.SelectedItem as SongOption)?.Path,
            WarmupPath = (warmupBox.SelectedItem as SongOption)?.Path
        };
        library.Save();
        var option = teamBox.SelectedItem as Option;
        if (option is not null)
        {
            var index = teamBox.SelectedIndex;
            teamBox.Items[index] = new Option(option.Key, name);
            teamBox.SelectedIndex = index;
        }
        MessageBox.Show(this, $"Configuration de « {name} » enregistrée.", "Équipes", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private string? AskTitle(string current)
    {
        using var dialog = new Form { Text = "Titre affiché", StartPosition = FormStartPosition.CenterParent, ClientSize = new Size(480, 145), FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
        var box = new TextBox { Left = 16, Top = 18, Width = 445, Text = current, MaxLength = 150 };
        var ok = new Button { Text = "Enregistrer", Left = 250, Top = 75, Width = 100, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Annuler", Left = 362, Top = 75, Width = 100, DialogResult = DialogResult.Cancel };
        dialog.Controls.AddRange(new Control[] { box, ok, cancel });
        dialog.AcceptButton = ok;
        dialog.CancelButton = cancel;
        return dialog.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(box.Text) ? box.Text.Trim() : null;
    }

    private void Perform(Action action)
    {
        try { action(); }
        catch (Exception error)
        {
            MessageBox.Show(this, $"Modification impossible : {error.Message}", "Personnalisation", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
