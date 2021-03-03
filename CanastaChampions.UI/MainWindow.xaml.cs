using CanastaChampions.Domain.ViewModel;
using System.Windows;

namespace CanastaChampions.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppViewModel _app = null;
        public MainWindow()
        {
            InitializeComponent();
            _app = new AppViewModel();
            CompetitionList.ItemsSource = _app.Competitions;
        }

        private void CreateCompetitionButton_Click(object sender, RoutedEventArgs e)
        {
            _app.CreateCompetition();
        }

        private void SaveCompetitionButton_Click(object sender, RoutedEventArgs e)
        {
            _app.SaveCompetition();
        }
    }
}
