using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusiqueHockey;

public sealed class TeamPreference
{
    public string Name { get; set; } = "";
    public string? GoalPath { get; set; }
    public string? WarmupPath { get; set; }
    public bool IsCustom { get; set; }
    public bool Deleted { get; set; }
}

public sealed class ArenaPreferences
{
    public Dictionary<string, string> Titles { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, TeamPreference> Teams { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed record LibraryTrack(string Path, string Title, string Category, bool Imported)
{
    public override string ToString() => Imported ? Title : $"{Title}  (ancienne bibliothèque)";
}

public sealed class ArenaLibrary
{
    public static readonly (string Key, string Label)[] Categories =
    {
        ("all", "Musique générale"),
        ("entracte", "Entracte"),
        ("finGame", "Fin de partie"),
        ("PPLocal", "Pénalité locale"),
        ("PPvis", "Pénalité visiteur"),
        ("buts", "Musiques de but"),
        ("warmup", "Échauffement")
    };

    public static string SettingsFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ArenaDJ");
    public string ImportedRoot => Path.Combine(SettingsFolder, "musiques");
    private string SettingsPath => Path.Combine(SettingsFolder, "preferences.json");
    private readonly string legacyRoot;
    public ArenaPreferences Preferences { get; private set; } = new();

    public ArenaLibrary(string applicationRoot)
    {
        legacyRoot = Path.Combine(applicationRoot, "netX", "musiques");
    }

    public void Load()
    {
        if (File.Exists(SettingsPath))
            Preferences = JsonSerializer.Deserialize<ArenaPreferences>(File.ReadAllText(SettingsPath)) ?? new();
        Preferences.Titles ??= new(StringComparer.OrdinalIgnoreCase);
        Preferences.Teams ??= new(StringComparer.OrdinalIgnoreCase);
    }

    public void Save()
    {
        Directory.CreateDirectory(SettingsFolder);
        var temporary = SettingsPath + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(Preferences, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporary, SettingsPath, true);
    }

    public IReadOnlyList<LibraryTrack> Tracks(string category)
    {
        if (!Categories.Any(item => item.Key == category)) return Array.Empty<LibraryTrack>();
        var results = new List<LibraryTrack>();
        foreach (var (root, imported) in new[] { (legacyRoot, false), (ImportedRoot, true) })
        {
            var folder = Path.Combine(root, category);
            if (!Directory.Exists(folder)) continue;
            foreach (var file in Directory.EnumerateFiles(folder, "*.mp3", SearchOption.TopDirectoryOnly))
            {
                var title = Preferences.Titles.TryGetValue(file, out var customTitle) && !string.IsNullOrWhiteSpace(customTitle)
                    ? customTitle : Path.GetFileNameWithoutExtension(file);
                results.Add(new LibraryTrack(file, title, category, imported));
            }
        }
        return results.OrderBy(track => track.Title, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    public async Task<string> AddAsync(string source, string category, CancellationToken cancellationToken = default)
    {
        if (!Categories.Any(item => item.Key == category)) throw new ArgumentException("Catégorie inconnue.");
        if (!string.Equals(Path.GetExtension(source), ".mp3", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Sélectionnez un fichier MP3.");
        if (!File.Exists(source))
            throw new FileNotFoundException("Le fichier sélectionné n'est plus accessible.", source);

        var directory = Path.Combine(ImportedRoot, category);
        Directory.CreateDirectory(directory);

        var target = AvailableName(directory, Path.GetFileName(source));
        var temporary = target + ".importing";

        try
        {
            await using (var input = new FileStream(
                             source, FileMode.Open, FileAccess.Read, FileShare.Read,
                             128 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan))
            await using (var output = new FileStream(
                             temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                             128 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan))
            {
                await input.CopyToAsync(output, 128 * 1024, cancellationToken);
                await output.FlushAsync(cancellationToken);
            }

            // Both streams must be closed before Windows can rename the temporary file.
            File.Move(temporary, target);
            return target;
        }
        catch
        {
            try
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
            catch
            {
                // The original import error is more useful than a cleanup error.
            }
            throw;
        }
    }

    public string Move(LibraryTrack song, string category)
    {
        if (!song.Imported) throw new InvalidOperationException("Importez d'abord la musique de l'ancienne bibliothèque pour pouvoir la déplacer.");
        if (!Categories.Any(item => item.Key == category)) throw new ArgumentException("Catégorie inconnue.");
        if (song.Category == category) return song.Path;
        var directory = Path.Combine(ImportedRoot, category);
        Directory.CreateDirectory(directory);
        var target = AvailableName(directory, Path.GetFileName(song.Path));
        File.Move(song.Path, target);
        ReplacePath(song.Path, target);
        Save();
        return target;
    }

    public void Remove(LibraryTrack song)
    {
        if (!song.Imported) throw new InvalidOperationException("Seules les musiques importées peuvent être supprimées ici.");
        File.Delete(song.Path);
        ReplacePath(song.Path, null);
        Save();
    }

    private void ReplacePath(string oldPath, string? newPath)
    {
        if (Preferences.Titles.Remove(oldPath, out var title) && newPath is not null)
            Preferences.Titles[newPath] = title;
        foreach (var team in Preferences.Teams.Values)
        {
            if (string.Equals(team.GoalPath, oldPath, StringComparison.OrdinalIgnoreCase)) team.GoalPath = newPath;
            if (string.Equals(team.WarmupPath, oldPath, StringComparison.OrdinalIgnoreCase)) team.WarmupPath = newPath;
        }
    }

    private static string AvailableName(string directory, string filename)
    {
        var stem = Path.GetFileNameWithoutExtension(filename);
        var extension = Path.GetExtension(filename);
        var target = Path.Combine(directory, filename);
        for (var number = 2; File.Exists(target) || File.Exists(target + ".importing"); number++)
            target = Path.Combine(directory, $"{stem} ({number}){extension}");
        return target;
    }
}
