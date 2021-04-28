using System.Windows.Media;

using MvvmCross.Platforms.Wpf.Views;

using MvxCanastaChampions.Core.ViewModels;

namespace MvxCanastaChampions.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CompetitionView.xaml
    /// </summary>
    public partial class CompetitionView : MvxWpfView
    {
        CompetitionViewModel _viewModel;
        MediaPlayer _mediaPlayer;

        public CompetitionView()
        {
            InitializeComponent();

            if (this.ViewModel is CompetitionViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            _mediaPlayer = new MediaPlayer();

            if (_viewModel != null)
            {
                // As long as the view model exists, this sets up our local method as the delegate (event handler) for the event
                _viewModel.CompetitionStarted += this.ViewModel_CompetitionStarted;
            }
        }

        /// <summary>
        /// When the event in the ViewModel is raised (via invoking the event handler delegate), then this method is actually what is being invoked
        /// </summary>
        private void ViewModel_CompetitionStarted(object sender, System.EventArgs e)
        {
            _mediaPlayer.Open(new System.Uri("path_to_some_media_file"));
            _mediaPlayer.Play();

            // Alternatively, play whatever sound is associated with the Asterisk sound event in Windows.
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
