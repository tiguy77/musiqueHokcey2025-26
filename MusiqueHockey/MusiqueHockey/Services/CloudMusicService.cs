using MusiqueHockey.Models;

namespace MusiqueHockey.Services;

public sealed class CloudMusicService
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromMinutes(5) };
    private readonly DatabaseService database;

    public CloudMusicService(DatabaseService database) => this.database = database;

    public async Task<IReadOnlyList<CloudTrack>> GetTracksAsync()
    {
        var tracks = new List<CloudTrack>();
        await using var connection = database.CreateConnection();
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, title, category, download_url, file_name FROM music_tracks WHERE is_active ORDER BY category, title";
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (Uri.TryCreate(reader.GetString(3), UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps)
                tracks.Add(new CloudTrack(reader.GetGuid(0), reader.GetString(1), reader.GetString(2), uri, reader.GetString(4)));
        }
        return tracks;
    }

    public async Task<int> DownloadAllAsync(string musicRoot, IProgress<(int current, int total, string title)>? progress = null)
    {
        var tracks = await GetTracksAsync();
        var downloaded = 0;
        for (var index = 0; index < tracks.Count; index++)
        {
            var track = tracks[index];
            progress?.Report((index + 1, tracks.Count, track.Title));
            var directory = Path.Combine(musicRoot, track.Category);
            Directory.CreateDirectory(directory);
            var safeName = Path.GetFileName(track.FileName);
            if (string.IsNullOrWhiteSpace(safeName)) continue;
            var destination = Path.Combine(directory, safeName);
            var temporary = destination + ".download";
            await using (var source = await HttpClient.GetStreamAsync(track.DownloadUrl))
            await using (var target = File.Create(temporary))
                await source.CopyToAsync(target);
            File.Move(temporary, destination, true);
            downloaded++;
        }
        return downloaded;
    }
}
