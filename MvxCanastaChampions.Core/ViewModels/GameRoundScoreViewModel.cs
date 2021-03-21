﻿using CanastaChampions.DataAccess.Models;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.Services;
using System.Collections.Generic;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class GameRoundScoreViewModel : MvxViewModel<RoundModel>
    {
        private RoundModel _gameRound;
        public RoundModel GameRound
        {
            get => _gameRound;
            set
            {
                _gameRound = value;
                SetProperty(ref _gameRound, value);
            }
        }

        private bool _isTeam3Playing = false;
        public bool IsTeam3Playing
        {
            get => _isTeam3Playing;
            set
            {
                _isTeam3Playing = value;
                SetProperty(ref _isTeam3Playing, value);
            }
        }

        private TeamModel _team1;

        public TeamModel Team1
        {
            get { return _team1; }
            set { 
                _team1 = value;
                SetProperty(ref _team1, value);
            }
        }

        private TeamModel _team2;

        public TeamModel Team2
        {
            get { return _team2; }
            set
            {
                _team2 = value;
                SetProperty(ref _team2, value);
            }
        }

        private TeamModel _team3;

        public TeamModel Team3
        {
            get { return _team3; }
            set
            {
                _team3 = value;
                SetProperty(ref _team3, value);
            }
        }


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
                switch (GameRound?.Dealer?.TeamNumber)
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

        public IMvxCommand ScoringCompletedCommand { get; set; }

        public void CompleteScoring()
        {
            // Insert scores for each team.
            GameServices.AddTeamRoundScore(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team1.TeamID, Team1NaturalCanastaCount, Team1UnnaturalCanastaCount, Team1Red3Count, Team1PointsOnHand);
            GameServices.AddTeamRoundScore(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team2.TeamID, Team2NaturalCanastaCount, Team2UnnaturalCanastaCount, Team2Red3Count, Team2PointsOnHand);
            GameServices.AddTeamRoundScore(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team3.TeamID, Team3NaturalCanastaCount, Team3UnnaturalCanastaCount, Team3Red3Count, Team3PointsOnHand);

            // Insert Cutting Bonus (if applicable)
            if (CuttingBonus)
                GameServices.AddRoundCuttingBonus(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, 
                    GameRound.Dealer.TeamID, GameRound.Dealer.PlayerID);
        }

        public GameRoundScoreViewModel()
        {
            ScoringCompletedCommand =  new MvxCommand(CompleteScoring);
        }

        public override void Prepare(RoundModel parameter)
        {
            GameRound = parameter;
            System.Diagnostics.Debug.WriteLine($"==> Dealer is {GameRound.Dealer.PlayerName} on Team {GameRound.Dealer.TeamNumber}");

            List<TeamModel> teams = GameServices.GetTeams(GameRound.CompetitionID, GameRound.GameID);

            Team1 = teams[0];
            Team2 = teams[1];

            if (teams.Count > 2)
                Team3 = teams[2];

            // Load Penalties
            Team1PenaltyCount = GameServices.GetTeamPenaltyCount(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team1.TeamID);
            Team2PenaltyCount = GameServices.GetTeamPenaltyCount(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team2.TeamID);

            if (IsTeam3Playing)
                Team3PenaltyCount = GameServices.GetTeamPenaltyCount(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team3.TeamID);
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

            if (CuttingBonus && teamNumber == GameRound.Dealer.TeamNumber)
                totalScore += 100;

            return totalScore;
        }
    }
}
