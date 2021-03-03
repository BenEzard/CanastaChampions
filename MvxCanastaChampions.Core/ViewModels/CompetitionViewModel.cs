using CanastaChampions.DataAccess.Models;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.Services;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class CompetitionViewModel : MvxViewModel
    {
        private MvxObservableCollection<CompetitionModel> _competitions = new MvxObservableCollection<CompetitionModel>();
        private CompetitionModel _selectedCompetition;
        private string _competitionName;

        public MvxObservableCollection<CompetitionModel> Competitions
        {
            get { return _competitions; }
            set { 
                _competitions = value;
                SetProperty(ref _competitions, value);
            }
        }


        public CompetitionModel SelectedCompetition
        {
            get { return _selectedCompetition; }
            set { 
                SetProperty(ref _selectedCompetition, value);
                RaisePropertyChanged(() => IsCompetitionSelected);
            }
        }

        public IMvxCommand AddCompetitionCommand { get; set; }
        public IMvxCommand PlayCompetitionCommand { get; set; }



        public string CompetitionName
        {
            get { return _competitionName; }
            set { 
                _competitionName = value;
                SetProperty(ref _competitionName, value);
                RaisePropertyChanged(() => CanAddCompetition);
            }
        }

        private bool _randomisedTeams;

        public bool RandomisedTeams
        {
            get { return _randomisedTeams; }
            set { 
                _randomisedTeams = value;
                SetProperty(ref _randomisedTeams, value);
            }
        }

        private bool _fixedTeams;

        public bool FixedTeams
        {
            get { return _fixedTeams; }
            set
            {
                _fixedTeams = value;
                SetProperty(ref _fixedTeams, value);
            }
        }

        public CompetitionViewModel(IMvxNavigationService navigationService)
        {
            Competitions = new MvxObservableCollection<CompetitionModel>(CompetitionServices.GetAllCompetitions());
            AddCompetitionCommand = new MvxCommand(AddCompetition);
            PlayCompetitionCommand = new MvxCommand(PlayCompetition);
            _navigationService = navigationService;
        }

        public void AddCompetition()
        {
            CompetitionModel cm = new CompetitionModel
            {
                CompetitionName = CompetitionName,
                TeamsAreFixed = FixedTeams,
                TeamsAreRandomised = RandomisedTeams,
                LogicallyDeleted = false
            };

            cm = CompetitionServices.CreateCompetition(cm);
            Competitions.Add(cm);
        }

        public void PlayCompetition()
        {
            _navigationService.Navigate<TeamsViewModel, CompetitionModel>(SelectedCompetition);
        }

        /// <summary>
        /// Check to see if a Competition Name has been entered, and therefore a Competition can be created.
        /// </summary>
        public bool CanAddCompetition
            => CompetitionName?.Length > 0;

        public bool IsCompetitionSelected
            => SelectedCompetition != null;

        private readonly IMvxNavigationService _navigationService;
    }
}
