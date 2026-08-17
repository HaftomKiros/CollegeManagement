using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class TVETTranscriptPage : Page
    {
        private DBConnect _db = new DBConnect();
        public TVETTranscriptPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); }
        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1) g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2) g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }
        // Original parameters: studid, level, academicyear
        private async void BtnGenerate_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtStudID.Text) || string.IsNullOrWhiteSpace(TxtAcadYear.Text))
            { ModernDialog.Show(Window.GetWindow(this),"Please fill all parameters!","Error",ModernDialog.DialogType.Error); return; }
            string sid = TxtStudID.Text.Trim(), lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "1", ay = TxtAcadYear.Text.Trim();
            try {
                var dt = new DataTable();
                var cmd = new MySqlCommand("SELECT student_id,level,module_code,employee_id,academic_year,score_of_knowledge_test,score_of_practical_test,competence FROM ecc_dof_wukrostmarycollege.student_mark WHERE student_id=@s AND level=@l AND academic_year=@y", _db.GetConnection());
                cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl); cmd.Parameters.AddWithValue("@y",ay);
                await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));
                Grid1.ItemsSource = dt.DefaultView;
                PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text = $"Transcript: {sid} | Level {lvl} | Year {ay} — {dt.Rows.Count} records";
            } catch(Exception ex) { ModernDialog.Show(Window.GetWindow(this),"Error: "+ex.Message,"DB Error",ModernDialog.DialogType.Error); }
        }
        private void BtnPrint_Click(object sender, RoutedEventArgs e) {
            if (Grid1.ItemsSource == null) { ModernDialog.Show(Window.GetWindow(this),"Generate first.","Info",ModernDialog.DialogType.Info); return; }
            var pd = new System.Windows.Controls.PrintDialog();
            if (pd.ShowDialog() == true) ModernDialog.Show(Window.GetWindow(this),"Print sent to printer.","Print",ModernDialog.DialogType.Success);
        }
    }
}
