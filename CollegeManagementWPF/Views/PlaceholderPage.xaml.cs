using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class PlaceholderPage : Page
    {
        public PlaceholderPage(string name)
        {
            InitializeComponent();
            TxtName.Text = name;
        }
    }
}
