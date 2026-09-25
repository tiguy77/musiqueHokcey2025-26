namespace MusiqueHockey;

partial class Form1
{
    private System.ComponentModel.IContainer? components;
    private Button PlayMusiqueBtn = null!, LocalButBtn = null!, VisiteurButBtn = null!, EntracteBtn = null!, PenLocalBtn = null!, PenVisBtn = null!, WarmUpbtn = null!, SyncButton = null!;
    private Button FinPartieBtn = null!, ResetButton = null!, SettingsButton = null!;
    private ComboBox LocalBox = null!, VisiteurBox = null!;
    private Label UserLabel = null!, StatusLabel = null!, CurrentTrackLabel = null!, NextTrackLabel = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        Text = "Aréna DJ 2.0 — Console musicale"; StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1120, 700); MinimumSize = new Size(980, 650); BackColor = Color.FromArgb(8, 15, 30);
        ForeColor = Color.White; Font = new Font("Segoe UI", 10F);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(34), RowCount = 5, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 230));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3 };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        header.Controls.Add(new Label { Text = "🏒  ARÉNA DJ 2.0\n     Console de match", Font = new Font("Segoe UI", 21, FontStyle.Bold), AutoSize = true }, 0, 0);
        UserLabel = new Label { TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, ForeColor = Color.FromArgb(148, 163, 184) };
        SettingsButton = MakeButton("⚙  Personnaliser", Color.FromArgb(51, 65, 85));
        header.Controls.Add(UserLabel, 1, 0);
        header.Controls.Add(SettingsButton, 2, 0);
        root.Controls.Add(header, 0, 0);

        var teams = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3, Padding = new Padding(0, 8, 0, 8) };
        teams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        teams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        teams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        teams.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        teams.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        teams.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        LocalBox = MakeCombo(); VisiteurBox = MakeCombo();
        LocalButBtn = MakeButton("🥅  BUT LOCAL", Color.FromArgb(22, 163, 74));
        VisiteurButBtn = MakeButton("🥅  BUT VISITEUR", Color.FromArgb(22, 163, 74));
        PenLocalBtn = MakeButton("⚡  Pénalité locale", Color.FromArgb(180, 83, 9));
        PenVisBtn = MakeButton("⚡  Pénalité visiteur", Color.FromArgb(180, 83, 9));
        WarmUpbtn = MakeButton("🔥  Échauffement", Color.FromArgb(51, 65, 85));

        LocalButBtn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        VisiteurButBtn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        WarmUpbtn.Font = new Font("Segoe UI", 11, FontStyle.Bold);

        teams.Controls.Add(LocalBox, 0, 0); teams.Controls.Add(VisiteurBox, 2, 0);
        teams.Controls.Add(LocalButBtn, 0, 1); teams.Controls.Add(WarmUpbtn, 1, 1);
        teams.Controls.Add(VisiteurButBtn, 2, 1); teams.Controls.Add(PenLocalBtn, 0, 2);
        teams.Controls.Add(PenVisBtn, 2, 2); root.Controls.Add(teams, 0, 1);

        var console = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, Padding = new Padding(0, 20, 0, 20) };
        console.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));
        console.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));
        console.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 29));
        console.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        console.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21));
        EntracteBtn = MakeButton("🎵  Entracte", Color.FromArgb(51, 65, 85));
        FinPartieBtn = MakeButton("🏁  Fin de partie", Color.FromArgb(124, 58, 237));
        PlayMusiqueBtn = MakeButton("▶  MUSIQUE / ARRÊT", Color.FromArgb(37, 99, 235));
        PlayMusiqueBtn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        ResetButton = MakeButton("↻  RESET", Color.FromArgb(153, 27, 27));
        SyncButton = MakeButton("☁  Télécharger les musiques", Color.FromArgb(7, 89, 133));
        console.Controls.Add(EntracteBtn, 0, 0); console.Controls.Add(FinPartieBtn, 1, 0);
        console.Controls.Add(PlayMusiqueBtn, 2, 0); console.Controls.Add(ResetButton, 3, 0);
        console.Controls.Add(SyncButton, 4, 0); root.Controls.Add(console, 0, 2);

        var playback = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.FromArgb(15, 23, 42), Padding = new Padding(12, 8, 12, 8) };
        playback.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        playback.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        var currentPanel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        currentPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        currentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        currentPanel.Controls.Add(new Label { Text = "EN LECTURE", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(148, 163, 184), Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 0);
        CurrentTrackLabel = new Label { Text = "Aucune musique", Dock = DockStyle.Fill, AutoEllipsis = true, ForeColor = Color.White, Font = new Font("Segoe UI", 13, FontStyle.Bold) };
        currentPanel.Controls.Add(CurrentTrackLabel, 0, 1);
        var nextPanel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        nextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        nextPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        nextPanel.Controls.Add(new Label { Text = "MUSIQUE SUIVANTE", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(148, 163, 184), Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 0);
        NextTrackLabel = new Label { Text = "—", Dock = DockStyle.Fill, AutoEllipsis = true, ForeColor = Color.FromArgb(125, 211, 252), Font = new Font("Segoe UI", 13, FontStyle.Bold) };
        nextPanel.Controls.Add(NextTrackLabel, 0, 1);
        playback.Controls.Add(currentPanel, 0, 0); playback.Controls.Add(nextPanel, 1, 0);
        root.Controls.Add(playback, 0, 3);

        StatusLabel = new Label { Dock = DockStyle.Fill, ForeColor = Color.FromArgb(148, 163, 184), TextAlign = ContentAlignment.MiddleLeft };
        root.Controls.Add(StatusLabel, 0, 4); Controls.Add(root);

        PlayMusiqueBtn.Click += PlayMusiqueBtn_Click;
        LocalButBtn.Click += LocalButBtn_Click;
        VisiteurButBtn.Click += VisiteurButBtn_Click;
        EntracteBtn.Click += EntracteBtn_Click;
        PenLocalBtn.Click += PenLocalBtn_Click;
        PenVisBtn.Click += PenVisBtn_Click;
        WarmUpbtn.Click += WarmUpBtn_Click;
        SyncButton.Click += SyncButton_Click;
        ConfigurePlaybackExtras();
        ConfigurePersonalization();
    }

    private static Button MakeButton(string text, Color color)
    {
        var button = new Button { Text = text, Dock = DockStyle.Fill, Margin = new Padding(6), BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private static ComboBox MakeCombo() => new() { Dock = DockStyle.Fill, Margin = new Padding(8), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11), BackColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White };
}
