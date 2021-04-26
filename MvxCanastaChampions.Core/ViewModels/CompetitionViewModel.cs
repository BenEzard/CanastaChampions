using CanastaChampions.DataAccess;
using CanastaChampions.DataAccess.Models;
using CanastaChampions.DataAccess.Services;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.Services;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class CompetitionViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;

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

        public IMvxCommand ResetDatabaseCommand { get; set; }

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
            ResetDatabaseCommand = new MvxCommand(ResetDatabase);
            _navigationService = navigationService;

            //MediaPlayer mp = new MediaPlayer();
        }

        public void ResetDatabase()
        {
            bool loadData = true;
            _ = new DBInstaller(BaseDataAccess.DB_FILE, loadData);

            if (loadData)
            {
                // Create Competitions
                var burgerComp = CompetitionServices.GetOrCreateCompetition(new CompetitionModel("Burger Empire", "/Images/canasta_champions.png", true, false));
                var daveComp = CompetitionServices.GetOrCreateCompetition(new CompetitionModel("Dave's Canasta Regulars", "/Images/no_image.jpg", false, true));

                // Add Players
                var ben = CompetitionServices.GetOrCreatePlayer(new PlayerModel("Ben"));
                var jen = CompetitionServices.GetOrCreatePlayer(new PlayerModel("Jen"));
                var nathan = CompetitionServices.GetOrCreatePlayer(new PlayerModel("Nathan"));
                var danni = CompetitionServices.GetOrCreatePlayer(new PlayerModel("Danni"));
                var rowan = CompetitionServices.GetOrCreatePlayer(new PlayerModel("Rowan"));
                var poly = CompetitionServices.GetOrCreatePlayer(new PlayerModel("Poly"));
                var kitty = CompetitionServices.GetOrCreatePlayer(new PlayerModel("Kitty"));
                var droot = CompetitionServices.GetOrCreatePlayer(new PlayerModel("Droot"));

                // Register Players to Competition
                CompetitionServices.RegisterPlayer(burgerComp.CompetitionID, ben.PlayerID);
                CompetitionServices.RegisterPlayer(burgerComp.CompetitionID, jen.PlayerID);
                CompetitionServices.RegisterPlayer(burgerComp.CompetitionID, nathan.PlayerID);
                CompetitionServices.RegisterPlayer(burgerComp.CompetitionID, danni.PlayerID);
                CompetitionServices.RegisterPlayer(daveComp.CompetitionID, ben.PlayerID);
                CompetitionServices.RegisterPlayer(daveComp.CompetitionID, jen.PlayerID);
                CompetitionServices.RegisterPlayer(daveComp.CompetitionID, poly.PlayerID);
                CompetitionServices.RegisterPlayer(daveComp.CompetitionID, rowan.PlayerID);
                CompetitionServices.RegisterPlayer(daveComp.CompetitionID, kitty.PlayerID);
                CompetitionServices.RegisterPlayer(daveComp.CompetitionID, droot.PlayerID);

                // Register Teams
                CompetitionServices.RegisterTeam(burgerComp.CompetitionID, ben.PlayerID, nathan.PlayerID);
                CompetitionServices.RegisterTeam(burgerComp.CompetitionID, jen.PlayerID, danni.PlayerID);
            }
        }

        public void AddCompetition()
        {
            CompetitionModel cm = new CompetitionModel(CompetitionName, FixedTeams, RandomisedTeams);

            CompetitionServices.GetOrCreateCompetition(cm);
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

        
    }
}
