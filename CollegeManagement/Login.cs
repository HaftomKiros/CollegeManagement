using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CollegeManagement
{
    public partial class Login : Form
    {
        // ── DB ─────────────────────────────────────────────────────────────
        private string priority = null, userName = null;
        private DBConnect db;
        private MySqlConnection conn;
        private MySqlCommand sqlCmd;
        private MySqlDataReader dataReader;

        // ── Drag ───────────────────────────────────────────────────────────
        private bool   _dragging;
        private Point  _dragStart;

        // ── Focused input box ──────────────────────────────────────────────
        private Panel _focusedBox = null;

        // ── Colors ─────────────────────────────────────────────────────────
        private static readonly Color BgDark      = Color.FromArgb(22,  22,  35);
        private static readonly Color InputBg     = Color.FromArgb(32,  32,  48);
        private static readonly Color InputBorder = Color.FromArgb(55,  55,  75);
        private static readonly Color InputFocus  = Color.FromArgb(140, 80, 240);
        private static readonly Color Purple      = Color.FromArgb(120, 60, 220);
        private static readonly Color PurpleHover = Color.FromArgb(100, 40, 200);
        private static readonly Color PurpleLight = Color.FromArgb(160, 90, 255);

        public Login()
        {
            InitializeComponent();
            // Rounded form corners via region
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 12, 12));
            this.Resize += (s, e) =>
                this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 12, 12));
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        // ── Left panel: scenic dark overlay + college branding ─────────────
        private void leftPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.InterpolationMode  = InterpolationMode.HighQualityBicubic;
            Rectangle rc = leftPanel.ClientRectangle;

            // Background base
            using (var bg = new LinearGradientBrush(rc,
                Color.FromArgb(20, 15, 40), Color.FromArgb(50, 30, 80),
                LinearGradientMode.ForwardDiagonal))
                g.FillRectangle(bg, rc);

            // Atmospheric glow layers
            DrawGlow(g, -60, rc.Height - 200, 320, Color.FromArgb(35, 100, 60, 180));
            DrawGlow(g, rc.Width / 2 - 100, rc.Height - 160, 280, Color.FromArgb(25, 140, 80, 220));
            DrawGlow(g, rc.Width - 80,  rc.Height - 180, 260, Color.FromArgb(20, 80, 50, 160));

            // Top-right accent circle
            DrawGlow(g, rc.Width - 120, -60, 240, Color.FromArgb(18, 160, 100, 255));

            // Semi-transparent dark overlay for text readability
            using (var overlay = new LinearGradientBrush(rc,
                Color.FromArgb(140, 10, 8, 25), Color.FromArgb(80, 15, 10, 35),
                LinearGradientMode.Vertical))
                g.FillRectangle(overlay, rc);

            // ── Hexagon logo ───────────────────────────────────────────────
            DrawHexLogo(g, rc.Width / 2, 160, 52);

            // ── College name ───────────────────────────────────────────────
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                using (var f = new Font("Segoe UI", 17F, FontStyle.Bold))
                using (var b = new SolidBrush(Color.White))
                    g.DrawString("Wukro St. Mary College", f, b, new RectangleF(0, 240, rc.Width, 40), sf);

                // Tagline
                using (var f = new Font("Segoe UI", 9.5F, FontStyle.Italic))
                using (var b = new SolidBrush(Color.FromArgb(180, 170, 200)))
                    g.DrawString("Solutions for a better tomorrow", f, b, new RectangleF(0, 282, rc.Width, 28), sf);

                // Purple divider
                int cx = rc.Width / 2;
                using (var pen = new Pen(Color.FromArgb(180, 140, 80, 240), 2f))
                    g.DrawLine(pen, cx - 30, 320, cx + 30, 320);

                // Welcome text
                using (var f = new Font("Segoe UI", 13F, FontStyle.Bold))
                using (var b = new SolidBrush(Color.White))
                    g.DrawString("Welcome Back!", f, b, new RectangleF(0, 335, rc.Width, 36), sf);

                using (var f = new Font("Segoe UI", 9F))
                using (var b = new SolidBrush(Color.FromArgb(160, 155, 175)))
                    g.DrawString("Please sign in to continue", f, b, new RectangleF(0, 372, rc.Width, 26), sf);
            }

            // Bottom border accent
            using (var pen = new Pen(Color.FromArgb(60, 140, 80, 240), 1f))
                g.DrawLine(pen, 0, rc.Height - 1, rc.Width, rc.Height - 1);
        }

        private void DrawGlow(Graphics g, int x, int y, int size, Color color)
        {
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(x, y, size, size);
                using (var pgb = new PathGradientBrush(path))
                {
                    pgb.CenterColor    = color;
                    pgb.SurroundColors = new[] { Color.Transparent };
                    g.FillPath(pgb, path);
                }
            }
        }

        private void DrawHexLogo(Graphics g, int cx, int cy, int r)
        {
            // Draw hexagon shape
            PointF[] hex = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 180 * (60 * i - 30);
                hex[i] = new PointF(cx + r * (float)Math.Cos(angle),
                                    cy + r * (float)Math.Sin(angle));
            }

            using (var path = new GraphicsPath())
            {
                path.AddPolygon(hex);

                // Gradient fill
                using (var pgb = new PathGradientBrush(path))
                {
                    pgb.CenterColor    = Color.FromArgb(200, 140, 80, 255);
                    pgb.SurroundColors = new[] { Color.FromArgb(180, 80, 40, 180) };
                    g.FillPath(pgb, path);
                }

                // Border
                using (var pen = new Pen(Color.FromArgb(220, 180, 120, 255), 2f))
                    g.DrawPath(pen, path);
            }

            // Inner "C" letter
            using (var f = new Font("Segoe UI", 22F, FontStyle.Bold))
            using (var b = new SolidBrush(Color.White))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString("C", f, b, new RectangleF(cx - r, cy - r, r * 2, r * 2), sf);
        }

        // ── Input box rounded paint ────────────────────────────────────────
        private void InputBox_Paint(object sender, PaintEventArgs e)
        {
            Panel box = (Panel)sender;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rc = new Rectangle(0, 0, box.Width - 1, box.Height - 1);

            bool focused = (_focusedBox == box);
            Color borderColor = focused ? InputFocus : InputBorder;

            using (var path = RoundedPath(rc, 8))
            {
                // Background fill
                using (var brush = new SolidBrush(InputBg))
                    g.FillPath(brush, path);

                // Border
                using (var pen = new Pen(borderColor, focused ? 1.8f : 1f))
                    g.DrawPath(pen, path);
            }

            // Draw icon on left
            string icon = (box == uNameBox) ? "👤" : "🔒";
            using (var f = new Font("Segoe UI", 11F))
            using (var b = new SolidBrush(focused ? Color.FromArgb(160, 120, 255) : Color.FromArgb(90, 90, 115)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString(icon, f, b, new RectangleF(6, 0, 34, box.Height), sf);
        }

        private GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            p.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            p.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            p.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            p.CloseFigure();
            return p;
        }

        // ── Focus events ───────────────────────────────────────────────────
        private void Input_Enter(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            _focusedBox = ctrl.Parent as Panel;
            if (_focusedBox != null) _focusedBox.Invalidate();
        }

        private void Input_Leave(object sender, EventArgs e)
        {
            Panel prev = _focusedBox;
            _focusedBox = null;
            if (prev != null) prev.Invalidate();
        }

        // ── Login button: gradient rounded paint ───────────────────────────
        private void loginButton_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rc = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);

            using (var path = RoundedPath(rc, 8))
            {
                // Gradient fill: left purple → right bright purple
                using (var lgb = new LinearGradientBrush(rc, btn.BackColor, PurpleLight, LinearGradientMode.Horizontal))
                    g.FillPath(lgb, path);

                // Top highlight
                using (var hi = new LinearGradientBrush(new Rectangle(0, 0, btn.Width, btn.Height / 2),
                    Color.FromArgb(45, 255, 255, 255), Color.Transparent, LinearGradientMode.Vertical))
                    g.FillPath(hi, path);
            }

            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var f  = new Font("Segoe UI", 11F, FontStyle.Bold))
            using (var b  = new SolidBrush(Color.White))
                g.DrawString(btn.Text, f, b, new RectangleF(0, 0, btn.Width, btn.Height), sf);
        }

        private void loginButton_MouseEnter(object sender, EventArgs e) { loginButton.BackColor = PurpleHover; }
        private void loginButton_MouseLeave(object sender, EventArgs e) { loginButton.BackColor = Purple; }

        // ── Show/hide password ─────────────────────────────────────────────
        private void showPassBtn_Click(object sender, EventArgs e)
        {
            password.PasswordChar = (password.PasswordChar == '●') ? '\0' : '●';
            showPassBtn.ForeColor = (password.PasswordChar == '\0')
                ? Color.FromArgb(160, 120, 255)
                : Color.FromArgb(120, 120, 150);
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) loginButton_Click(sender, EventArgs.Empty);
        }

        // ── Window controls ────────────────────────────────────────────────
        private void closeBtn_Click(object sender, EventArgs e)    { Application.Exit(); }
        private void minimizeBtn_Click(object sender, EventArgs e) { this.WindowState = FormWindowState.Minimized; }
        private void closeBtn_MouseEnter(object sender, EventArgs e)    { closeBtn.ForeColor    = Color.White; }
        private void closeBtn_MouseLeave(object sender, EventArgs e)    { closeBtn.ForeColor    = Color.FromArgb(160, 160, 180); }
        private void minimizeBtn_MouseEnter(object sender, EventArgs e) { minimizeBtn.ForeColor = Color.White; }
        private void minimizeBtn_MouseLeave(object sender, EventArgs e) { minimizeBtn.ForeColor = Color.FromArgb(160, 160, 180); }

        // ── Drag (borderless) ──────────────────────────────────────────────
        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { _dragging = true; _dragStart = e.Location; }
        }
        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point screen = ((Control)sender).PointToScreen(e.Location);
                Location = new Point(screen.X - _dragStart.X, screen.Y - _dragStart.Y);
            }
        }
        private void TitleBar_MouseUp(object sender, MouseEventArgs e) { _dragging = false; }

        // ── Login logic ────────────────────────────────────────────────────
        private void loginButton_Click(object sender, EventArgs e)
        {
            incorrect.Text = "";

            db = (!string.IsNullOrWhiteSpace(IP_a.Text) &&
                  !string.IsNullOrWhiteSpace(IP_b.Text) &&
                  !string.IsNullOrWhiteSpace(IP_c.Text) &&
                  !string.IsNullOrWhiteSpace(IP_d.Text))
                ? new DBConnect(IP_a.Text, IP_b.Text, IP_c.Text, IP_d.Text)
                : new DBConnect();

            int checker = 0;
            conn   = db.getConnection();
            sqlCmd = new MySqlCommand(
                "SELECT user_name, priority FROM ecc_dof_wukrostmarycollege.admins " +
                "WHERE user_name = @user AND password = @pass", conn);
            sqlCmd.Parameters.AddWithValue("@user", uName.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@pass", password.Text);

            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    ++checker;
                    priority = dataReader["priority"].ToString();
                    userName = dataReader["user_name"].ToString();
                    HomePage hp = new HomePage();
                    hp.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (dataReader != null && !dataReader.IsClosed) dataReader.Close();
                if (conn != null && conn.State == System.Data.ConnectionState.Open) conn.Close();
            }

            if (checker == 0 && string.IsNullOrEmpty(incorrect.Text))
                incorrect.Text = "✕  Invalid username or password.";
        }
    }
}
