namespace MusiqueHockey
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var connectionString = Environment.GetEnvironmentVariable("MUSIQUE_HOCKEY_DATABASE_URL");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Application.Run(new Form1());
                return;
            }

            var database = new Services.DatabaseService(connectionString);
            try
            {
                await database.InitializeAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Le service cloud est temporairement indisponible. L'application va démarrer avec les musiques locales.\n\n{exception.Message}",
                    "Mode hors ligne", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Run(new Form1());
                return;
            }

            using var login = new LoginForm(new Services.AuthService(database));
            if (login.ShowDialog() != DialogResult.OK || login.AuthenticatedUser is null)
            {
                return;
            }

            Application.Run(new Form1(new Services.CloudMusicService(database), login.AuthenticatedUser));
        }
    }
}
