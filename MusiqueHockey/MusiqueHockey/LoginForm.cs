using MusiqueHockey.Models;
using MusiqueHockey.Services;

namespace MusiqueHockey;

public sealed class LoginForm : Form
{
    private readonly AuthService authService;
    private readonly TextBox emailBox = new() { PlaceholderText = "nom@organisation.ca", Dock = DockStyle.Fill };
    private readonly TextBox passwordBox = new() { PlaceholderText = "Mot de passe", UseSystemPasswordChar = true, Dock = DockStyle.Fill };
    private readonly Button signInButton = new() { Text = "Se connecter", Dock = DockStyle.Fill, Height = 46 };
    private readonly Label errorLabel = new() { ForeColor = Color.FromArgb(248, 113, 113), AutoSize = true };
    public AppUser? AuthenticatedUser { get; private set; }

    public LoginForm(AuthService authService)
    {
        this.authService = authService;
        Text = "Connexion — Aréna DJ"; StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(460, 430); BackColor = Color.FromArgb(8, 15, 30); ForeColor = Color.White;
        Font = new Font("Segoe UI", 10F); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;
        var card = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(55), RowCount = 8, ColumnCount = 1 };
        foreach (var height in new[] { 58, 42, 28, 46, 16, 46, 54 }) card.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
        card.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        card.Controls.Add(new Label { Text = "🏒  ARÉNA DJ", Font = new Font("Segoe UI", 22, FontStyle.Bold), AutoSize = true }, 0, 0);
        card.Controls.Add(new Label { Text = "Connectez-vous à votre console musicale", ForeColor = Color.FromArgb(148, 163, 184), AutoSize = true }, 0, 1);
        card.Controls.Add(new Label { Text = "Courriel", AutoSize = true }, 0, 2); card.Controls.Add(emailBox, 0, 3);
        card.Controls.Add(passwordBox, 0, 5); card.Controls.Add(signInButton, 0, 6); card.Controls.Add(errorLabel, 0, 7); Controls.Add(card);
        signInButton.BackColor = Color.FromArgb(37, 99, 235); signInButton.ForeColor = Color.White; signInButton.FlatStyle = FlatStyle.Flat;
        signInButton.FlatAppearance.BorderSize = 0; signInButton.Click += SignInAsync; AcceptButton = signInButton;
    }

    private async void SignInAsync(object? sender, EventArgs e)
    {
        errorLabel.Text = ""; signInButton.Enabled = false; signInButton.Text = "Connexion…";
        try
        {
            AuthenticatedUser = await authService.SignInAsync(emailBox.Text, passwordBox.Text);
            if (AuthenticatedUser is null) { errorLabel.Text = "Courriel ou mot de passe incorrect."; return; }
            DialogResult = DialogResult.OK; Close();
        }
        catch { errorLabel.Text = "Le service est indisponible. Réessayez dans un instant."; }
        finally { signInButton.Enabled = true; signInButton.Text = "Se connecter"; }
    }
}
