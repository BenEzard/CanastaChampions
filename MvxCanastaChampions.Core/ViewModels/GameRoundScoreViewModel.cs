using CanastaChampions.DataAccess.Models;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.Services;
using System.Collections.Generic;
using System.Media;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class GameRoundScoreViewModel : MvxViewModel<RoundModel>
    {
        private readonly IMvxNavigationService _navigationService;

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

        #region Teams
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
        #endregion

        #region ScoreBeforeRound
        private int _team1ScoreBeforeRound = 0;

        public int Team1ScoreBeforeRound
        {
            get { return _team1ScoreBeforeRound; }
            set { 
                _team1ScoreBeforeRound = value;
                SetProperty(ref _team1ScoreBeforeRound, value);
            }
        }

        private int _team2ScoreBeforeRound = 0;

        public int Team2ScoreBeforeRound
        {
            get { return _team2ScoreBeforeRound; }
            set
            {
                _team2ScoreBeforeRound = value;
                SetProperty(ref _team2ScoreBeforeRound, value);
            }
        }

        private int _team3ScoreBeforeRound = 0;

        public int Team3ScoreBeforeRound
        {
            get { return _team3ScoreBeforeRound; }
            set
            {
                _team3ScoreBeforeRound = value;
                SetProperty(ref _team3ScoreBeforeRound, value);
            }
        }
        #endregion

        #region NaturalCanasta
        private int _team1NaturalCanastaCount = 0;

        public int Team1NaturalCanastaCount
        {
            get { return _team1NaturalCanastaCount; }
            set { 
                _team1NaturalCanastaCount = value;
                SetProperty(ref _team1NaturalCanastaCount, value);

                /* below code doesn't work
                 * intent was to de-select the finish if the number of canasta's is set back to 0.
                 * if (_team1NaturalCanastaCount == 0 && _team1FinishingBonus)
                {
                    Team1FinishingBonus = false;
                }*/

                RaisePropertyChanged(() => Team1RoundScore);
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
                RaisePropertyChanged(() => Team2RoundScore);
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
                RaisePropertyChanged(() => Team3RoundScore);
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
                RaisePropertyChanged(() => Team1RoundScore);
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
                RaisePropertyChanged(() => Team2RoundScore);
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
                RaisePropertyChanged(() => Team3RoundScore);
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
                RaisePropertyChanged(() => Team1RoundScore);
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
                RaisePropertyChanged(() => Team2RoundScore);
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
                RaisePropertyChanged(() => Team3RoundScore);
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
                RaisePropertyChanged(() => Team1RoundScore);
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
                RaisePropertyChanged(() => Team2RoundScore);
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
                RaisePropertyChanged(() => Team3RoundScore);
            }
        }
        #endregion

        #region CuttingBonus
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
                        RaisePropertyChanged(() => Team1RoundScore);
                        break;
                    case 2:
                        RaisePropertyChanged(() => Team2RoundScore);
                        break;
                    case 3:
                        RaisePropertyChanged(() => Team3RoundScore);
                        break;
                }
                
            }
        }
        #endregion

        #region RoundScores
        private int _team1RoundScore;

        public int Team1RoundScore
        {
            get
            {
                _team1RoundScore = CalculateScore(1);
                return _team1RoundScore;
            }
        }

        private int _team2RoundScore;

        public int Team2RoundScore
        {
            get
            {
                _team2RoundScore = CalculateScore(2);
                return _team2RoundScore;
            }
        }
        private int _team3RoundScore;

        public int Team3RoundScore
        {
            get
            {
                _team3RoundScore = CalculateScore(3);
                return _team3RoundScore;
            }
        }
        #endregion

        #region TotalScores
        public int Team1TotalScore
            => _team1ScoreBeforeRound + Team1RoundScore;

        public int Team2TotalScore
            => _team2ScoreBeforeRound + Team2RoundScore;

        public int Team3TotalScore
            => _team3ScoreBeforeRound + Team3RoundScore;
        #endregion

        #region PenaltyCount
        private int _team1PenaltyCount = 0;

        public int Team1PenaltyCount
        {
            get { return _team1PenaltyCount; }
            set { 
                _team1PenaltyCount = value;
                SetProperty(ref _team1PenaltyCount, value);
                RaisePropertyChanged(() => Team1RoundScore);
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
                RaisePropertyChanged(() => Team2RoundScore);
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
                RaisePropertyChanged(() => Team3RoundScore);
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
                if (_team1NaturalCanastaCount + _team1UnnaturalCanastaCount == 0)
                    SystemSounds.Beep.Play();
                else
                {
                    //DeselectOtherFinishing(1);
                    _team1FinishingBonus = value;
                    SetProperty(ref _team1FinishingBonus, value);
                    RaisePropertyChanged(() => Team1RoundScore);
                    RaisePropertyChanged(() => CanScoringBeCompleted);
                }
            }
        }

        /*private void DeselectOtherFinishing(int teamSelected)
        {
            switch (teamSelected)
            {
                case 1:
                    Team2FinishingBonus = false;
                    Team3FinishingBonus = false;
                    break;
                case 2:
                    Team1FinishingBonus = false;
                    Team3FinishingBonus = false;
                    break;
                case 3:
                    Team1FinishingBonus = false;
                    Team3FinishingBonus = false;
                    break;
            }
        }*/

        private bool _team2FinishingBonus = false;

        public bool Team2FinishingBonus
        {
            get { return _team2FinishingBonus; }
            set
            {
                if (_team2NaturalCanastaCount + _team2UnnaturalCanastaCount == 0)
                    SystemSounds.Beep.Play();
                else
                {
                    _team2FinishingBonus = value;
                    SetProperty(ref _team2FinishingBonus, value);
                    RaisePropertyChanged(() => Team2RoundScore);
                    RaisePropertyChanged(() => CanScoringBeCompleted);
                }
            }
        }
        
        private bool _team3FinishingBonus = false;

        public bool Team3FinishingBonus
        {
            get { return _team3FinishingBonus; }
            set
            {
                if (_team3NaturalCanastaCount + _team3UnnaturalCanastaCount == 0)
                    SystemSounds.Beep.Play();
                else
                {
                    SetProperty(ref _team3FinishingBonus, value);
                    RaisePropertyChanged(() => Team3RoundScore);
                    RaisePropertyChanged(() => CanScoringBeCompleted);
                }
            }
        }

        #endregion

        public bool CanScoringBeCompleted
            => (Team1FinishingBonus || Team2FinishingBonus || Team3FinishingBonus) ? true : false;

        public IMvxCommand ScoringCompletedCommand { get; set; }

        public void ScoringCompleted()
        {
            SaveScores();
            FinaliseRound();

            // TODO check round for any achievements
            // TODO check total score for end of game.
            // 

            List<GamePlayerModel> players = GameServices.GetCurrentPlayers(GameRound.CompetitionID, GameRound.GameID);
            
            foreach (GamePlayerModel p in players)
                System.Diagnostics.Debug.WriteLine($"::::==> Player {p.PlayerName} on Team {p.TeamNumber}");

            _navigationService.Navigate<GameViewModel, List<GamePlayerModel>>(players);
        }

        /// <summary>
        /// Save the scores for the Round.
        /// </summary>
        public void SaveScores()
        {
            // Insert scores for each team.
            GameServices.AddTeamRoundScore(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team1.TeamID, Team1NaturalCanastaCount, Team1UnnaturalCanastaCount, Team1Red3Count, Team1PointsOnHand);
            GameServices.AddTeamRoundScore(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team2.TeamID, Team2NaturalCanastaCount, Team2UnnaturalCanastaCount, Team2Red3Count, Team2PointsOnHand);
            if (IsTeam3Playing)
            {
                GameServices.AddTeamRoundScore(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team3.TeamID, Team3NaturalCanastaCount, Team3UnnaturalCanastaCount, Team3Red3Count, Team3PointsOnHand);
            }

            // Insert Finishing Bonus
            if (Team1FinishingBonus)
                GameServices.AddFinishingBonus(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team1.TeamID);
            else if (Team2FinishingBonus)
                GameServices.AddFinishingBonus(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team2.TeamID);
            else if (Team3FinishingBonus)
                GameServices.AddFinishingBonus(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, Team3.TeamID);

            // Insert Cutting Bonus (if applicable)
            if (CuttingBonus)
                GameServices.AddRoundCuttingBonus(GameRound.CompetitionID, GameRound.GameID, GameRound.GameRoundID, 
                    GameRound.Dealer.TeamID, GameRound.Dealer.PlayerID);
        }

        /// <summary>
        /// Finalise the Game Round.
        /// </summary>
        public void FinaliseRound()
        {
            if (Team1FinishingBonus)
                GameRound.WinningTeamID = Team1.TeamID;
            else if (Team2FinishingBonus)
                GameRound.WinningTeamID = Team2.TeamID;
            else if (Team3FinishingBonus)
                GameRound.WinningTeamID = Team3.TeamID;

            GameServices.FinaliseRound(GameRound);
        }

        public GameRoundScoreViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
            ScoringCompletedCommand =  new MvxCommand(ScoringCompleted);
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

        /// <summary>
        /// Calculate Score for the given consecutive team number
        /// </summary>
        /// <param name="teamNumber"></param>
        /// <returns></returns>
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

            totalScore += pointsOnHand;
            }
            else {  // hasCanasta == false
                if (red3Count == 4)
                    totalScore -= 800;
                else
                    totalScore -= red3Count * 100;

                totalScore -= pointsOnHand;
            }

            if (finishingBonus && hasCanasta)
                totalScore += 100;

            if (CuttingBonus && teamNumber == GameRound.Dealer.TeamNumber)
                totalScore += 100;

            totalScore -= penaltyCount * 100;

            return totalScore;
        }
    }
}
