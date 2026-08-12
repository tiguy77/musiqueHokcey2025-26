using Npgsql;

namespace MusiqueHockey.Services;

public sealed class DatabaseService
{
    public string ConnectionString { get; }

    public DatabaseService(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString) { SslMode = SslMode.Require };
        ConnectionString = builder.ConnectionString;
    }

    public NpgsqlConnection CreateConnection() => new(ConnectionString);

    public async Task InitializeAsync()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS app_users (
                id uuid PRIMARY KEY,
                email text NOT NULL UNIQUE,
                display_name text NOT NULL,
                password_hash text NOT NULL,
                created_at timestamptz NOT NULL DEFAULT now()
            );
            CREATE TABLE IF NOT EXISTS music_tracks (
                id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
                title text NOT NULL,
                category text NOT NULL CHECK (category IN ('all', 'warmup', 'buts', 'PPLocal', 'PPVis', 'entracte')),
                download_url text NOT NULL,
                file_name text NOT NULL,
                is_active boolean NOT NULL DEFAULT true,
                created_at timestamptz NOT NULL DEFAULT now()
            );
            """;
        await command.ExecuteNonQueryAsync();
    }
}
