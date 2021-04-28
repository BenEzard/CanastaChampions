using CanastaChampions.DataAccess.Models;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class TeamsViewModel : MvxViewModel<CompetitionModel>
    {
        private readonly IMvxNavigationService _navigationService;

        public CompetitionModel SelectedCompetition { get; set; } = null;

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

        /// <summary>
        /// Check to see if there any unfinished games from this competition.
        /// </summary>
        public bool UnfinishedGameExists
            => GameServices.CheckForUnfinishedGame(SelectedCompetition.CompetitionID, out _, out _);

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

        /// <summary>
        /// Determine if a Player is currently selected.
        /// Returns false if no Player, or more than 1 Player is selected.
        /// </summary>
        public bool IsPlayerSelected
            => (SelectedTeamMembers.Count() == 1) ? true : false;

        #region MoveUpPlayerCommand
        public IMvxCommand MoveUpPlayerCommand { get; set; }

        /// <summary>
        /// Check to see if a Player can be moved up.
        /// For this to happen a Player must be selected, and must not be at the top of the list already.
        /// </summary>
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
        
        #region MovePlayerToTopCommand
        public IMvxCommand MovePlayerToTopCommand { get; set; }

        /// <summary>
        /// Move a Player to the top of the list of Players.
        /// </summary>
        public void MovePlayerToTop()
        {
            int selectedIndex = SelectedIndex;
            GamePlayerModel tf = TeamFormationList[SelectedIndex];
            TeamFormationList.RemoveAt(selectedIndex);
            TeamFormationList.Insert(0, tf);
        }
        #endregion

        #region MoveDownPlayer
        public IMvxCommand MoveDownPlayerCommand { get; set; }

        /// <summary>
        /// Check to see if the Player can be moved down in the list.
        /// In order for this to happen, a Player must be selected and not already at the bottom of the list.
        /// </summary>
        public bool CanMoveDown
            => IsPlayerSelected && (SelectedIndex < TeamFormationList.Count - 1) ? true : false;


        /// <summary>
        /// Move a Player down in the list of Players.
        /// </summary>
        public void MoveDownPlayer()
        {
            int selectedIndex = SelectedIndex;
            GamePlayerModel tf = TeamFormationList[SelectedIndex];
            TeamFormationList.RemoveAt(selectedIndex);
            TeamFormationList.Insert(selectedIndex + 1, tf);
        }
        #endregion

        #region MovePlayerToBottomCommand
        public IMvxCommand MovePlayerToBottomCommand { get; set; }

        public void MovePlayerToBottom()
        {
            int selectedIndex = SelectedIndex;
            GamePlayerModel tf = TeamFormationList[SelectedIndex];
            TeamFormationList.RemoveAt(selectedIndex);
            TeamFormationList.Insert(TeamFormationList.Count, tf);
        }
        #endregion

        #region StartGameCommand
        public IMvxCommand StartGameCommand { get; set; }

        /// <summary>
        /// Start a new Game.
        /// As a prevention against bugs, this will close any incomplete Games from this Competition.
        /// </summary>
        public void StartGame()
        {
            // As a precaution: close of any incomplete Games from this Competition.
            GameServices.ForceablyCloseAnyOpenGames(SelectedCompetition.CompetitionID);

            List<GamePlayerModel> teams = new List<GamePlayerModel>();
            foreach (GamePlayerModel m in TeamFormationList)
            {
                teams.Add(m);
                System.Diagnostics.Debug.WriteLine($"GamePlayerModel entry: CompetitionID = {m.PlayerName} (PlayerID = {m.PlayerID}); {m.CompetitionID}; GameID = {m.GameID}; GameTeamID = {m.TeamID} Team Number = {m.TeamNumber}");
            }

            _navigationService.Navigate<GameViewModel, List<GamePlayerModel>>(teams);
        }
        #endregion



        #region LoadGameCommand
        public IMvxCommand LoadGameCommand { get; set; }

        public void LoadGame()
        {
            System.Diagnostics.Debug.WriteLine($"Load Game");
            List<GamePlayerModel> players = new List<GamePlayerModel>();
            players.Add(new GamePlayerModel
            {
                CompetitionID = SelectedCompetition.CompetitionID,
                PlayerName = "LOAD"
            });
            _navigationService.Navigate<GameViewModel, List<GamePlayerModel>>(players);
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
            MovePlayerToTopCommand = new MvxCommand(MovePlayerToTop);
            MovePlayerToBottomCommand = new MvxCommand(MovePlayerToBottom);
            AddPlayerCommand = new MvxCommand(AddPlayer);
            EditPlayerCommand = new MvxCommand(EditPlayer);
            StartGameCommand = new MvxCommand(StartGame);
            LoadGameCommand = new MvxCommand(LoadGame);
        }


        public override void Prepare(CompetitionModel parameter)
        {
            SelectedCompetition = parameter;
            List<GamePlayerModel> players = CompetitionServices.GetPlayerRoster(parameter);
            TeamFormationList = new MvxObservableCollection<GamePlayerModel>(players);
        }

    }
}
