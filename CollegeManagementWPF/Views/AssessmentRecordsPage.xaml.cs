using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class AssessmentRecordsPage : Page
    {
        public AssessmentRecordsPage()
        {
            InitializeComponent();
            // Navigate to StudentMarksPage as the content
            InnerFrame.Navigate(new StudentMarksPage());
        }
    }
}
