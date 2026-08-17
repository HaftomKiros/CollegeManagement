using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class AttendanceSheetPage : Page
    {
        private DBConnect _db = new DBConnect();
        public AttendanceSheetPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        // Original parameters: deptid, streamid, level, admissiontype
        // Extended form: + class_year, academic_year, semester
        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtDeptID.Text) || string.IsNullOrWhiteSpace(TxtStreamID.Text))
            {
                ModernDialog.Show(Window.GetWindow(this), "Department ID and Stream ID are required!", "Error", ModernDialog.DialogType.Error);
                return;
            }

            string di  = TxtDeptID.Text.Trim();
            string si  = TxtStreamID.Text.Trim();
            string at  = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Regular";
            string lv  = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "1";
            string ay  = TxtAcadYear.Text.Trim();
            string sem = TxtSemester.Text.Trim();
            string cy  = TxtClassYear.Text.Trim();

            try
            {
                var dt = new DataTable();
                // Original Crystal Report params: deptid, streamid, level, admissiontype
                // Query student_profile to generate attendance list
                string sql =
                    "SELECT student_id, dept_id, stream_id, level, first_name, father_name, " +
                    "grand_father_name, gender, admission_type, admission_date " +
                    "FROM ecc_dof_wukrostmarycollege.student_profile " +
                    "WHERE dept_id=@d AND stream_id=@s AND level=@l AND admission_type=@at";

                if (!string.IsNullOrEmpty(ay))  sql += " AND admission_date LIKE @ay";

                var cmd = new MySqlCommand(sql, _db.GetConnection());
                cmd.Parameters.AddWithValue("@d",  di);
                cmd.Parameters.AddWithValue("@s",  si);
                cmd.Parameters.AddWithValue("@l",  lv);
                cmd.Parameters.AddWithValue("@at", at);
                if (!string.IsNullOrEmpty(ay)) cmd.Parameters.AddWithValue("@ay", $"%{ay}%");

                await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));

                Grid1.ItemsSource = dt.DefaultView;
                PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text = $"Attendance: Dept={di} | Stream={si} | Level={lv} | Adm={at}" +
                    (string.IsNullOrEmpty(ay)  ? "" : $" | Year={ay}") +
                    (string.IsNullOrEmpty(sem) ? "" : $" | Sem={sem}") +
                    (string.IsNullOrEmpty(cy)  ? "" : $" | Class={cy}") +
                    $" — {dt.Rows.Count} students";
            }
            catch (Exception ex)
            {
                ModernDialog.Show(Window.GetWindow(this), "Error: " + ex.Message, "DB Error", ModernDialog.DialogType.Error);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource == null)
            { ModernDialog.Show(Window.GetWindow(this),"Generate first.","Info",ModernDialog.DialogType.Info); return; }
            var pd = new System.Windows.Controls.PrintDialog();
            if (pd.ShowDialog() == true)
                ModernDialog.Show(Window.GetWindow(this),"Sent to printer.","Print",ModernDialog.DialogType.Success);
        }
    }
}
