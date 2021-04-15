using System.Windows.Media;
using MvvmCross.Platforms.Wpf.Views;

namespace MvxCanastaChampions.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CompetitionView.xaml
    /// </summary>
    public partial class CompetitionView : MvxWpfView
    {
        MediaPlayer _mediaPlayer;

        public CompetitionView()
        {
            InitializeComponent();

            _mediaPlayer = new MediaPlayer();
        }
    }
}
