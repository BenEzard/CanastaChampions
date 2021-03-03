using CanastaChampions.Data.DataAccess;
using CanastaChampions.Data.Models;
using CanastaChampions.Domain.Services;
using System;
using System.Collections.Generic;

namespace CanastaChampions.Cmd
{
    class Program
    {
        private const string DAVE = "Dave's Canasta Regulars";
        private const string BURGER = "Burger Empire";

        private CompetitionRepository _compRepo = new CompetitionRepository();
        private GameRepository _gameRepo = new GameRepository();
        private PlayerRepository _playerRepo = new PlayerRepository();

        public Program()
        {
            if (true)
            {
                DBMethods.PurgeDatabase();
                test_CreateCompetitions();
                test_AddPlayers();
                test_RegisterPlayersToCompetition();
                test_RegisterTeams();
                test_CreateGame();
            }
            test_doRounds();
        }

        private void test_CreateCompetitions()
        {
            _compRepo.CreateCompetition(BURGER, 
                fixedTeams: true,
                randomiseTeams: false);
            _compRepo.CreateCompetition(DAVE, 
                fixedTeams: false, 
                randomiseTeams: true);
        }

        private void test_AddPlayers()
        {
            _playerRepo.AddPlayer("Ben");
            _playerRepo.AddPlayer("Jen");
            _playerRepo.AddPlayer("Nathan");
            _playerRepo.AddPlayer("Dannii");
            _playerRepo.AddPlayer("Rowan");
            _playerRepo.AddPlayer("Poly");
            _playerRepo.AddPlayer("Drooten");
            _playerRepo.AddPlayer("Kitty");
        }

        private void test_RegisterPlayersToCompetition()
        {
            long competitionID = _compRepo.GetCompetition(BURGER).CompetitionID;
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Ben").PlayerID);
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Jen").PlayerID);
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Nathan").PlayerID);
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Dannii").PlayerID);

            competitionID = _compRepo.GetCompetition(DAVE).CompetitionID;
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Ben").PlayerID);
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Jen").PlayerID);
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Poly").PlayerID);
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Rowan").PlayerID);
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Drooten").PlayerID);
            _compRepo.RegisterPlayer(competitionID, _playerRepo.GetPlayerByName("Kitty").PlayerID);
        }

        private void test_RegisterTeams()
        {
            long competitionID = _compRepo.GetCompetition(BURGER).CompetitionID;

            _compRepo.InsertTeam(competitionID, _playerRepo.GetPlayerByName("Ben").PlayerID, _playerRepo.GetPlayerByName("Nathan").PlayerID);
            _compRepo.InsertTeam(competitionID, _playerRepo.GetPlayerByName("Jen").PlayerID, _playerRepo.GetPlayerByName("Dannii").PlayerID); 

            competitionID = _compRepo.GetCompetition(DAVE).CompetitionID;
            _compRepo.AddAllTeamCombinations(competitionID);
        }

        private void test_CreateGame()
        {
            long competitionID = _compRepo.GetCompetition(DAVE).CompetitionID;
            
            List <TeamModel> teams = new List<TeamModel>();
            teams.Add(_compRepo.GetTeam(competitionID, "Jen", "Rowan"));
            teams.Add(_compRepo.GetTeam(competitionID, "Ben", "Poly"));
            teams.Add(_compRepo.GetTeam(competitionID, "Kitty", "Drooten"));

            // Simulate the positions at the table
            PlayerModel[] playersInPosition = new PlayerModel[] {
                _playerRepo.GetPlayerByName("Jen"),
                _playerRepo.GetPlayerByName("Ben"),
                _playerRepo.GetPlayerByName("Kitty"),
                _playerRepo.GetPlayerByName("Rowan"),
                _playerRepo.GetPlayerByName("Poly"),
                _playerRepo.GetPlayerByName("Drooten"),
            };

            long gameID = _gameRepo.CreateGame(competitionID, "Ben's house", DateTime.Now, teams, playersInPosition, 1);
        }

