using CanastaChampions.DataAccess.Models;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class TeamsViewModel : MvxViewModel<CompetitionModel>
    {
        private readonly IMvxNavigationService _navigationService;

        public CompetitionModel SelectedCompetition = null;

        private MvxObservableCollection<GamePlayerModel> _teamFormationList = new MvxObservableCollection<GamePlayerModel>();


        public MvxObservableCollection<GamePlayerModel> TeamFormationList
        {
            get => _teamFormationList;
            set { 
                _teamFormationList = value;
                //System.Diagnostics.Debug.WriteLine($"updating the list");
                SetProperty(ref _teamFormationList, value);
            }
        }

        private static int _teamNumber = 1;

        public static int TeamNumber
        {
            get { return _teamNumber; }
        }

        public int SelectedIndex { get; set; }

        private IEnumerable<GamePlayerModel> _selectedTeamMembers = new List<GamePlayerModel>();

        public IEnumerable<GamePlayerModel> SelectedTeamMembers
        {
            get { return _selectedTeamMembers; }
            set { 
                _selectedTeamMembers = value; 
                SetProperty(ref _selectedTeamMembers, value);
                RaisePropertyChanged(() => CanFormTeam);
                RaisePropertyChanged(() => CanUnBindTeam);
                RaisePropertyChanged(() => CanMoveUp);
                RaisePropertyChanged(() => CanMoveDown);
                RaisePropertyChanged(() => IsPlayerSelected);
            }
        }

        #region FormTeamCommand
        public IMvxCommand FormTeamCommand { get; set; }

        /// <summary>
        /// Can a Team be formed? (Checks to ensure that the number of Players selected is 2).
        /// </summary>
        public bool CanFormTeam
            => (SelectedTeamMembers.Count() == 2) ? true : false;

        /// <summary>
        /// Form a Team, changing the Team Number on the 2 selected Players.
        /// TODO: This method doesn't update the UI. (Could be because it doesn't detect the change to the MEMBER of the collection?
        /// </summary>
        public void FormTeam()
        {
            foreach (GamePlayerModel model in SelectedTeamMembers)
            {
                GamePlayerModel tfm = TeamFormationList.Where(x => x.PlayerName == model.PlayerName).FirstOrDefault();
                tfm.TeamNumber = _teamNumber;
                System.Diagnostics.Debug.WriteLine($"==> tfm is {tfm.PlayerName} and {tfm.TeamNumber}");
            }
            ++_teamNumber;
            RaisePropertyChanged(() => CanStartGame);
        }
        #endregion

        #region AddPlayerCommand
        public IMvxCommand AddPlayerCommand { get; set; }

        public void AddPlayer()
        {
            PlayerModel player = new PlayerModel
            {
                PlayerID = -1,
                PlayerName = string.Empty,
                CompetitionID = SelectedCompetition.CompetitionID
            };
            _navigationService.Navigate<PlayerViewModel, PlayerModel>(player);
        }
        #endregion

        #region EditPlayerCommand
        public IMvxCommand EditPlayerCommand { get; set; }

        public void EditPlayer()
        {
            GamePlayerModel tf = TeamFormationList[SelectedIndex];
            PlayerModel player = new PlayerModel
            {

                PlayerID = tf.PlayerID,
                PlayerName = tf.PlayerName,
                CompetitionID = SelectedCompetition.CompetitionID
            };
            _navigationService.Navigate<PlayerViewModel, PlayerModel>(player);
        }
        #endregion

        #region UnBindTeamCommand
        public IMvxCommand UnBindTeamCommand { get; set; }

        public bool CanUnBindTeam
            => (SelectedTeamMembers.Where(x => x.TeamNumber > 0).Count() > 0) ? true : false;

        public void UnBindTeam()
        {
            foreach (GamePlayerModel model in SelectedTeamMembers)
            {
                GamePlayerModel tfm = TeamFormationList.Where(x => x.PlayerName == model.PlayerName).FirstOrDefault();
                tfm.TeamNumber = 0;
                System.Diagnostics.Debug.WriteLine($"==> tfm is {tfm.PlayerName} and {tfm.TeamNumber}");
            }
            RaisePropertyChanged(() => CanStartGame);
        }
        #endregion

        public bool IsPlayerSelected
            => (SelectedTeamMembers.Count() == 1) ? true : false;

        #region MoveUpPlayerCommand
        public IMvxCommand MoveUpPlayerCommand { get; set; }

        public bool CanMoveUp
            => IsPlayerSelected && (SelectedIndex > 0) ? true : false;

        public void MoveUpPlayer()
        {
            int selectedIndex = SelectedIndex;
            GamePlayerModel tf = TeamFormationList[SelectedIndex];
            TeamFormationList.RemoveAt(selectedIndex);
            TeamFormationList.Insert(selectedIndex - 1, tf);
        }
        #endregion

        #region MoveDownPlayer
        public IMvxCommand MoveDownPlayerCommand { get; set; }

        public bool CanMoveDown
            => IsPlayerSelected && (SelectedIndex < TeamFormationList.Count - 1) ? true : false;


        public void MoveDownPlayer()
        {
            int selectedIndex = SelectedIndex;
            GamePlayerModel tf = TeamFormationList[SelectedIndex];
            TeamFormationList.RemoveAt(selectedIndex);
            TeamFormationList.Insert(selectedIndex + 1, tf);
        }
        #endregion

        #region StartGameCommand
        public IMvxCommand StartGameCommand { get; set; }

        public void StartGame()
        {
            List<GamePlayerModel> teams = new List<GamePlayerModel>();
            foreach (GamePlayerModel m in TeamFormationList)
            {
                teams.Add(m);
            }

            _navigationService.Navigate<GameViewModel, List<GamePlayerModel>>(teams);
        }
        #endregion

        public bool CanStartGame
            => (TeamFormationList.Where(x => x.TeamNumber == 0).Count() == 0) ? true : false;


        public TeamsViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
            FormTeamCommand = new MvxCommand(FormTeam);
            UnBindTeamCommand = new MvxCommand(UnBindTeam);
            MoveUpPlayerCommand = new MvxCommand(MoveUpPlayer);
            MoveDownPlayerCommand = new MvxCommand(MoveDownPlayer);
            AddPlayerCommand = new MvxCommand(AddPlayer);
            EditPlayerCommand = new MvxCommand(EditPlayer);
            StartGameCommand = new MvxCommand(StartGame);
        }


        public override void Prepare(CompetitionModel parameter)
        {
            SelectedCompetition = parameter;
            List<GamePlayerModel> players = CompetitionServices.GetPlayerRoster(parameter);
            TeamFormationList = new MvxObservableCollection<GamePlayerModel>(players);
        }

    }
}
