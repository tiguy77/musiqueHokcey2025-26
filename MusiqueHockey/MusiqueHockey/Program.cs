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
                MessageBox.Show(
                    "La variable MUSIQUE_HOCKEY_DATABASE_URL n'est pas configurée. Consultez le README.",
                    "Configuration requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var database = new Services.DatabaseService(connectionString);
            try
            {
                await database.InitializeAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Connexion au service cloud impossible.\n\n{exception.Message}",
                    "Service indisponible", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
