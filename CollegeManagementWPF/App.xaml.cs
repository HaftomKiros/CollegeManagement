using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace CollegeManagementWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                File.WriteAllText("crash.log", ex.ExceptionObject.ToString());
                MessageBox.Show(ex.ExceptionObject.ToString(), "Crash", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                File.WriteAllText("crash.log", ex.Exception.ToString());
                MessageBox.Show(ex.Exception.ToString(), "Crash", MessageBoxButton.OK, MessageBoxImage.Error);
                ex.Handled = true;
            };

            base.OnStartup(e);
            ThemeManager.Apply(); // ensure correct theme dict is loaded
        }
    }
}
