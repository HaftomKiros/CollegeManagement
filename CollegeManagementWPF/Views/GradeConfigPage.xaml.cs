using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class GradeConfigPage : Page
    {
        private readonly DBConnect _db = new DBConnect();
        private int _selId = -1;

        private const string CREATE_SQL =
            "CREATE TABLE IF NOT EXISTS ecc_dof_wukrostmarycollege.grade_config (" +
            "  id          INT AUTO_INCREMENT PRIMARY KEY," +
            "  min_score   DECIMAL(5,2) NOT NULL," +
            "  max_score   DECIMAL(5,2) NOT NULL," +
            "  letter_grade VARCHAR(10) NOT NULL," +
            "  grade_points DECIMAL(4,2) NOT NULL," +
            "  range_label  VARCHAR(30) NOT NULL" +
            ") ENGINE=InnoDB;";

        public GradeConfigPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) =>
            {
                await EnsureTableAsync();
                await LoadAsync();
            };
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private async Task EnsureTableAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    new MySqlCommand(CREATE_SQL, conn).ExecuteNonQuery();
                    conn.Close();
                });
            }
            catch { }
        }

        private async Task LoadAsync()
        {
            try
            {
                var dt = new DataTable();
                await Task.Run(() =>
                {
                    new MySqlDataAdapter(
                        "SELECT id, min_score, max_score, letter_grade, grade_points, range_label " +
                        "FROM ecc_dof_wukrostmarycollege.grade_config ORDER BY min_score DESC",
                        _db.GetConnection()).Fill(dt);
                });
                Grid1.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex) { Msg("DB Error: " + ex.Message, false); }
        }

        private void Grid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Grid1.SelectedItem is not DataRowView r) return;
            _selId = Convert.ToInt32(r["id"]);
            TxtMin.Text    = r["min_score"]?.ToString()    ?? "";
            TxtMax.Text    = r["max_score"]?.ToString()    ?? "";
            TxtLetter.Text = r["letter_grade"]?.ToString() ?? "";
            TxtPoints.Text = r["grade_points"]?.ToString() ?? "";
        }

        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            if (!Validate(out double min, out double max, out string letter, out double pts)) return;
            try
            {
                string label = BuildLabel(min, max);
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.grade_config " +
                        "(min_score,max_score,letter_grade,grade_points,range_label) " +
                        "VALUES(@mn,@mx,@l,@p,@rl)", conn);
                    cmd.Parameters.AddWithValue("@mn", min);
                    cmd.Parameters.AddWithValue("@mx", max);
                    cmd.Parameters.AddWithValue("@l",  letter);
                    cmd.Parameters.AddWithValue("@p",  pts);
                    cmd.Parameters.AddWithValue("@rl", label);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                Msg("Saved!", true); await LoadAsync(); Clear();
            }
            catch (Exception ex) { Msg(ex.Message, false); }
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (_selId < 0) { Msg("Select a row first.", false); return; }
            if (!Validate(out double min, out double max, out string letter, out double pts)) return;
            try
            {
                string label = BuildLabel(min, max);
                int id = _selId;
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE ecc_dof_wukrostmarycollege.grade_config " +
                        "SET min_score=@mn,max_score=@mx,letter_grade=@l,grade_points=@p,range_label=@rl " +
                        "WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@mn", min);
                    cmd.Parameters.AddWithValue("@mx", max);
                    cmd.Parameters.AddWithValue("@l",  letter);
                    cmd.Parameters.AddWithValue("@p",  pts);
                    cmd.Parameters.AddWithValue("@rl", label);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                Msg("Updated!", true); await LoadAsync(); Clear();
            }
            catch (Exception ex) { Msg(ex.Message, false); }
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (_selId < 0) { Msg("Select a row first.", false); return; }
            var dlg = new ModernDialog($"Delete grade rule?", "Confirm",
                ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;
            int id = _selId;
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand(
                        "DELETE FROM ecc_dof_wukrostmarycollege.grade_config WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                Msg("Deleted!", true); await LoadAsync(); Clear();
            }
            catch (Exception ex) { Msg(ex.Message, false); }
        }

        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();

        // Load the standard default grade scale
        private async void BtnDefaults_Click(object s, RoutedEventArgs e)
        {
            var dlg = new ModernDialog(
                "This will replace all existing grade rules with the standard defaults. Continue?",
                "Load Defaults", ModernDialog.DialogType.Warning)
            { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;

            // All ranges inclusive: min <= score <= max
            var defaults = new[]
            {
                (95.0, 100.0, "A+",  4.00),
                (92.0,  94.0, "A",   4.00),
                (89.0,  91.0, "A-",  3.75),
                (86.0,  88.0, "B+",  3.50),
                (83.0,  85.0, "B",   3.00),
                (80.0,  82.0, "B-",  2.75),
                (77.0,  79.0, "C+",  2.50),
                (74.0,  76.0, "C",   2.00),
                ( 0.0,  73.0, "NYC", 0.00),
            };

            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.grade_config", conn).ExecuteNonQuery();
                    foreach (var (mn, mx, l, p) in defaults)
                    {
                        var cmd = new MySqlCommand(
                            "INSERT INTO ecc_dof_wukrostmarycollege.grade_config " +
                            "(min_score,max_score,letter_grade,grade_points,range_label) VALUES(@mn,@mx,@l,@p,@rl)", conn);
                        cmd.Parameters.AddWithValue("@mn", mn);
                        cmd.Parameters.AddWithValue("@mx", mx);
                        cmd.Parameters.AddWithValue("@l",  l);
                        cmd.Parameters.AddWithValue("@p",  p);
                        cmd.Parameters.AddWithValue("@rl", BuildLabel(mn, mx));
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                });
                Msg("Default grades loaded!", true);
                await LoadAsync();
            }
            catch (Exception ex) { Msg(ex.Message, false); }
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private bool Validate(out double min, out double max, out string letter, out double pts)
        {
            min = max = pts = 0; letter = TxtLetter.Text.Trim();
            if (!double.TryParse(TxtMin.Text.Trim(), out min) ||
                !double.TryParse(TxtMax.Text.Trim(), out max) ||
                string.IsNullOrEmpty(letter) ||
                !double.TryParse(TxtPoints.Text.Trim(), out pts))
            {
                Msg("Please fill all fields with valid numbers.", false);
                return false;
            }
            return true;
        }

        private static string BuildLabel(double min, double max)
        {
            if (min <= 0 && max < 74) return "Below 74";
            if (min == max)           return $"{(int)min}";
            return $"{(int)min}–{(int)max}";
        }

        private void Clear()
        {
            TxtMin.Text = TxtMax.Text = TxtLetter.Text = TxtPoints.Text = "";
            _selId = -1;
        }

        private void Msg(string m, bool ok)
        {
            var owner = Window.GetWindow(this);
            if (ok) ModernDialog.Show(owner, m, "Success", ModernDialog.DialogType.Success);
            else    ModernDialog.Show(owner, m, "Error",   ModernDialog.DialogType.Error);
        }
    }
}
