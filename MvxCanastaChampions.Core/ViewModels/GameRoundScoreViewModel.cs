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
    public class GameRoundScoreViewModel : MvxViewModel<List<GamePlayerModel>>
    {
        private long _competitionID = -1;
        private long _gameID = -1;
        private long _gameRoundID = -1;


        #region teamNames
        private string _team1Name;

        public string Team1Name
        {
            get { return _team1Name; }
            set { 
                _team1Name = value;
                SetProperty(ref _team1Name, value);
            }
        }

        private string _team2Name;

        public string Team2Name
        {
            get { return _team2Name; }
            set
            {
                _team2Name = value;
                SetProperty(ref _team2Name, value);
            }
        }

        private string _team3Name;

        public string Team3Name
        {
            get { return _team3Name; }
            set
            {
                _team3Name = value;
                SetProperty(ref _team3Name, value);
                RaisePropertyChanged(() => IsTeam3Playing);
            }
        }

        public bool IsTeam3Playing
        {
            get => (String.IsNullOrEmpty(_team3Name)) ? false : true;
        }
        #endregion

        #region
        private long _team1ID = -1;
        private long _team2ID = -1;
        private long _team3ID = -1;
        #endregion

        #region NaturalCanasta
        private int _team1NaturalCanastaCount = 0;

        public int Team1NaturalCanastaCount
        {
            get { return _team1NaturalCanastaCount; }
            set { 
                _team1NaturalCanastaCount = value;
                SetProperty(ref _team1NaturalCanastaCount, value);
                RaisePropertyChanged(() => Team1Score);
            }
        }

        private int _team2NaturalCanastaCount = 0;

        public int Team2NaturalCanastaCount
        {
            get { return _team2NaturalCanastaCount; }
            set
            {
                _team2NaturalCanastaCount = value;
                SetProperty(ref _team2NaturalCanastaCount, value);
                RaisePropertyChanged(() => Team2Score);
            }
        }

        private int _team3NaturalCanastaCount = 0;

        public int Team3NaturalCanastaCount
        {
            get { return _team3NaturalCanastaCount; }
            set
            {
                _team3NaturalCanastaCount = value;
                SetProperty(ref _team3NaturalCanastaCount, value);
                RaisePropertyChanged(() => Team3Score);
            }
        }
        #endregion

        #region UnnaturalCanasta
        private int _team1UnnaturalCanastaCount = 0;

        public int Team1UnnaturalCanastaCount
        {
            get { return _team1UnnaturalCanastaCount; }
            set
            {
                _team1UnnaturalCanastaCount = value;
                SetProperty(ref _team1UnnaturalCanastaCount, value);
                RaisePropertyChanged(() => Team1Score);
            }
        }

        private int _team2UnnaturalCanastaCount = 0;

        public int Team2UnnaturalCanastaCount
        {
            get { return _team2UnnaturalCanastaCount; }
            set
            {
                _team2UnnaturalCanastaCount = value;
                SetProperty(ref _team2UnnaturalCanastaCount, value);
                RaisePropertyChanged(() => Team2Score);
            }
        }

        private int _team3UnnaturalCanastaCount = 0;

        public int Team3UnnaturalCanastaCount
        {
            get { return _team3UnnaturalCanastaCount; }
            set
            {
                _team3UnnaturalCanastaCount = value;
                SetProperty(ref _team3UnnaturalCanastaCount, value);
                RaisePropertyChanged(() => Team3Score);
            }
        }
        #endregion

        #region Red3Count
        private int _team1Red3Count = 0;

        public int Team1Red3Count
        {
            get { return _team1Red3Count; }
            set
            {
                _team1Red3Count = value;
                SetProperty(ref _team1Red3Count, value);
                RaisePropertyChanged(() => Team1Score);
            }
        }

        private int _team2Red3Count = 0;

        public int Team2Red3Count
        {
            get { return _team2Red3Count; }
            set
            {
                _team2Red3Count = value;
                SetProperty(ref _team2Red3Count, value);
                RaisePropertyChanged(() => Team2Score);
            }
        }

        private int _team3Red3Count = 0;

        public int Team3Red3Count
        {
            get { return _team3Red3Count; }
            set
            {
                _team3Red3Count = value;
                SetProperty(ref _team3Red3Count, value);
                RaisePropertyChanged(() => Team3Score);
            }
        }
        #endregion

        #region PointsOnHand
        private int _team1PointsOnHand = 0;

        public int Team1PointsOnHand
        {
            get { return _team1PointsOnHand; }
            set
            {
                _team1PointsOnHand = value;
                SetProperty(ref _team1PointsOnHand, value);
                RaisePropertyChanged(() => Team1Score);
            }
        }

        private int _team2PointsOnHand = 0;

        public int Team2PointsOnHand
        {
            get { return _team2PointsOnHand; }
            set
            {
                _team2PointsOnHand = value;
                SetProperty(ref _team2PointsOnHand, value);
                RaisePropertyChanged(() => Team2Score);
            }
        }

        private int _team3PointsOnHand = 0;

        public int Team3PointsOnHand
        {
            get { return _team3PointsOnHand; }
            set
            {
                _team3PointsOnHand = value;
                SetProperty(ref _team3PointsOnHand, value);
                RaisePropertyChanged(() => Team3Score);
            }
        }
        #endregion

        private bool _cuttingBonus;

        public bool CuttingBonus
        {
            get { return _cuttingBonus; }
            set { 
                _cuttingBonus = value;
                SetProperty(ref _cuttingBonus, value);
                switch (DealerTeam)
                {
                    case 0:
                        break;
                    case 1:
                        RaisePropertyChanged(() => Team1Score);
                        break;
                    case 2:
                        RaisePropertyChanged(() => Team2Score);
                        break;
                    case 3:
                        RaisePropertyChanged(() => Team3Score);
                        break;
                }
                
            }
        }


        #region TotalScores
        private int _team1TotalScore;

        public int Team1Score
        {
            get
            {
                _team1TotalScore = CalculateScore(1);
                return _team1TotalScore;
            }
        }

        private int _team2TotalScore;

        public int Team2Score
        {
            get
            {
                _team2TotalScore = CalculateScore(2);
                return _team2TotalScore;
            }
        }
        private int _team3TotalScore;

        public int Team3Score
        {
            get
            {
                _team1TotalScore = CalculateScore(2);
                return _team3TotalScore;
            }
        }
        #endregion

        #region
        private int _team1PenaltyCount = 0;

        public int Team1PenaltyCount
        {
            get { return _team1PenaltyCount; }
            set { 
                _team1PenaltyCount = value;
                SetProperty(ref _team1PenaltyCount, value);
                RaisePropertyChanged(() => Team1Score);
            }
        }

        private int _team2PenaltyCount = 0;

        public int Team2PenaltyCount
        {
            get { return _team2PenaltyCount; }
            set
            {
                _team2PenaltyCount = value;
                SetProperty(ref _team2PenaltyCount, value);
                RaisePropertyChanged(() => Team2Score);
            }
        }

        private int _team3PenaltyCount = 0;

        public int Team3PenaltyCount
        {
            get { return _team3PenaltyCount; }
            set
            {
                _team3PenaltyCount = value;
                SetProperty(ref _team3PenaltyCount, value);
                RaisePropertyChanged(() => Team3Score);
            }
        }
        #endregion

        #region FinishingBonus
        private bool _team1FinishingBonus = false;

        public bool Team1FinishingBonus
        {
            get { return _team1FinishingBonus; }
            set
            {
                _team1FinishingBonus = value;
                SetProperty(ref _team1FinishingBonus, value);
                RaisePropertyChanged(() => Team1Score);
            }
        }

        private bool _team2FinishingBonus = false;

        public bool Team2FinishingBonus
        {
            get { return _team2FinishingBonus; }
            set
            {
                _team2FinishingBonus = value;
                SetProperty(ref _team2FinishingBonus, value);
                RaisePropertyChanged(() => Team2Score);
            }
        }
        
        private bool _team3FinishingBonus = false;

        public bool Team3FinishingBonus
        {
            get { return _team3FinishingBonus; }
            set
            {
                _team3FinishingBonus = value;
                SetProperty(ref _team3FinishingBonus, value);
                RaisePropertyChanged(() => Team3Score);
            }
        }

        #endregion

        private int _dealerTeam = 0;
        public int DealerTeam
        {
            get => _dealerTeam;
            set
            {
                _dealerTeam = value;
                SetProperty(ref _dealerTeam, value);
            }
        }

        public IMvxCommand ScoringCompletedCommand { get; set; }

        public void CompleteScoring()
        {
            // Insert scores for each team.
            GameServices.AddTeamRoundScore(_competitionID, _gameID, _gameRoundID, _team1ID, Team1NaturalCanastaCount, Team1UnnaturalCanastaCount, Team1Red3Count, Team1PointsOnHand);
            GameServices.AddTeamRoundScore(_competitionID, _gameID, _gameRoundID, _team2ID, Team2NaturalCanastaCount, Team2UnnaturalCanastaCount, Team2Red3Count, Team2PointsOnHand);
            GameServices.AddTeamRoundScore(_competitionID, _gameID, _gameRoundID, _team3ID, Team3NaturalCanastaCount, Team3UnnaturalCanastaCount, Team3Red3Count, Team3PointsOnHand);

            // Insert Cutting Bonus (if applicable)
            /*if (CuttingBonus)
                GameServices.AddRoundCuttingBonus(_competitionID, _gameID, _gameRoundID, DealerTeam, Dea)*/
        }

        public GameRoundScoreViewModel()
        {
            ScoringCompletedCommand =  new MvxCommand(CompleteScoring);
        }

        public override void Prepare(List<GamePlayerModel> parameter)
        {
            IEnumerable<GamePlayerModel> team = parameter.Where(x => x.TeamNumber == 1);
            Team1Name = $"{team.ElementAt(0).PlayerName} & {team.ElementAt(1).PlayerName}";
            _team1ID = team.ElementAt(0).TeamID;
            
            _competitionID = team.ElementAt(0).CompetitionID;
            _gameID = team.ElementAt(0).GameID;
            _gameRoundID = GameServices.GetRoundNumber(_competitionID, _gameID);


            team = parameter.Where(x => x.TeamNumber == 2);
            Team2Name = $"{team.ElementAt(0).PlayerName} & {team.ElementAt(1).PlayerName}";
            _team2ID = team.ElementAt(0).TeamID;

            team = parameter.Where(x => x.TeamNumber == 3);
            if (team.Count() > 0)
            {
                Team3Name = $"{team.ElementAt(0).PlayerName} & {team.ElementAt(1).PlayerName}";
                _team3ID = team.ElementAt(0).TeamID;
            }

            // Load Penalties
            Team1PenaltyCount = GameServices.GetTeamPenaltyCount(_competitionID, _gameID, _gameRoundID, _team1ID);
            Team2PenaltyCount = GameServices.GetTeamPenaltyCount(_competitionID, _gameID, _gameRoundID, _team1ID);
            Team3PenaltyCount = GameServices.GetTeamPenaltyCount(_competitionID, _gameID, _gameRoundID, _team1ID);

            // Get Dealer
            (PlayerModel dealer, _) = GameServices.GetDealer(_gameID);
            DealerTeam = IsOnTeam(dealer);

        }

        /// <summary>
        /// Return the team where the player name is found.
        /// TODO NOTE this means that player's names must be unique within a game.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public int IsOnTeam(PlayerModel player)
        {
            int rValue = 0;

            if (Team1Name.Contains(player.PlayerName))
                rValue = 1;
            else if (Team2Name.Contains(player.PlayerName))
                rValue = 2;
            else if (Team3Name.Contains(player.PlayerName))
                rValue = 3;

            return rValue;
        }

        public int CalculateScore(int teamNumber)
        {
            // Setup temp variables
            int totalScore = 0;
            bool hasCanasta = false;
            int naturalCanastaCount = 0;
            int unnaturalCanastaCount = 0;
            int red3Count = 0;
            int penaltyCount = 0;
            int pointsOnHand = 0;
            bool finishingBonus = false;

            // Assign the appropriate team values
            switch (teamNumber)
            {
                case 1:
                    naturalCanastaCount = Team1NaturalCanastaCount;
                    unnaturalCanastaCount = Team1UnnaturalCanastaCount;
                    red3Count = Team1Red3Count;
                    penaltyCount = Team1PenaltyCount;
                    pointsOnHand = Team1PointsOnHand;
                    finishingBonus = Team1FinishingBonus;
                    break;
                case 2:
                    naturalCanastaCount = Team2NaturalCanastaCount;
                    unnaturalCanastaCount = Team2UnnaturalCanastaCount;
                    red3Count = Team2Red3Count;
                    penaltyCount = Team2PenaltyCount;
                    pointsOnHand = Team2PointsOnHand;
                    finishingBonus = Team2FinishingBonus; 
                    break;
                case 3:
                    naturalCanastaCount = Team3NaturalCanastaCount;
                    unnaturalCanastaCount = Team3UnnaturalCanastaCount;
                    red3Count = Team3Red3Count;
                    penaltyCount = Team3PenaltyCount;
                    pointsOnHand = Team3PointsOnHand;
                    finishingBonus = Team3FinishingBonus;
                    break;
            }

            // Do the calculations
            if (naturalCanastaCount + unnaturalCanastaCount > 0)
                hasCanasta = true;

            totalScore += naturalCanastaCount * 500;
            totalScore += unnaturalCanastaCount * 300;
            if (hasCanasta) {
                if (red3Count == 4)
                    totalScore += 800;
                else
                    totalScore += red3Count * 100;
            }
            else {  // hasCanasta == false
                if (red3Count == 4)
                    totalScore -= 800;
                else
                    totalScore -= red3Count * 100;
            }
            totalScore += pointsOnHand;

            if (finishingBonus)
                totalScore += 100;

            if (CuttingBonus && teamNumber == DealerTeam)
                totalScore += 100;

            return totalScore;
        }
    }
}
