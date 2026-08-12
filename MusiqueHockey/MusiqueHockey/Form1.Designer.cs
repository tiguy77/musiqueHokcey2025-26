namespace MusiqueHockey;

partial class Form1
{
    private System.ComponentModel.IContainer? components;
    private Button PlayMusiqueBtn = null!, LocalButBtn = null!, VisiteurButBtn = null!, EntracteBtn = null!, PenLocalBtn = null!, PenVisBtn = null!, WarmUpbtn = null!, SyncButton = null!;
    private ComboBox LocalBox = null!, VisiteurBox = null!;
    private Label UserLabel = null!, StatusLabel = null!;

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        Text = "Aréna DJ — Console musicale"; StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1120, 700); MinimumSize = new Size(980, 650); BackColor = Color.FromArgb(8, 15, 30);
        ForeColor = Color.White; Font = new Font("Segoe UI", 10F);
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(34), RowCount = 4, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 90)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 230));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65)); header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        header.Controls.Add(new Label { Text = "🏒  ARÉNA DJ\n     Console de match", Font = new Font("Segoe UI", 21, FontStyle.Bold), AutoSize = true }, 0, 0);
        UserLabel = new Label { TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, ForeColor = Color.FromArgb(148, 163, 184) };
        header.Controls.Add(UserLabel, 1, 0); root.Controls.Add(header, 0, 0);
        var teams = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3, Padding = new Padding(0, 8, 0, 8) };
        teams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35)); teams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); teams.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        LocalBox = MakeCombo(); VisiteurBox = MakeCombo();
        LocalButBtn = MakeButton("🥅  BUT LOCAL", Color.FromArgb(22, 163, 74)); VisiteurButBtn = MakeButton("🥅  BUT VISITEUR", Color.FromArgb(22, 163, 74));
        PenLocalBtn = MakeButton("⚡  Pénalité locale", Color.FromArgb(180, 83, 9)); PenVisBtn = MakeButton("⚡  Pénalité visiteur", Color.FromArgb(180, 83, 9));
        WarmUpbtn = MakeButton("🔥  Échauffement", Color.FromArgb(51, 65, 85));
        teams.Controls.Add(LocalBox, 0, 0); teams.Controls.Add(VisiteurBox, 2, 0); teams.Controls.Add(LocalButBtn, 0, 1);
        teams.Controls.Add(WarmUpbtn, 1, 1); teams.Controls.Add(VisiteurButBtn, 2, 1); teams.Controls.Add(PenLocalBtn, 0, 2); teams.Controls.Add(PenVisBtn, 2, 2); root.Controls.Add(teams, 0, 1);
        var console = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, Padding = new Padding(0, 28, 0, 28) };
        console.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); console.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); console.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        EntracteBtn = MakeButton("🎵  Entracte", Color.FromArgb(51, 65, 85)); PlayMusiqueBtn = MakeButton("▶  MUSIQUE / ARRÊT", Color.FromArgb(37, 99, 235));
        PlayMusiqueBtn.Font = new Font("Segoe UI", 16, FontStyle.Bold); SyncButton = MakeButton("☁  Télécharger les musiques", Color.FromArgb(7, 89, 133));
        console.Controls.Add(EntracteBtn, 0, 0); console.Controls.Add(PlayMusiqueBtn, 1, 0); console.Controls.Add(SyncButton, 2, 0); root.Controls.Add(console, 0, 2);
        StatusLabel = new Label { Dock = DockStyle.Fill, ForeColor = Color.FromArgb(148, 163, 184), TextAlign = ContentAlignment.MiddleLeft }; root.Controls.Add(StatusLabel, 0, 3); Controls.Add(root);
        PlayMusiqueBtn.Click += PlayMusiqueBtn_Click; LocalButBtn.Click += LocalButBtn_Click; VisiteurButBtn.Click += VisiteurButBtn_Click;
        EntracteBtn.Click += EntracteBtn_Click; PenLocalBtn.Click += PenLocalBtn_Click; PenVisBtn.Click += PenVisBtn_Click; WarmUpbtn.Click += WarmUpBtn_Click; SyncButton.Click += SyncButton_Click;
    }
    private static Button MakeButton(string text, Color color) { var button = new Button { Text = text, Dock = DockStyle.Fill, Margin = new Padding(8), BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Cursor = Cursors.Hand }; button.FlatAppearance.BorderSize = 0; return button; }
    private static ComboBox MakeCombo() => new() { Dock = DockStyle.Fill, Margin = new Padding(8), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11), BackColor = Color.FromArgb(30, 41, 59), ForeColor = Color.White };
}
