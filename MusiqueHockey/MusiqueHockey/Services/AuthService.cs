using System.Security.Cryptography;
using MusiqueHockey.Models;

namespace MusiqueHockey.Services;

public sealed class AuthService
{
    private const int Iterations = 210_000;
    private readonly DatabaseService database;

    public AuthService(DatabaseService database) => this.database = database;

    public async Task<AppUser?> SignInAsync(string email, string password)
    {
        await using var connection = database.CreateConnection();
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, email, display_name, password_hash FROM app_users WHERE lower(email) = lower($1)";
        command.Parameters.AddWithValue(email.Trim());
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync() || !VerifyPassword(password, reader.GetString(3))) return null;
        return new AppUser(reader.GetGuid(0), reader.GetString(1), reader.GetString(2));
    }

    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return $"pbkdf2-sha256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string encoded)
    {
        var parts = encoded.Split('$');
        if (parts.Length != 4 || parts[0] != "pbkdf2-sha256" || !int.TryParse(parts[1], out var iterations)) return false;
        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}
