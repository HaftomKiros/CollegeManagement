using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class COCListPage : Page
    {
        private DBConnect _db = new DBConnect();
        public COCListPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        // Original parameters: deptid, streamid, level, admissiontype, admissiondate(academicyear)
        // Extended: + assessment_date
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
            string ad  = TxtAssessDate.Text.Trim();
            string ay  = TxtAcadYear.Text.Trim();

            try
            {
                var dt = new DataTable();
                string sql =
                    "SELECT c.student_id, c.level, c.assessment_date, c.assessor_name, " +
                    "c.supervisor_name, c.competence, c.coc_level_id " +
                    "FROM ecc_dof_wukrostmarycollege.coc c " +
                    "JOIN ecc_dof_wukrostmarycollege.student_profile sp " +
                    "ON c.student_id = sp.student_id AND c.level = sp.level " +
                    "WHERE sp.dept_id=@d AND sp.stream_id=@s AND c.level=@l AND sp.admission_type=@at";

                if (!string.IsNullOrEmpty(ad)) sql += " AND c.assessment_date LIKE @ad";
                if (!string.IsNullOrEmpty(ay)) sql += " AND sp.admission_date LIKE @ay";

                var cmd = new MySqlCommand(sql, _db.GetConnection());
                cmd.Parameters.AddWithValue("@d",  di);
                cmd.Parameters.AddWithValue("@s",  si);
                cmd.Parameters.AddWithValue("@l",  lv);
                cmd.Parameters.AddWithValue("@at", at);
                if (!string.IsNullOrEmpty(ad)) cmd.Parameters.AddWithValue("@ad", $"%{ad}%");
                if (!string.IsNullOrEmpty(ay)) cmd.Parameters.AddWithValue("@ay", $"%{ay}%");

                await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));

                Grid1.ItemsSource = dt.DefaultView;
                PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text = $"COC List — Dept={di} | Stream={si} | Level={lv} | Adm={at}" +
                    (string.IsNullOrEmpty(ad) ? "" : $" | Assessment={ad}") +
                    (string.IsNullOrEmpty(ay) ? "" : $" | Year={ay}") +
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
            { ModernDialog.Show(Window.GetWindow(this),"Generate first.","Info",ModernDialog.DialogType.Info); return; }
            var pd = new System.Windows.Controls.PrintDialog();
            if (pd.ShowDialog() == true)
                ModernDialog.Show(Window.GetWindow(this),"Sent to printer.","Print",ModernDialog.DialogType.Success);
        }
    }
}
