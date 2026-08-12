namespace MusiqueHockey.Models;

public sealed record CloudTrack(Guid Id, string Title, string Category, Uri DownloadUrl, string FileName);
