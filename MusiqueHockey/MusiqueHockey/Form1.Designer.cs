namespace MusiqueHockey
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            PlayMusiqueBtn = new Button();
            LocalBox = new ComboBox();
            LocalButBtn = new Button();
            VisiteurButBtn = new Button();
            EntracteBtn = new Button();
            PenLocalBtn = new Button();
            PenVisBtn = new Button();
            VisiteurBox = new ComboBox();
            WarmUpbtn = new Button();
            SuspendLayout();
            // 
            // PlayMusiqueBtn
            // 
            PlayMusiqueBtn.Location = new Point(320, 284);
            PlayMusiqueBtn.Margin = new Padding(4);
            PlayMusiqueBtn.Name = "PlayMusiqueBtn";
            PlayMusiqueBtn.Size = new Size(269, 126);
            PlayMusiqueBtn.TabIndex = 0;
            PlayMusiqueBtn.Text = "Play Musique";
            PlayMusiqueBtn.UseVisualStyleBackColor = true;
            PlayMusiqueBtn.Click += PlayMusiqueBtn_Click;
            // 
            // LocalBox
            // 
            LocalBox.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LocalBox.ForeColor = Color.Black;
            LocalBox.FormattingEnabled = true;
            LocalBox.Items.AddRange(new object[] { "Assurance Séguin", "**Seguin Spécial", "Coffrages Thibault", "Concept Phénix", "Fauteux Mini-Moteur", "Hubby Mike", "Laporte&Fils", "Orlando", "Vérisécur" });
            LocalBox.Location = new Point(29, 49);
            LocalBox.Margin = new Padding(4);
            LocalBox.Name = "LocalBox";
            LocalBox.Size = new Size(210, 28);
            LocalBox.TabIndex = 3;
            LocalBox.Text = "Local";
            // 
            // LocalButBtn
            // 
            LocalButBtn.Location = new Point(29, 128);
            LocalButBtn.Margin = new Padding(4);
            LocalButBtn.Name = "LocalButBtn";
            LocalButBtn.Size = new Size(210, 46);
            LocalButBtn.TabIndex = 4;
            LocalButBtn.Text = "But !!";
            LocalButBtn.UseVisualStyleBackColor = true;
            LocalButBtn.Click += LocalButBtn_Click;
            // 
            // VisiteurButBtn
            // 
            VisiteurButBtn.Location = new Point(680, 128);
            VisiteurButBtn.Margin = new Padding(4);
            VisiteurButBtn.Name = "VisiteurButBtn";
            VisiteurButBtn.Size = new Size(210, 46);
            VisiteurButBtn.TabIndex = 5;
            VisiteurButBtn.Text = "But !!";
            VisiteurButBtn.UseVisualStyleBackColor = true;
            VisiteurButBtn.Click += VisiteurButBtn_Click;
            // 
            // EntracteBtn
            // 
            EntracteBtn.Location = new Point(349, 210);
            EntracteBtn.Margin = new Padding(4);
            EntracteBtn.Name = "EntracteBtn";
            EntracteBtn.Size = new Size(210, 46);
            EntracteBtn.TabIndex = 6;
            EntracteBtn.Text = "Entracte";
            EntracteBtn.UseVisualStyleBackColor = true;
            EntracteBtn.Click += EntracteBtn_Click;
            // 
            // PenLocalBtn
            // 
            PenLocalBtn.Location = new Point(29, 210);
            PenLocalBtn.Margin = new Padding(4);
            PenLocalBtn.Name = "PenLocalBtn";
            PenLocalBtn.Size = new Size(210, 46);
            PenLocalBtn.TabIndex = 9;
            PenLocalBtn.Text = "Pénalité";
            PenLocalBtn.UseVisualStyleBackColor = true;
            PenLocalBtn.Click += PenLocalBtn_Click;
            // 
            // PenVisBtn
            // 
            PenVisBtn.Location = new Point(680, 210);
            PenVisBtn.Margin = new Padding(4);
            PenVisBtn.Name = "PenVisBtn";
            PenVisBtn.Size = new Size(210, 46);
            PenVisBtn.TabIndex = 10;
            PenVisBtn.Text = "Pénalité";
            PenVisBtn.UseVisualStyleBackColor = true;
            PenVisBtn.Click += PenVisBtn_Click;
            // 
            // VisiteurBox
            // 
            VisiteurBox.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            VisiteurBox.ForeColor = Color.Black;
            VisiteurBox.FormattingEnabled = true;
            VisiteurBox.Items.AddRange(new object[] { "Assurance Séguin", "Coffrages Thibault", "Concept Phénix", "Fauteux Mini-Moteur", "Hubby Mike", "Laporte&Fils", "Orlando", "Vérisécur" });
            VisiteurBox.Location = new Point(680, 49);
            VisiteurBox.Margin = new Padding(4);
            VisiteurBox.Name = "VisiteurBox";
            VisiteurBox.Size = new Size(210, 28);
            VisiteurBox.TabIndex = 11;
            VisiteurBox.Text = "Visiteur";
            // 
            // WarmUpbtn
            // 
            WarmUpbtn.Location = new Point(349, 72);
            WarmUpbtn.Margin = new Padding(4);
            WarmUpbtn.Name = "WarmUpbtn";
            WarmUpbtn.Size = new Size(210, 46);
            WarmUpbtn.TabIndex = 12;
            WarmUpbtn.Text = "Warmup";
            WarmUpbtn.UseVisualStyleBackColor = true;
            WarmUpbtn.Click += WarmUpBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(909, 437);
            Controls.Add(WarmUpbtn);
            Controls.Add(VisiteurBox);
            Controls.Add(PenVisBtn);
            Controls.Add(PenLocalBtn);
            Controls.Add(EntracteBtn);
            Controls.Add(VisiteurButBtn);
            Controls.Add(LocalButBtn);
            Controls.Add(LocalBox);
            Controls.Add(PlayMusiqueBtn);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Musique";
            ResumeLayout(false);

        }

        private System.Windows.Forms.Button PlayMusiqueBtn;
        private System.Windows.Forms.ComboBox LocalBox;
        private System.Windows.Forms.Button LocalButBtn;
        private System.Windows.Forms.Button VisiteurButBtn;
        private System.Windows.Forms.Button EntracteBtn;
        private System.Windows.Forms.Button PenLocalBtn;
        private System.Windows.Forms.Button PenVisBtn;
        private ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private ComboBox VisiteurBox;
        private Button WarmUpbtn;
    }
}
