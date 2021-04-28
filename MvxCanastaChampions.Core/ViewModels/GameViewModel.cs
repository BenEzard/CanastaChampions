using CanastaChampions.DataAccess.Models;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class GameViewModel : MvxViewModel<List<GamePlayerModel>>
    {
        private readonly IMvxNavigationService _navigationService;

        #region Team1Members
        private GamePlayerModel _team1Member1;

        public GamePlayerModel Team1Player1
        {
            get { return _team1Member1; }
            set {
                _team1Member1 = value;
                SetProperty(ref _team1Member1, value);
            }
        }

        private GamePlayerModel _team1Member2;

        public GamePlayerModel Team1Player2
        {
            get { return _team1Member2; }
            set
            {
                _team1Member2 = value;
                SetProperty(ref _team1Member2, value);
            }
        }
        #endregion

        #region Team2Members
        private GamePlayerModel _team2Player1;

        public GamePlayerModel Team2Player1
        {
            get { return _team2Player1; }
            set
            {
                _team2Player1 = value;
                SetProperty(ref _team2Player1, value);
            }
        }

        private GamePlayerModel _team2Player2;

        public GamePlayerModel Team2Player2
        {
            get { return _team2Player2; }
            set
            {
                _team2Player2 = value;
                SetProperty(ref _team2Player2, value);
            }
        }
        #endregion

        #region Team3Members
        private bool _isTeam3Utilised = false;

        public bool IsTeam3Utilised
        {
            get => _isTeam3Utilised;
            set
            {
                _isTeam3Utilised = value;
                SetProperty(ref _isTeam3Utilised, value);
            }
        }

        private GamePlayerModel _team3Player1;

        public GamePlayerModel Team3Player1
        {
            get { return _team3Player1; }
            set
            {
                _team3Player1 = value;
                SetProperty(ref _team3Player1, value);
                IsTeam3Utilised = true;
            }
        }

        private GamePlayerModel _team3Player2;

        public GamePlayerModel Team3Player2
        {
            get { return _team3Player2; }
            set
            {
                _team3Player2 = value;
                SetProperty(ref _team3Player2, value);
                IsTeam3Utilised = true;
            }
        }
        #endregion

        #region TotalScore
        private int _team1TotalScore = 0;

        public int Team1TotalScore
        {
            get { return _team1TotalScore; }
            set {
                _team1TotalScore = value;
                SetProperty(ref _team1TotalScore, value);
                RaisePropertyChanged(() => Team1Target);
            }
        }

        private int _team2TotalScore = 0;

        public int Team2TotalScore
        {
            get { return _team2TotalScore; }
            set
            {
                _team2TotalScore = value;
                SetProperty(ref _team2TotalScore, value);
                RaisePropertyChanged(() => Team2Target);
            }
        }

        private int _team3TotalScore = 0;

        public int Team3TotalScore
        {
            get { return _team3TotalScore; }
            set
            {
                _team3TotalScore = value;
                SetProperty(ref _team3TotalScore, value);
                RaisePropertyChanged(() => Team3Target);
            }
        }
        #endregion

        #region TeamTarget
        public int Team1Target
        { get
            {
                int target = 50;

                if (Team1TotalScore >= 3000)
                    target = 120;
                else if (Team1TotalScore >= 1500)
                    target = 90;

                return target;
            }
        }

        public int Team2Target
        { get
            {
                int target = 50;

                if (Team2TotalScore >= 3000)
                    target = 120;
                else if (Team2TotalScore >= 1500)
                    target = 90;

                return target;
            }
        }

        public int Team3Target
        {
            get
            {
                int target = 50;

                if (Team3TotalScore >= 3000)
                    target = 120;
                else if (Team3TotalScore >= 1500)
                    target = 90;

                return target;
            }
        }
        #endregion

        private long _competitionID;
        private long _gameID;

        private List<GamePlayerModel> _gamePlayers = null;

        private GameModel _game = new GameModel();

        public GameModel Game
        {
            get { return _game; }
            set
            {
                _game = value;
                SetProperty(ref _game, value);
            }
        }


        private RoundModel _round = new RoundModel();

        public RoundModel GameRound
        {
            get { return _round; }
            set
            {
                _round = value;
                SetProperty(ref _round, value);
            }
        }

        private MvxObservableCollection<RoundResultModel> _results = new MvxObservableCollection<RoundResultModel>();

        public MvxObservableCollection<RoundResultModel> Results
        {
            get => _results;
            set
            {
                _results = value;
                //System.Diagnostics.Debug.WriteLine($"updating the list");
                SetProperty(ref _results, value);
            }
        }

        /// <summary>
        /// Handle the pass-through variable.
        /// It expects either:
        ///     (a) 1 element in the list with the competitionID filled out
        ///     (b) all of the players in the list
        /// </summary>
        /// <param name="parameter"></param>
        public override void Prepare(List<GamePlayerModel> parameter)
        {
            _gamePlayers = parameter;
            
            // Unpack the pass-through variable into teams
            UnpackPlayerVariable(parameter);

            // Check to see if there is an ongoing game.
            if (GameServices.CheckForUnfinishedGame(_competitionID, out long gameID, out long gameRoundID))
            {
                System.Diagnostics.Debug.WriteLine($"==> Unfinished gameID found {gameID} (roundID = {gameRoundID})");
                LoadGame(_competitionID, gameID, gameRoundID);
            }
            else 
                StartGame();
        }

        /// <summary>
        /// Unpack the List of GamePlayerModel's into individual Teams and Players.
        /// Also populates CompetitionID and GameID if they are populated within the GamePlayerModel.
        /// </summary>
        /// <param name="parameter"></param>
        private void UnpackPlayerVariable(List<GamePlayerModel> parameter)
        {
            System.Diagnostics.Debug.WriteLine($"GameViewModel.UnpackPlayerVariable()");
            foreach(GamePlayerModel gmp in parameter)
            {
                System.Diagnostics.Debug.WriteLine($"   Player: {gmp.PlayerName} (TeamNumber = {gmp.TeamNumber})");
            }

            if (parameter.Count == 1)
            {
                _competitionID = parameter.ElementAt(0).CompetitionID;
                return;
            }

            IEnumerable<GamePlayerModel> team = parameter.Where(x => x.TeamNumber == 1);

            if (team.ElementAt(0).CompetitionID != -1)
                _competitionID = team.ElementAt(0).CompetitionID;
            if (team.ElementAt(0).GameID != -1)
                _gameID = team.ElementAt(0).GameID;

            Team1Player1 = team.ElementAt(0);
            Team1Player2 = team.ElementAt(1);

            team = parameter.Where(x => x.TeamNumber == 2);
            Team2Player1 = team.ElementAt(0);
            Team2Player2 = team.ElementAt(1);


            team = parameter.Where(x => x.TeamNumber == 3);
            if (team.Count() > 0)
            {
                Team3Player1 = team.ElementAt(0);
                Team3Player2 = team.ElementAt(1);
            }
        }

        /// <summary>
        /// Load an existing Game and/or Round that was incomplete.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        private void LoadGame(long competitionID, long gameID, long gameRoundID)
        {
            _gameID = gameID;
            Game = GameServices.LoadGame(competitionID, gameID);
            
            // Load players
            List<GamePlayerModel> players = GameServices.GetCurrentPlayers(competitionID, gameID);
            UnpackPlayerVariable(players);

            if (gameRoundID > 0)
            {
                GameRound = GameServices.LoadRound(gameRoundID);
                long dealerPlayerID = GameRound.Dealer.PlayerID;
                GameRound.Dealer = players.Where(x => x.PlayerID == dealerPlayerID).FirstOrDefault();
                System.Diagnostics.Debug.WriteLine($"Dealer is {GameRound.Dealer.PlayerName}");
            }

            if (GameRound.Dealer == null)
            {
                (GamePlayerModel currentDealer, _) = GameServices.GetDealer(_gameID);
                GameRound.Dealer = currentDealer;
            }

            Results.Clear();
            Results = new MvxObservableCollection<RoundResultModel>(GameServices.GetResults(_competitionID, _gameID));
            Team1TotalScore = Results.Sum(x => x.Team1Score);
            Team2TotalScore = Results.Sum(x => x.Team2Score);
            Team3TotalScore = Results.Sum(x => x.Team3Score);

            System.Diagnostics.Debug.WriteLine($"==> Unfinished gameID found {gameID} (roundID = {gameRoundID})");
        }

        #region StartRoundCommand
        public IMvxCommand StartRoundCommand { get; set; }

        public void StartRound()
        {
            // Initiliase GameRound object
            GameRound.CompetitionID = _competitionID;
            GameRound.GameID = _gameID;
            GameRound.RoundStartDateTime = DateTime.Now;

            GameServices.StartRound(GameRound);
            RaisePropertyChanged(() => IsStartRoundButtonAvailable);
            RaisePropertyChanged(() => IsEndRoundButtonAvailable);
            RaisePropertyChanged(() => IsEndRoundButtonAndTeam3Available);
        }
        #endregion

        #region EndRoundCommand
        public IMvxCommand EndRoundCommand { get; set; }

        public void EndRound()
        {
            GameRound.RoundEndDateTime = DateTime.Now;

            RaisePropertyChanged(() => IsStartRoundButtonAvailable);
            RaisePropertyChanged(() => IsEndRoundButtonAvailable);

            _navigationService.Navigate<GameRoundScoreViewModel, RoundModel>(GameRound);
        }
        #endregion

        #region EndGameCommand
        public IMvxCommand EndGameCommand { get; set; }

        public void EndGame()
        {
            Game.GameEndDateTime = DateTime.Now;

            // TODO check to see if 5000 has been reached, if so
            // TODO check end game achievements
            // TODO present score

            GameServices.FinaliseGame(Game.CompetitionID, Game.GameID, Game.GameEndDateTime);

            _navigationService.Navigate<CompetitionViewModel>();
        }
        #endregion 

        public IMvxCommand Team1Player1PenaltyCommand { get; set; }

        public void Team1Player1Penalty()
        {
            Team1Player1.PenaltyCount += 1;
            GameServices.AddPlayerPenalty(Team1Player1, _round.GameRoundID);
            RaisePropertyChanged(() => Team1Player1);
        }

        public IMvxCommand Team1Player2PenaltyCommand { get; set; }

        public void Team1Player2Penalty()
        {
            Team1Player2.PenaltyCount += 1;
            GameServices.AddPlayerPenalty(Team1Player2, _round.GameRoundID);
            RaisePropertyChanged(() => Team1Player2);
        }

        public IMvxCommand Team2Player1PenaltyCommand { get; set; }

        public void Team2Player1Penalty()
        {
            Team2Player1.PenaltyCount += 1;
            GameServices.AddPlayerPenalty(Team2Player1, _round.GameRoundID);
            RaisePropertyChanged(() => Team2Player1);
        }

        public IMvxCommand Team2Player2PenaltyCommand { get; set; }

        public void Team2Player2Penalty()
        {
            Team2Player2.PenaltyCount += 1;
            GameServices.AddPlayerPenalty(Team2Player2, _round.GameRoundID);
            RaisePropertyChanged(() => Team2Player2);
        }

        public IMvxCommand Team3Player1PenaltyCommand { get; set; }

        public void Team3Player1Penalty()
        {
            Team3Player1.PenaltyCount += 1;
            GameServices.AddPlayerPenalty(Team3Player1, _round.GameRoundID);
            RaisePropertyChanged(() => Team3Player1);
        }

        public IMvxCommand Team3Player2PenaltyCommand { get; set; }

        public void Team3Player2Penalty()
        {
            Team3Player2.PenaltyCount += 1;
            GameServices.AddPlayerPenalty(Team3Player2, _round.GameRoundID);
            RaisePropertyChanged(() => Team3Player2);
        }

        public bool IsEndRoundButtonAvailable
            => GameRound.GameRoundID == -1 ? false : true;

        public bool IsStartRoundButtonAvailable
            => GameRound.GameRoundID == -1 && FinalScoreNotReached == false ? true : false;

        public bool IsEndRoundButtonAndTeam3Available
            => GameRound.GameRoundID == -1  && IsTeam3Utilised ? false : true;

        public bool FinalScoreNotReached
            => Team1TotalScore >= 5000 || Team2TotalScore >= 5000 || Team3TotalScore >= 5000 ? true : false;

        public GameViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
            StartRoundCommand = new MvxCommand(StartRound);
            EndRoundCommand = new MvxCommand(EndRound);
            EndGameCommand = new MvxCommand(EndGame);
            Team1Player1PenaltyCommand = new MvxCommand(Team1Player1Penalty);
            Team1Player2PenaltyCommand = new MvxCommand(Team1Player2Penalty);            
            Team2Player1PenaltyCommand = new MvxCommand(Team2Player1Penalty);
            Team2Player2PenaltyCommand = new MvxCommand(Team2Player2Penalty);            
            Team3Player1PenaltyCommand = new MvxCommand(Team3Player1Penalty);
            Team3Player2PenaltyCommand = new MvxCommand(Team3Player2Penalty);
        }

        public void StartGame()
        {
            Game.GameStartDateTime = DateTime.Now;

            List <GamePlayerModel> playerList = GameServices.CreateGame(
                competitionID: _competitionID, 
                location: null, 
                gameStartDateTime: Game.GameStartDateTime,
                gamePlayers: _gamePlayers);

            UnpackPlayerVariable(playerList);
            System.Diagnostics.Debug.WriteLine($"UnPackParameterVariable Team1Player1: " +
                $"CompetitionID = {Team1Player1.PlayerName} (PlayerID = {Team1Player1.PlayerID}); " +
                $"{Team1Player1.CompetitionID}; GameID = {Team1Player1.GameID}; " +
                $" GameTeamID = {Team1Player1.TeamID} Team Number = {Team1Player1.TeamNumber}");
            System.Diagnostics.Debug.WriteLine($"UnPackParameterVariable Team1Player2: " +
                $"CompetitionID = {Team1Player2.PlayerName} (PlayerID = {Team1Player2.PlayerID}); " +
                $"{Team1Player2.CompetitionID}; GameID = {Team1Player2.GameID}; " +
                $" GameTeamID = {Team1Player2.TeamID} Team Number = {Team1Player2.TeamNumber}");
            System.Diagnostics.Debug.WriteLine($"UnPackParameterVariable Team2Player1: " +
                $"CompetitionID = {Team2Player1.PlayerName} (PlayerID = {Team2Player1.PlayerID}); " +
                $"{Team2Player1.CompetitionID}; GameID = {Team2Player1.GameID}; " +
                $" GameTeamID = {Team2Player1.TeamID} Team Number = {Team2Player1.TeamNumber}");
            System.Diagnostics.Debug.WriteLine($"UnPackParameterVariable Team2Player2: " +
                $"CompetitionID = {Team2Player2.PlayerName} (PlayerID = {Team2Player2.PlayerID}); " +
                $"{Team2Player2.CompetitionID}; GameID = {Team2Player2.GameID}; " +
                $" GameTeamID = {Team2Player2.TeamID} Team Number = {Team2Player2.TeamNumber}"); 
            if (_isTeam3Utilised)
            {
                System.Diagnostics.Debug.WriteLine($"UnPackParameterVariable Team3Player1: " +
                    $"CompetitionID = {Team3Player1.PlayerName} (PlayerID = {Team3Player1.PlayerID}); " +
                    $"{Team3Player1.CompetitionID}; GameID = {Team3Player1.GameID}; " +
                    $" GameTeamID = {Team3Player1.TeamID} Team Number = {Team3Player1.TeamNumber}");
                System.Diagnostics.Debug.WriteLine($"UnPackParameterVariable Team2Player2: " +
                    $"CompetitionID = {Team3Player2.PlayerName} (PlayerID = {Team3Player2.PlayerID}); " +
                    $"{Team3Player2.CompetitionID}; GameID = {Team3Player2.GameID}; " +
                    $" GameTeamID = {Team3Player2.TeamID} Team Number = {Team3Player2.TeamNumber}");
            }

            (GamePlayerModel currentDealer, GamePlayerModel _) = GameServices.GetDealer(_gameID);
            GameRound.Dealer = currentDealer;
        }

    }
}
