namespace CollegeManagement
{
    partial class Login
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components     = new System.ComponentModel.Container();
            this.leftPanel      = new System.Windows.Forms.Panel();
            this.secureLabel    = new System.Windows.Forms.Label();
            this.rightPanel     = new System.Windows.Forms.Panel();
            this.signInTitle    = new System.Windows.Forms.Label();
            this.signInSub      = new System.Windows.Forms.Label();
            this.lblUsername    = new System.Windows.Forms.Label();
            this.uNameBox       = new System.Windows.Forms.Panel();
            this.uName          = new System.Windows.Forms.TextBox();
            this.lblPassword    = new System.Windows.Forms.Label();
            this.passBox        = new System.Windows.Forms.Panel();
            this.password       = new System.Windows.Forms.TextBox();
            this.showPassBtn    = new System.Windows.Forms.Label();
            this.ipSection      = new System.Windows.Forms.Panel();
            this.lblHost        = new System.Windows.Forms.Label();
            this.ipInputRow     = new System.Windows.Forms.Panel();
            this.IP_a           = new System.Windows.Forms.TextBox();
            this.dot1           = new System.Windows.Forms.Label();
            this.IP_b           = new System.Windows.Forms.TextBox();
            this.dot2           = new System.Windows.Forms.Label();
            this.IP_c           = new System.Windows.Forms.TextBox();
            this.dot3           = new System.Windows.Forms.Label();
            this.IP_d           = new System.Windows.Forms.TextBox();
            this.ipHint         = new System.Windows.Forms.Label();
            this.loginButton    = new System.Windows.Forms.Button();
            this.incorrect      = new System.Windows.Forms.Label();
            this.footerLabel    = new System.Windows.Forms.Label();
            this.closeBtn       = new System.Windows.Forms.Label();
            this.minimizeBtn    = new System.Windows.Forms.Label();
            this.timer1         = new System.Windows.Forms.Timer(this.components);

            this.leftPanel.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.uNameBox.SuspendLayout();
            this.passBox.SuspendLayout();
            this.ipSection.SuspendLayout();
            this.ipInputRow.SuspendLayout();
            this.SuspendLayout();

