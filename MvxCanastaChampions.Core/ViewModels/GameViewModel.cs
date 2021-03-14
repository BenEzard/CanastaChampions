using CanastaChampions.DataAccess.Models;
using MvvmCross.Commands;
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
        #region Team1Members
        private GamePlayerModel _team1Member1;

        public GamePlayerModel Team1Member1
        {
            get { return _team1Member1; }
            set { 
                _team1Member1 = value;
                SetProperty(ref _team1Member1, value);
            }
        }

        private GamePlayerModel _team1Member2;

        public GamePlayerModel Team1Member2
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
        private GamePlayerModel _team2Member1;

        public GamePlayerModel Team2Member1
        {
            get { return _team2Member1; }
            set
            {
                _team2Member1 = value;
                SetProperty(ref _team2Member1, value);
            }
        }

        private GamePlayerModel _team2Member2;

        public GamePlayerModel Team2Member2
        {
            get { return _team2Member2; }
            set
            {
                _team2Member2 = value;
                SetProperty(ref _team2Member2, value);
            }
        }
        #endregion

        #region Team3Members
        private GamePlayerModel _team3Member1;

        public GamePlayerModel Team3Member1
        {
            get { return _team3Member1; }
            set
            {
                _team3Member1 = value;
                SetProperty(ref _team3Member1, value);
                RaisePropertyChanged(() => IsTeam3Utilised);
            }
        }

        private GamePlayerModel _team3Member2;

        public GamePlayerModel Team3Member2
        {
            get { return _team3Member2; }
            set
            {
                _team3Member2 = value;
                SetProperty(ref _team3Member2, value);
                RaisePropertyChanged(() => IsTeam3Utilised);
            }
        }

        public bool IsTeam3Utilised
            => (_team3Member1 == null) ? true : false;
        #endregion

        private long _competitionID;
        private long _gameID;

        private List<GamePlayerModel> _gamePlayers = null;

        public override void Prepare(List<GamePlayerModel> parameter)
        {
            //foreach (GamePlayerModel m in parameter)
            //{
            //    System.Diagnostics.Debug.WriteLine($"==> Team Member is {m.PlayerName}, team # {m.TeamNumber}");
            //}

            _gamePlayers = parameter;

            // Unpack the pass-through variable into teams
            UnPackParameterVariable(parameter);

            StartGame();
        }

        /// <summary>
        /// Unpack the List of GamePlayerModel's into individual Teams and Players.
        /// Also populates CompetitionID and GameID if they are populated within the GamePlayerModel.
        /// </summary>
        /// <param name="parameter"></param>
        private void UnPackParameterVariable(List<GamePlayerModel> parameter)
        {
            IEnumerable<GamePlayerModel> team = parameter.Where(x => x.TeamNumber == 1);

            if (team.ElementAt(0).CompetitionID != -1)
                _competitionID = team.ElementAt(0).CompetitionID;
            if (team.ElementAt(0).GameID != -1)
                _gameID = team.ElementAt(0).GameID;

            Team1Member1 = team.ElementAt(0);
            Team1Member2 = team.ElementAt(1);

            team = parameter.Where(x => x.TeamNumber == 2);
            Team2Member1 = team.ElementAt(0);
            Team2Member2 = team.ElementAt(1);

            team = parameter.Where(x => x.TeamNumber == 3);
            if (team.Count() > 0)
            {
                Team3Member1 = team.ElementAt(0);
                Team3Member2 = team.ElementAt(1);
            }
            else
            {
                Team3Member1 = null;
                Team3Member2 = null;
            }
        }

        private GameModel _game = new GameModel();

        public GameModel Game
        {
            get { return _game; }
            set { 
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
        }

        public IMvxCommand EndRoundCommand { get; set; }

        public void EndRound()
        {
            GameRound.RoundEndDateTime = DateTime.Now;

            RaisePropertyChanged(() => IsStartRoundButtonAvailable);
            RaisePropertyChanged(() => IsEndRoundButtonAvailable);
        }

        public bool IsEndRoundButtonAvailable
            => GameRound.GameRoundID == -1 ? false : true;

        public bool IsStartRoundButtonAvailable
            => GameRound.GameRoundID == -1 ? true : false;

        public GameViewModel()
        {
            StartRoundCommand = new MvxCommand(StartRound);
            EndRoundCommand = new MvxCommand(EndRound);
        }

        public void StartGame()
        {
            Game.GameStartDateTime = DateTime.Now;

            List <GamePlayerModel> playerList = GameServices.CreateGame(
                competitionID: _competitionID, 
                location: null, 
                gameStartDateTime: Game.GameStartDateTime,
                gamePlayers: _gamePlayers);

            UnPackParameterVariable(playerList);

            (PlayerModel currentDealer, PlayerModel nextDealer) = GameServices.GetDealer(_gameID);
            GameRound.Dealer = currentDealer;
        }

    }
}
