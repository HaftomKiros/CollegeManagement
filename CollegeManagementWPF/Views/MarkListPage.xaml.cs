using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class MarkListPage : Page
    {
        private DBConnect _db = new DBConnect();
        public MarkListPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        // Original report parameters: deptid, streamid, level, admissiontype
        // Extended form also captures: module_code, academic_year for richer preview
        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtDeptID.Text) || string.IsNullOrWhiteSpace(TxtStreamID.Text))
            {
                ModernDialog.Show(Window.GetWindow(this), "Department ID and Stream ID are required!", "Error", ModernDialog.DialogType.Error);
                return;
            }

            string di  = TxtDeptID.Text.Trim();
            string si  = TxtStreamID.Text.Trim();
            string lv  = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "1";
            string at  = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Regular";
            string mc  = TxtModCode.Text.Trim();
            string ay  = TxtAcadYear.Text.Trim();
            string ins = TxtInstructor.Text.Trim();

            try
            {
                var dt = new DataTable();

                // Build query matching original report parameters (deptid, streamid, level, admissiontype)
                // + optional module_code and academic_year filters
                string sql =
                    "SELECT sm.student_id, sm.level, sm.module_code, sm.employee_id, " +
                    "sm.academic_year, sm.score_of_knowledge_test, sm.score_of_practical_test, sm.competence " +
                    "FROM ecc_dof_wukrostmarycollege.student_mark sm " +
                    "JOIN ecc_dof_wukrostmarycollege.student_profile sp " +
                    "ON sm.student_id = sp.student_id AND sm.level = sp.level " +
                    "WHERE sp.dept_id = @d AND sp.stream_id = @s " +
                    "AND sm.level = @l AND sp.admission_type = @at";

                if (!string.IsNullOrEmpty(mc))  sql += " AND sm.module_code = @mc";
                if (!string.IsNullOrEmpty(ay))  sql += " AND sm.academic_year = @ay";
                if (!string.IsNullOrEmpty(ins)) sql += " AND sm.employee_id = @ins";

                var cmd = new MySqlCommand(sql, _db.GetConnection());
                cmd.Parameters.AddWithValue("@d",  di);
                cmd.Parameters.AddWithValue("@s",  si);
                cmd.Parameters.AddWithValue("@l",  lv);
                cmd.Parameters.AddWithValue("@at", at);
                if (!string.IsNullOrEmpty(mc))  cmd.Parameters.AddWithValue("@mc",  mc);
                if (!string.IsNullOrEmpty(ay))  cmd.Parameters.AddWithValue("@ay",  ay);
                if (!string.IsNullOrEmpty(ins)) cmd.Parameters.AddWithValue("@ins", ins);

                await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));

                Grid1.ItemsSource = dt.DefaultView;
                PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text = $"Mark List — Dept: {di} | Stream: {si} | Level: {lv} | Adm: {at}" +
                    (string.IsNullOrEmpty(mc) ? "" : $" | Module: {mc}") +
                    (string.IsNullOrEmpty(ay) ? "" : $" | Year: {ay}") +
                    $" — {dt.Rows.Count} records";
            }
            catch (Exception ex)
            {
                ModernDialog.Show(Window.GetWindow(this), "Error: " + ex.Message, "DB Error", ModernDialog.DialogType.Error);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource == null)
            {
                ModernDialog.Show(Window.GetWindow(this), "Generate the report first.", "Info", ModernDialog.DialogType.Info);
                return;
            }
            var pd = new System.Windows.Controls.PrintDialog();
            if (pd.ShowDialog() == true)
                ModernDialog.Show(Window.GetWindow(this), "Sent to printer.", "Print", ModernDialog.DialogType.Success);
        }
    }
}
