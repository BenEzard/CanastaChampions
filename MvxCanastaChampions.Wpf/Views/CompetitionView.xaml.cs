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
        //MediaPlayer _mediaPlayer;

        public CompetitionView()
        {
            InitializeComponent();

            // Since DataContext / ViewModel is not set at time of construction, setup an event handler to register the view model's event handler when the DataContext is set.
            this.DataContextChanged += this.CompetitionView_DataContextChanged;

            //_mediaPlayer = new MediaPlayer();
        }

        private void CompetitionView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // First Check the DataContext, then the ViewModel, but it appears they are both the same...
            // see https://www.mvvmcross.com/documentation/fundamentals/data-binding#a-note-about-datacontext
            if (this.DataContext is CompetitionViewModel viewModel)
            {
                // I'm preferencing the DataContext since that is the property that triggered this event
                _viewModel = viewModel;
            }
            else if (this.ViewModel is CompetitionViewModel competitionViewModel)
            {
                _viewModel = competitionViewModel;
            }

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
            //_mediaPlayer.Open(new System.Uri("path_to_some_media_file"));
            //_mediaPlayer.Play();

            // Alternatively, play whatever sound is associated with the Asterisk sound event in Windows.
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