            // ── leftPanel ──────────────────────────────────────────────────
            this.leftPanel.Dock     = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Name     = "leftPanel";
            this.leftPanel.Size     = new System.Drawing.Size(400, 640);
            this.leftPanel.TabIndex = 0;
            this.leftPanel.Paint   += new System.Windows.Forms.PaintEventHandler(this.leftPanel_Paint);
            this.leftPanel.Controls.Add(this.secureLabel);
            this.leftPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.leftPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);
            this.leftPanel.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseUp);

            // ── secureLabel (bottom-left badge) ────────────────────────────
            this.secureLabel.AutoSize  = false;
            this.secureLabel.BackColor = System.Drawing.Color.Transparent;
            this.secureLabel.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.secureLabel.ForeColor = System.Drawing.Color.FromArgb(200, 200, 220);
            this.secureLabel.Location  = new System.Drawing.Point(18, 596);
            this.secureLabel.Name      = "secureLabel";
            this.secureLabel.Size      = new System.Drawing.Size(130, 28);
            this.secureLabel.Text      = "🔒  Secure Login";

            // ── rightPanel ─────────────────────────────────────────────────
            this.rightPanel.BackColor = System.Drawing.Color.FromArgb(22, 22, 35);
            this.rightPanel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Name      = "rightPanel";
            this.rightPanel.TabIndex  = 1;
            this.rightPanel.Controls.Add(this.closeBtn);
            this.rightPanel.Controls.Add(this.minimizeBtn);
            this.rightPanel.Controls.Add(this.signInTitle);
            this.rightPanel.Controls.Add(this.signInSub);
            this.rightPanel.Controls.Add(this.lblUsername);
            this.rightPanel.Controls.Add(this.uNameBox);
            this.rightPanel.Controls.Add(this.lblPassword);
            this.rightPanel.Controls.Add(this.passBox);
            this.rightPanel.Controls.Add(this.ipSection);
            this.rightPanel.Controls.Add(this.loginButton);
            this.rightPanel.Controls.Add(this.incorrect);
            this.rightPanel.Controls.Add(this.footerLabel);
            this.rightPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.rightPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);
            this.rightPanel.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseUp);

            // ── Window control buttons ─────────────────────────────────────
            this.closeBtn.AutoSize  = true;
            this.closeBtn.BackColor = System.Drawing.Color.Transparent;
            this.closeBtn.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.closeBtn.Font      = new System.Drawing.Font("Segoe UI", 11F);
            this.closeBtn.ForeColor = System.Drawing.Color.FromArgb(160, 160, 180);
            this.closeBtn.Location  = new System.Drawing.Point(530, 14);
            this.closeBtn.Name      = "closeBtn";
            this.closeBtn.Text      = "✕";
            this.closeBtn.Click    += new System.EventHandler(this.closeBtn_Click);
            this.closeBtn.MouseEnter += new System.EventHandler(this.closeBtn_MouseEnter);
            this.closeBtn.MouseLeave += new System.EventHandler(this.closeBtn_MouseLeave);

            this.minimizeBtn.AutoSize  = true;
            this.minimizeBtn.BackColor = System.Drawing.Color.Transparent;
            this.minimizeBtn.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.minimizeBtn.Font      = new System.Drawing.Font("Segoe UI", 11F);
            this.minimizeBtn.ForeColor = System.Drawing.Color.FromArgb(160, 160, 180);
            this.minimizeBtn.Location  = new System.Drawing.Point(500, 14);
            this.minimizeBtn.Name      = "minimizeBtn";
            this.minimizeBtn.Text      = "─";
            this.minimizeBtn.Click    += new System.EventHandler(this.minimizeBtn_Click);
            this.minimizeBtn.MouseEnter += new System.EventHandler(this.minimizeBtn_MouseEnter);
            this.minimizeBtn.MouseLeave += new System.EventHandler(this.minimizeBtn_MouseLeave);

            // ── signInTitle ────────────────────────────────────────────────
            this.signInTitle.AutoSize  = false;
            this.signInTitle.BackColor = System.Drawing.Color.Transparent;
            this.signInTitle.Font      = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.signInTitle.ForeColor = System.Drawing.Color.White;
            this.signInTitle.Location  = new System.Drawing.Point(50, 70);
            this.signInTitle.Name      = "signInTitle";
            this.signInTitle.Size      = new System.Drawing.Size(500, 50);
            this.signInTitle.Text      = "Sign in";

            // ── signInSub ──────────────────────────────────────────────────
            this.signInSub.AutoSize  = false;
            this.signInSub.BackColor = System.Drawing.Color.Transparent;
            this.signInSub.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.signInSub.ForeColor = System.Drawing.Color.FromArgb(140, 140, 165);
            this.signInSub.Location  = new System.Drawing.Point(50, 120);
            this.signInSub.Name      = "signInSub";
            this.signInSub.Size      = new System.Drawing.Size(460, 22);
            this.signInSub.Text      = "Enter your credentials to access your account";

            // ── lblUsername ────────────────────────────────────────────────
            this.lblUsername.AutoSize  = true;
            this.lblUsername.BackColor = System.Drawing.Color.Transparent;
            this.lblUsername.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(220, 220, 235);
            this.lblUsername.Location  = new System.Drawing.Point(50, 165);
            this.lblUsername.Name      = "lblUsername";
            this.lblUsername.Text      = "Username";

            // ── uNameBox (dark rounded input container) ────────────────────
            this.uNameBox.BackColor = System.Drawing.Color.FromArgb(32, 32, 48);
            this.uNameBox.Location  = new System.Drawing.Point(50, 188);
            this.uNameBox.Name      = "uNameBox";
            this.uNameBox.Size      = new System.Drawing.Size(460, 46);
            this.uNameBox.TabIndex  = 0;
            this.uNameBox.Paint    += new System.Windows.Forms.PaintEventHandler(this.InputBox_Paint);
            this.uNameBox.Controls.Add(this.uName);

            this.uName.BackColor    = System.Drawing.Color.FromArgb(32, 32, 48);
            this.uName.BorderStyle  = System.Windows.Forms.BorderStyle.None;
            this.uName.Font         = new System.Drawing.Font("Segoe UI", 10.5F);
            this.uName.ForeColor    = System.Drawing.Color.White;
            this.uName.Location     = new System.Drawing.Point(42, 12);
            this.uName.Name         = "uName";
            this.uName.Size         = new System.Drawing.Size(408, 24);
            this.uName.TabIndex     = 1;
            this.uName.Enter       += new System.EventHandler(this.Input_Enter);
            this.uName.Leave       += new System.EventHandler(this.Input_Leave);

            // ── lblPassword ────────────────────────────────────────────────
            this.lblPassword.AutoSize  = true;
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(220, 220, 235);
            this.lblPassword.Location  = new System.Drawing.Point(50, 250);
            this.lblPassword.Name      = "lblPassword";
            this.lblPassword.Text      = "Password";

            // ── passBox ────────────────────────────────────────────────────
            this.passBox.BackColor = System.Drawing.Color.FromArgb(32, 32, 48);
            this.passBox.Location  = new System.Drawing.Point(50, 273);
            this.passBox.Name      = "passBox";
            this.passBox.Size      = new System.Drawing.Size(460, 46);
            this.passBox.TabIndex  = 0;
            this.passBox.Paint    += new System.Windows.Forms.PaintEventHandler(this.InputBox_Paint);
            this.passBox.Controls.Add(this.showPassBtn);
            this.passBox.Controls.Add(this.password);

            this.password.BackColor    = System.Drawing.Color.FromArgb(32, 32, 48);
            this.password.BorderStyle  = System.Windows.Forms.BorderStyle.None;
            this.password.Font         = new System.Drawing.Font("Segoe UI", 10.5F);
            this.password.ForeColor    = System.Drawing.Color.White;
            this.password.Location     = new System.Drawing.Point(42, 12);
            this.password.Name         = "password";
            this.password.PasswordChar = '●';
            this.password.Size         = new System.Drawing.Size(370, 24);
            this.password.TabIndex     = 2;
            this.password.Enter       += new System.EventHandler(this.Input_Enter);
            this.password.Leave       += new System.EventHandler(this.Input_Leave);
            this.password.KeyDown     += new System.Windows.Forms.KeyEventHandler(this.password_KeyDown);

            this.showPassBtn.AutoSize  = true;
            this.showPassBtn.BackColor = System.Drawing.Color.Transparent;
            this.showPassBtn.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.showPassBtn.Font      = new System.Drawing.Font("Segoe UI", 13F);
            this.showPassBtn.ForeColor = System.Drawing.Color.FromArgb(120, 120, 150);
            this.showPassBtn.Location  = new System.Drawing.Point(422, 10);
            this.showPassBtn.Name      = "showPassBtn";
            this.showPassBtn.Text      = "👁";
            this.showPassBtn.Click    += new System.EventHandler(this.showPassBtn_Click);

            // ── ipSection ──────────────────────────────────────────────────
            this.ipSection.BackColor = System.Drawing.Color.Transparent;
            this.ipSection.Location  = new System.Drawing.Point(50, 335);
            this.ipSection.Name      = "ipSection";
            this.ipSection.Size      = new System.Drawing.Size(460, 75);
            this.ipSection.TabIndex  = 0;
            this.ipSection.Controls.Add(this.lblHost);
            this.ipSection.Controls.Add(this.ipInputRow);
            this.ipSection.Controls.Add(this.ipHint);

            this.lblHost.AutoSize  = true;
            this.lblHost.BackColor = System.Drawing.Color.Transparent;
            this.lblHost.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblHost.ForeColor = System.Drawing.Color.FromArgb(220, 220, 235);
            this.lblHost.Location  = new System.Drawing.Point(0, 0);
            this.lblHost.Name      = "lblHost";
            this.lblHost.Text      = "Database Host  (optional)";

            this.ipInputRow.BackColor = System.Drawing.Color.Transparent;
            this.ipInputRow.Location  = new System.Drawing.Point(0, 22);
            this.ipInputRow.Name      = "ipInputRow";
            this.ipInputRow.Size      = new System.Drawing.Size(340, 30);
            this.ipInputRow.TabIndex  = 0;
            this.ipInputRow.Controls.Add(this.IP_a);
            this.ipInputRow.Controls.Add(this.dot1);
            this.ipInputRow.Controls.Add(this.IP_b);
            this.ipInputRow.Controls.Add(this.dot2);
            this.ipInputRow.Controls.Add(this.IP_c);
            this.ipInputRow.Controls.Add(this.dot3);
            this.ipInputRow.Controls.Add(this.IP_d);

            System.Drawing.Font octetFont = new System.Drawing.Font("Segoe UI", 9.5F);
            System.Drawing.Color octetBg  = System.Drawing.Color.FromArgb(32, 32, 48);
            System.Drawing.Color octetFg  = System.Drawing.Color.White;

            this.IP_a.BackColor  = octetBg; this.IP_a.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IP_a.Font = octetFont; this.IP_a.ForeColor = octetFg;
            this.IP_a.Location = new System.Drawing.Point(0, 2); this.IP_a.MaxLength = 3;
            this.IP_a.Name = "IP_a"; this.IP_a.Size = new System.Drawing.Size(52, 24);
            this.IP_a.TabIndex = 3; this.IP_a.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.dot1.AutoSize = true; this.dot1.BackColor = System.Drawing.Color.Transparent;
            this.dot1.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.dot1.ForeColor = System.Drawing.Color.FromArgb(140, 100, 220);
            this.dot1.Location = new System.Drawing.Point(55, 1); this.dot1.Text = "·";

            this.IP_b.BackColor = octetBg; this.IP_b.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IP_b.Font = octetFont; this.IP_b.ForeColor = octetFg;
            this.IP_b.Location = new System.Drawing.Point(68, 2); this.IP_b.MaxLength = 3;
            this.IP_b.Name = "IP_b"; this.IP_b.Size = new System.Drawing.Size(52, 24);
            this.IP_b.TabIndex = 4; this.IP_b.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.dot2.AutoSize = true; this.dot2.BackColor = System.Drawing.Color.Transparent;
            this.dot2.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.dot2.ForeColor = System.Drawing.Color.FromArgb(140, 100, 220);
            this.dot2.Location = new System.Drawing.Point(123, 1); this.dot2.Text = "·";

            this.IP_c.BackColor = octetBg; this.IP_c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IP_c.Font = octetFont; this.IP_c.ForeColor = octetFg;
            this.IP_c.Location = new System.Drawing.Point(136, 2); this.IP_c.MaxLength = 3;
            this.IP_c.Name = "IP_c"; this.IP_c.Size = new System.Drawing.Size(52, 24);
            this.IP_c.TabIndex = 5; this.IP_c.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.dot3.AutoSize = true; this.dot3.BackColor = System.Drawing.Color.Transparent;
            this.dot3.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.dot3.ForeColor = System.Drawing.Color.FromArgb(140, 100, 220);
            this.dot3.Location = new System.Drawing.Point(191, 1); this.dot3.Text = "·";

            this.IP_d.BackColor = octetBg; this.IP_d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IP_d.Font = octetFont; this.IP_d.ForeColor = octetFg;
            this.IP_d.Location = new System.Drawing.Point(204, 2); this.IP_d.MaxLength = 3;
            this.IP_d.Name = "IP_d"; this.IP_d.Size = new System.Drawing.Size(52, 24);
            this.IP_d.TabIndex = 6; this.IP_d.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.ipHint.AutoSize  = true;
            this.ipHint.BackColor = System.Drawing.Color.Transparent;
            this.ipHint.Font      = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Italic);
            this.ipHint.ForeColor = System.Drawing.Color.FromArgb(90, 90, 115);
            this.ipHint.Location  = new System.Drawing.Point(0, 56);
            this.ipHint.Name      = "ipHint";
            this.ipHint.Text      = "Leave blank to use 127.0.0.1 (localhost)";

            // ── loginButton ────────────────────────────────────────────────
            this.loginButton.BackColor              = System.Drawing.Color.FromArgb(120, 60, 220);
            this.loginButton.Cursor                 = System.Windows.Forms.Cursors.Hand;
            this.loginButton.FlatAppearance.BorderSize = 0;
            this.loginButton.FlatStyle              = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.Font                   = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.loginButton.ForeColor              = System.Drawing.Color.White;
            this.loginButton.Location               = new System.Drawing.Point(50, 425);
            this.loginButton.Name                   = "loginButton";
            this.loginButton.Size                   = new System.Drawing.Size(460, 48);
            this.loginButton.TabIndex               = 7;
            this.loginButton.Text                   = "Sign in";
            this.loginButton.UseVisualStyleBackColor = false;
            this.loginButton.Click      += new System.EventHandler(this.loginButton_Click);
            this.loginButton.MouseEnter += new System.EventHandler(this.loginButton_MouseEnter);
            this.loginButton.MouseLeave += new System.EventHandler(this.loginButton_MouseLeave);
            this.loginButton.Paint      += new System.Windows.Forms.PaintEventHandler(this.loginButton_Paint);

            // ── incorrect ──────────────────────────────────────────────────
            this.incorrect.AutoSize  = false;
            this.incorrect.BackColor = System.Drawing.Color.Transparent;
            this.incorrect.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            this.incorrect.ForeColor = System.Drawing.Color.FromArgb(255, 80, 100);
            this.incorrect.Location  = new System.Drawing.Point(50, 480);
            this.incorrect.Name      = "incorrect";
            this.incorrect.Size      = new System.Drawing.Size(460, 20);
            this.incorrect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── footerLabel ────────────────────────────────────────────────
            this.footerLabel.AutoSize  = false;
            this.footerLabel.BackColor = System.Drawing.Color.Transparent;
            this.footerLabel.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.footerLabel.Font      = new System.Drawing.Font("Segoe UI", 7.5F);
            this.footerLabel.ForeColor = System.Drawing.Color.FromArgb(70, 70, 95);
            this.footerLabel.Height    = 26;
            this.footerLabel.Name      = "footerLabel";
            this.footerLabel.Text      = "© 2024 Wukro St. Mary College Management System";
            this.footerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── Form ───────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(22, 22, 35);
            this.ClientSize          = new System.Drawing.Size(960, 640);
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.leftPanel);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize         = new System.Drawing.Size(860, 580);
            this.Name                = "Login";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "Login";

            this.leftPanel.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.rightPanel.PerformLayout();
            this.uNameBox.ResumeLayout(false);
            this.uNameBox.PerformLayout();
            this.passBox.ResumeLayout(false);
            this.passBox.PerformLayout();
            this.ipSection.ResumeLayout(false);
            this.ipSection.PerformLayout();
            this.ipInputRow.ResumeLayout(false);
            this.ipInputRow.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Panel      leftPanel;
        private System.Windows.Forms.Label      secureLabel;
        private System.Windows.Forms.Panel      rightPanel;
        private System.Windows.Forms.Label      closeBtn;
        private System.Windows.Forms.Label      minimizeBtn;
        private System.Windows.Forms.Label      signInTitle;
        private System.Windows.Forms.Label      signInSub;
        private System.Windows.Forms.Label      lblUsername;
        private System.Windows.Forms.Panel      uNameBox;
        private System.Windows.Forms.TextBox    uName;
        private System.Windows.Forms.Label      lblPassword;
        private System.Windows.Forms.Panel      passBox;
        private System.Windows.Forms.TextBox    password;
        private System.Windows.Forms.Label      showPassBtn;
        private System.Windows.Forms.Panel      ipSection;
        private System.Windows.Forms.Label      lblHost;
        private System.Windows.Forms.Panel      ipInputRow;
        private System.Windows.Forms.TextBox    IP_a;
        private System.Windows.Forms.Label      dot1;
        private System.Windows.Forms.TextBox    IP_b;
        private System.Windows.Forms.Label      dot2;
        private System.Windows.Forms.TextBox    IP_c;
        private System.Windows.Forms.Label      dot3;
        private System.Windows.Forms.TextBox    IP_d;
        private System.Windows.Forms.Label      ipHint;
        private System.Windows.Forms.Button     loginButton;
        private System.Windows.Forms.Label      incorrect;
        private System.Windows.Forms.Label      footerLabel;
        private System.Windows.Forms.Timer      timer1;
    }
}
