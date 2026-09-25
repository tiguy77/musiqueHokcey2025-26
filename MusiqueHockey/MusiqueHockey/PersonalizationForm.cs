using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly ListBox songList = new()
    {
        Dock = DockStyle.Fill,
        IntegralHeight = false,
        AllowDrop = true
    };
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

        tabs.SelectedIndexChanged += (_, _) =>
        {
            if (tabs.SelectedIndex == 1) LoadTeam();
        };

        foreach (var (key, label) in ArenaLibrary.Categories)
        {
            categoryBox.Items.Add(new Option(key, label));
            destinationBox.Items.Add(new Option(key, label));
        }

        categoryBox.SelectedIndexChanged += (_, _) => LoadTracks();
        categoryBox.SelectedIndex = 0;
        destinationBox.SelectedIndex = 0;

        teamBox.SelectedIndexChanged += (_, _) => LoadTeam();
        ReloadTeamOptions();
    }

    private static Label Hint(string text) => new()
    {
        Text = text,
        AutoSize = true,
        ForeColor = Color.FromArgb(148, 163, 184),
        Padding = new Padding(0, 5, 0, 5)
    };

    private static Button ActionButton(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Padding = new Padding(12, 6, 12, 6),
        BackColor = Color.FromArgb(37, 99, 235),
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat,
        Margin = new Padding(5)
    };

    private TabPage MakeMusicTab()
    {
        var page = new TabPage("Bibliothèque musicale")
        {
            BackColor = Color.FromArgb(15, 23, 42),
            ForeColor = Color.White,
            Padding = new Padding(18)
        };

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
        songList.DragEnter += SongList_DragEnter;
        songList.DragOver += SongList_DragEnter;
        songList.DragLeave += (_, _) => SetDropVisual(false);
        songList.DragDrop += async (_, e) => await PerformAsync(() => ImportDroppedFilesAsync(e));
        root.Controls.Add(songList, 0, 1);
        root.Controls.Add(songsInfo, 0, 2);

        var operations = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
        var import = ActionButton("Parcourir dans l’Explorateur");
        var rename = ActionButton("Renommer le titre");
        var remove = ActionButton("Supprimer l'import");

        import.Click += async (_, _) => await PerformAsync(() => ImportFilesAsync(import));
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
        var page = new TabPage("Équipes et musiques de but")
        {
            BackColor = Color.FromArgb(15, 23, 42),
            ForeColor = Color.White,
            Padding = new Padding(24)
        };

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 9 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(Hint("Choisir une équipe"), 0, 0);

        var teamSelector = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
        teamSelector.Controls.Add(teamBox);

        var addTeam = ActionButton("＋ Ajouter une équipe");
        addTeam.Click += (_, _) => Perform(AddTeam);
        teamSelector.Controls.Add(addTeam);

        var deleteTeam = ActionButton("Supprimer l'équipe");
        deleteTeam.BackColor = Color.FromArgb(153, 27, 27);
        deleteTeam.Click += (_, _) => Perform(DeleteTeam);
        teamSelector.Controls.Add(deleteTeam);

        root.Controls.Add(teamSelector, 0, 1);
        root.Controls.Add(Hint("Nom affiché pendant le match"), 0, 2);
        root.Controls.Add(teamName, 0, 3);
        root.Controls.Add(Hint("Musique du bouton BUT (catégorie Musiques de but)"), 0, 4);
        root.Controls.Add(goalBox, 0, 5);
        root.Controls.Add(Hint("Musique du bouton ÉCHAUFFEMENT (catégorie Échauffement)"), 0, 6);
        root.Controls.Add(warmupBox, 0, 7);

        var footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };

        var save = ActionButton("Enregistrer cette équipe");
        save.Click += (_, _) => Perform(SaveTeam);
        footer.Controls.Add(save);
        footer.Controls.Add(Hint("Vous pouvez ajouter et supprimer des équipes ici. Les changements sont conservés au prochain démarrage."));
        footer.Controls.Add(Hint("Importez les MP3 de but ou d'échauffement depuis l'onglet Bibliothèque musicale."));
        root.Controls.Add(footer, 0, 8);

        page.Controls.Add(root);
        return page;
    }

    private void LoadTracks()
    {
        var previous = (songList.SelectedItem as LibraryTrack)?.Path;
        songList.Items.Clear();

        if (categoryBox.SelectedItem is not Option category) return;

        foreach (var song in library.Tracks(category.Key))
            songList.Items.Add(song);

        var match = songList.Items.Cast<LibraryTrack>().FirstOrDefault(x => x.Path == previous);
        if (match is not null) songList.SelectedItem = match;

        songsInfo.Text = $"{songList.Items.Count} musique(s) • Glissez-déposez des MP3 ici depuis l’Explorateur Windows, ou utilisez Parcourir.";
        if (destinationBox.SelectedIndex < 0) destinationBox.SelectedIndex = categoryBox.SelectedIndex;
    }

    private async Task ImportFilesAsync(Button importButton)
    {
        if (categoryBox.SelectedItem is not Option category) return;

        using var picker = new OpenFileDialog
        {
            Title = $"Ajouter des MP3 à : {category.Label}",
            Filter = "Fichiers MP3 (*.mp3)|*.mp3",
            Multiselect = true,
            CheckFileExists = true,
            CheckPathExists = true,
            DereferenceLinks = true,
            RestoreDirectory = true,
            InitialDirectory = GetImportStartFolder(),

            // Use the current Windows Explorer-style file picker.
            AutoUpgradeEnabled = true
        };

        if (picker.ShowDialog(this) != DialogResult.OK) return;
        await ImportPathsAsync(picker.FileNames, importButton);
    }

    private void SongList_DragEnter(object? sender, DragEventArgs e)
    {
        var files = GetDroppedMp3Files(e);
        var accepted = files.Length > 0;
        e.Effect = accepted ? DragDropEffects.Copy : DragDropEffects.None;
        SetDropVisual(accepted);
    }

    private async Task ImportDroppedFilesAsync(DragEventArgs e)
    {
        SetDropVisual(false);
        var files = GetDroppedMp3Files(e);
        if (files.Length == 0)
        {
            MessageBox.Show(
                this,
                "Déposez un ou plusieurs fichiers MP3 provenant de l’Explorateur Windows.",
                "Importation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        await ImportPathsAsync(files, null);
    }

    private static string[] GetDroppedMp3Files(DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return Array.Empty<string>();

        return (e.Data.GetData(DataFormats.FileDrop) as string[] ?? Array.Empty<string>())
            .Where(path =>
                File.Exists(path) &&
                string.Equals(Path.GetExtension(path), ".mp3", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private void SetDropVisual(bool active)
    {
        songList.BackColor = active
            ? Color.FromArgb(30, 64, 100)
            : Color.FromArgb(30, 41, 59);

        if (active)
            songsInfo.Text = "Relâchez pour importer les MP3 dans la section sélectionnée.";
        else if (categoryBox.SelectedItem is Option)
            songsInfo.Text = $"{songList.Items.Count} musique(s) • Glissez-déposez des MP3 ici depuis l’Explorateur Windows, ou utilisez Parcourir.";
    }

    private async Task ImportPathsAsync(IEnumerable<string> paths, Button? importButton)
    {
        if (categoryBox.SelectedItem is not Option category) return;

        var files = paths
            .Where(path =>
                File.Exists(path) &&
                string.Equals(Path.GetExtension(path), ".mp3", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (files.Length == 0) return;

        var originalText = importButton?.Text;
        if (importButton is not null)
            importButton.Enabled = false;

        try
        {
            var count = 0;
            for (var index = 0; index < files.Length; index++)
            {
                var path = files[index];
                var fileName = Path.GetFileName(path);

                if (importButton is not null)
                    importButton.Text = $"Importation {index + 1}/{files.Length}";

                songsInfo.Text = $"Importation de « {fileName} » vers {category.Label}…";

                using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
                try
                {
                    await library.AddAsync(path, category.Key, timeout.Token);
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException($"L'import de « {fileName} » a dépassé 2 minutes et a été annulé.");
                }

                count++;
            }

            LoadTracks();
            MessageBox.Show(
                this,
                $"{count} fichier(s) importé(s) dans {category.Label}.",
                "Importation terminée",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        finally
        {
            if (importButton is not null)
            {
                importButton.Enabled = true;
                importButton.Text = originalText ?? "Parcourir dans l’Explorateur";
            }
        }
    }

    private static string GetImportStartFolder()
    {
        var music = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        if (!string.IsNullOrWhiteSpace(music) && Directory.Exists(music))
            return music;

        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        if (!string.IsNullOrWhiteSpace(desktop) && Directory.Exists(desktop))
            return desktop;

        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    private void RenameSong()
    {
        if (songList.SelectedItem is not LibraryTrack song) return;

        var answer = AskText("Titre affiché", song.Title);
        if (answer is null) return;

        library.Preferences.Titles[song.Path] = answer;
        library.Save();
        LoadTracks();
    }

    private void MoveSong()
    {
        if (songList.SelectedItem is not LibraryTrack song ||
            destinationBox.SelectedItem is not Option target) return;

        if (!song.Imported)
        {
            MessageBox.Show(
                this,
                "Cette musique provient de l'ancienne bibliothèque. Importez une copie dans la catégorie souhaitée pour conserver l'original.");
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
            MessageBox.Show(
                this,
                "Les musiques de l'ancienne bibliothèque ne sont pas effacées. Vous pouvez supprimer uniquement les fichiers importés depuis cet écran.");
            return;
        }

        if (MessageBox.Show(
                this,
                $"Supprimer définitivement le fichier importé « {song.Title} » ?",
                "Confirmer",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        library.Remove(song);
        LoadTracks();
    }

    private void ReloadTeamOptions(string? selectKey = null)
    {
        var previousKey = selectKey ?? (teamBox.SelectedItem as Option)?.Key;
        teamBox.Items.Clear();

        foreach (var (key, original) in originalTeams)
        {
            library.Preferences.Teams.TryGetValue(key, out var settings);
            if (settings?.Deleted == true) continue;

            var name = string.IsNullOrWhiteSpace(settings?.Name)
                ? original.Nom
                : settings.Name.Trim();

            teamBox.Items.Add(new Option(key, name));
        }

        foreach (var pair in library.Preferences.Teams
                     .Where(pair => pair.Value.IsCustom && !pair.Value.Deleted)
                     .OrderBy(pair => pair.Value.Name, StringComparer.CurrentCultureIgnoreCase))
        {
            var name = string.IsNullOrWhiteSpace(pair.Value.Name) ? "Nouvelle équipe" : pair.Value.Name.Trim();
            teamBox.Items.Add(new Option(pair.Key, name));
        }

        var selected = teamBox.Items.Cast<Option>()
            .FirstOrDefault(option => string.Equals(option.Key, previousKey, StringComparison.OrdinalIgnoreCase));

        if (selected is not null)
            teamBox.SelectedItem = selected;
        else if (teamBox.Items.Count > 0)
            teamBox.SelectedIndex = 0;
        else
        {
            teamName.Clear();
            goalBox.Items.Clear();
            warmupBox.Items.Clear();
        }
    }

    private void LoadTeam()
    {
        if (teamBox.SelectedItem is not Option team) return;

        var hasOriginal = originalTeams.TryGetValue(team.Key, out var original);
        library.Preferences.Teams.TryGetValue(team.Key, out var settings);

        teamName.Text = !string.IsNullOrWhiteSpace(settings?.Name)
            ? settings.Name.Trim()
            : original?.Nom ?? "";

        FillMusicBox(goalBox, "buts", settings?.GoalPath, hasOriginal);
        FillMusicBox(warmupBox, "warmup", settings?.WarmupPath, hasOriginal);
    }

    private void FillMusicBox(ComboBox box, string category, string? chosenPath, bool hasOriginal)
    {
        box.Items.Clear();
        box.Items.Add(new SongOption(
            null,
            hasOriginal ? "Musique d'origine de cette équipe" : "Aucune musique assignée"));

        foreach (var song in library.Tracks(category))
            box.Items.Add(new SongOption(song.Path, song.Title));

        if (!string.IsNullOrEmpty(chosenPath) &&
            !box.Items.Cast<SongOption>().Any(item =>
                string.Equals(item.Path, chosenPath, StringComparison.OrdinalIgnoreCase)))
        {
            box.Items.Add(new SongOption(
                chosenPath,
                $"Fichier introuvable : {Path.GetFileName(chosenPath)}"));
        }

        box.SelectedItem = box.Items.Cast<SongOption>()
            .FirstOrDefault(item =>
                string.Equals(item.Path, chosenPath, StringComparison.OrdinalIgnoreCase))
            ?? box.Items[0];
    }

    private void AddTeam()
    {
        var name = AskText("Ajouter une équipe", "");
        if (name is null) return;

        if (TeamEntries().Any(item =>
                string.Equals(item.Name, name, StringComparison.CurrentCultureIgnoreCase)))
        {
            MessageBox.Show(this, "Une équipe porte déjà ce nom.");
            return;
        }

        var key = "custom:" + Guid.NewGuid().ToString("N");
        library.Preferences.Teams[key] = new TeamPreference
        {
            Name = name,
            IsCustom = true,
            Deleted = false
        };

        library.Save();
        ReloadTeamOptions(key);
        LoadTeam();
    }

    private void DeleteTeam()
    {
        if (teamBox.SelectedItem is not Option selected) return;

        if (MessageBox.Show(
                this,
                $"Supprimer l'équipe « {selected.Label} » de l'application ?",
                "Supprimer l'équipe",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        if (originalTeams.ContainsKey(selected.Key))
        {
            if (!library.Preferences.Teams.TryGetValue(selected.Key, out var settings))
            {
                settings = new TeamPreference();
                library.Preferences.Teams[selected.Key] = settings;
            }

            settings.Deleted = true;
        }
        else
        {
            library.Preferences.Teams.Remove(selected.Key);
        }

        library.Save();
        ReloadTeamOptions();
    }

    private void SaveTeam()
    {
        if (teamBox.SelectedItem is not Option team) return;

        var name = teamName.Text.Trim();
        if (name.Length == 0)
        {
            MessageBox.Show(this, "Entrez un nom d'équipe.");
            return;
        }

        if (TeamEntries().Any(item =>
                !string.Equals(item.Key, team.Key, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.Name, name, StringComparison.CurrentCultureIgnoreCase)))
        {
            MessageBox.Show(this, "Deux équipes ne peuvent pas porter le même nom.");
            return;
        }

        if (!library.Preferences.Teams.TryGetValue(team.Key, out var settings))
        {
            settings = new TeamPreference
            {
                IsCustom = !originalTeams.ContainsKey(team.Key)
            };
            library.Preferences.Teams[team.Key] = settings;
        }

        settings.Name = name;
        settings.GoalPath = (goalBox.SelectedItem as SongOption)?.Path;
        settings.WarmupPath = (warmupBox.SelectedItem as SongOption)?.Path;
        settings.Deleted = false;

        library.Save();
        ReloadTeamOptions(team.Key);

        MessageBox.Show(
            this,
            $"Configuration de « {name} » enregistrée.",
            "Équipes",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private IEnumerable<(string Key, string Name)> TeamEntries()
    {
        foreach (var (key, original) in originalTeams)
        {
            library.Preferences.Teams.TryGetValue(key, out var settings);
            if (settings?.Deleted == true) continue;

            yield return (
                key,
                string.IsNullOrWhiteSpace(settings?.Name) ? original.Nom : settings.Name.Trim());
        }

        foreach (var pair in library.Preferences.Teams.Where(pair => pair.Value.IsCustom && !pair.Value.Deleted))
        {
            if (!string.IsNullOrWhiteSpace(pair.Value.Name))
                yield return (pair.Key, pair.Value.Name.Trim());
        }
    }

    private string? AskText(string caption, string current)
    {
        using var dialog = new Form
        {
            Text = caption,
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(480, 145),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var box = new TextBox
        {
            Left = 16,
            Top = 18,
            Width = 445,
            Text = current,
            MaxLength = 150
        };

        var ok = new Button
        {
            Text = "Enregistrer",
            Left = 250,
            Top = 75,
            Width = 100,
            DialogResult = DialogResult.OK
        };

        var cancel = new Button
        {
            Text = "Annuler",
            Left = 362,
            Top = 75,
            Width = 100,
            DialogResult = DialogResult.Cancel
        };

        dialog.Controls.AddRange(new Control[] { box, ok, cancel });
        dialog.AcceptButton = ok;
        dialog.CancelButton = cancel;

        if (dialog.ShowDialog(this) != DialogResult.OK) return null;

        var value = box.Text.Trim();
        return value.Length == 0 ? null : value;
    }

    private void Perform(Action action)
    {
        try
        {
            action();
        }
        catch (Exception error)
        {
            MessageBox.Show(
                this,
                $"Modification impossible : {error.Message}",
                "Personnalisation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async Task PerformAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception error)
        {
            MessageBox.Show(
                this,
                $"Modification impossible : {error.Message}",
                "Personnalisation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