        private void test_doRounds()
        {
            long competitionID = _compRepo.GetCompetition(DAVE).CompetitionID;
            long gameID = 1;
            TeamModel team = _compRepo.GetTeam(competitionID, "Jen", "Rowan");
            long playerID = _playerRepo.GetPlayerByName("Jen").PlayerID;

            long gameRoundID = _gameRepo.StartRound(competitionID, gameID, DateTime.Now);
            _gameRepo.AddCuttingBonus(competitionID, gameID, gameRoundID, team, playerID);

            DateTime simulatedTime = DateTime.Now.AddMinutes(20);

            _gameRepo.EndRound(competitionID, gameID, gameRoundID, simulatedTime, _compRepo.GetTeam(competitionID, "Kitty", "Drooten").TeamID);
            // When round ends, pop up a box for entering people's scores in. This will include a "winner" checkbox on one.
            // (Note there can be no winner).

            _gameRepo.InsertScoreTally(competitionID, gameID, gameRoundID,
                teamID: _compRepo.GetTeam(competitionID, "Jen", "Rowan").TeamID,
                naturalCanastaCount: 0,
                unnaturalCanastaCount: 1,
                redThreeCount: 1,
                pointsInHand: 720);
            _gameRepo.InsertScoreTally(competitionID, gameID, gameRoundID,
                teamID: _compRepo.GetTeam(competitionID, "Ben", "Poly").TeamID,
                naturalCanastaCount: 0,
                unnaturalCanastaCount: 0,
                redThreeCount: 1,
                pointsInHand: -120);
            _gameRepo.InsertScoreTally(competitionID, gameID, gameRoundID,
                teamID: _compRepo.GetTeam(competitionID, "Kitty", "Drooten").TeamID,
                naturalCanastaCount: 1,
                unnaturalCanastaCount: 1,
                redThreeCount: 1,
                pointsInHand: 35);

            simulatedTime = simulatedTime.AddMinutes(2);
            gameRoundID = _gameRepo.StartRound(competitionID, gameID, simulatedTime);

            simulatedTime = simulatedTime.AddMinutes(20).AddSeconds(37); 
            _gameRepo.EndRound(competitionID, gameID, gameRoundID, simulatedTime, _compRepo.GetTeam(competitionID, "Ben", "Poly").TeamID);
            // When round ends, pop up a box for entering people's scores in. This will include a "winner" checkbox on one.
            // (Note there can be no winner).

            _gameRepo.InsertScoreTally(competitionID, gameID, gameRoundID,
                teamID: _compRepo.GetTeam(competitionID, "Jen", "Rowan").TeamID,
                naturalCanastaCount: 0,
                unnaturalCanastaCount: 2,
                redThreeCount: 0,
                pointsInHand: 650);
            _gameRepo.InsertScoreTally(competitionID, gameID, gameRoundID,
                teamID: _compRepo.GetTeam(competitionID, "Ben", "Poly").TeamID,
                naturalCanastaCount: 1,
                unnaturalCanastaCount: 0,
                redThreeCount: 4,
                pointsInHand: 150);
            _gameRepo.InsertScoreTally(competitionID, gameID, gameRoundID,
                teamID: _compRepo.GetTeam(competitionID, "Kitty", "Drooten").TeamID,
                naturalCanastaCount: 2,
                unnaturalCanastaCount: 0,
                redThreeCount: 0,
                pointsInHand: 1035);
            _gameRepo.InsertPenalty(competitionID, gameID, gameRoundID, 
                _compRepo.GetTeam(competitionID, "Jen", "Rowan").TeamID, 
                _compRepo.GetTeam(competitionID, "Jen", "Rowan").TeamMember2ID);

            simulatedTime = simulatedTime.AddMinutes(3);
            gameRoundID = _gameRepo.StartRound(competitionID, gameID, simulatedTime);

            simulatedTime = simulatedTime.AddMinutes(13);
            _gameRepo.EndRound(competitionID, gameID, gameRoundID, simulatedTime, _compRepo.GetTeam(competitionID, "Kitty", "Drooten").TeamID);
            // When round ends, pop up a box for entering people's scores in. This will include a "winner" checkbox on one.
            // (Note there can be no winner).

            _gameRepo.InsertScoreTally(competitionID, gameID, gameRoundID,
                teamID: _compRepo.GetTeam(competitionID, "Jen", "Rowan").TeamID,
                naturalCanastaCount: 0,
                unnaturalCanastaCount: 2,
                redThreeCount: 0,
                pointsInHand: 1005);
            _gameRepo.InsertScoreTally(competitionID, gameID, gameRoundID,
                teamID: _compRepo.GetTeam(competitionID, "Ben", "Poly").TeamID,
                naturalCanastaCount: 0,
                unnaturalCanastaCount: 1,
                redThreeCount: 0,
                pointsInHand: 650);
            _gameRepo.InsertScoreTally(competitionID, gameID, gameRoundID,
                teamID: _compRepo.GetTeam(competitionID, "Kitty", "Drooten").TeamID,
                naturalCanastaCount: 0,
                unnaturalCanastaCount: 0,
                redThreeCount: 0,
                pointsInHand: -130);
        }

        static void Main(string[] args)
        {
            new Program();
        }
    }
}
