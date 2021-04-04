CREATE VIEW vwGameTeam AS

    SELECT GameTeams.GameTeamID,
           GameTeams.CompetitionID,
           GameTeams.GameID,
           GameTeams.Team1ID,
           Team1.TeamMember1ID AS Team1Player1ID,
           Player1.PlayerName AS Team1Player1Name,
           Team1.TeamMember2ID AS Team1Player2ID,
           Player2.PlayerName AS Team1Player2Name,
           Player1.PlayerName || ' & ' || Player2.PlayerName AS Team1Name,
           1 AS Team1Number,

           GameTeams.Team2ID,
           Team2.TeamMember1ID AS Team2Player1ID,
           Player3.PlayerName AS Team2Player1Name,
           Team2.TeamMember2ID AS Team2Player2ID,
           Player4.PlayerName AS Team2Player2Name,
           Player3.PlayerName || ' & ' || Player4.PlayerName AS Team2Name,
           2 AS Team2Number,

           GameTeams.Team3ID,
           Team3.TeamMember1ID AS Team3Player1ID,
           Player5.PlayerName AS Team3Player1Name,
           Team3.TeamMember2ID AS Team3Player2ID,
           Player6.PlayerName AS Team3Player2Name,
           Player5.PlayerName || ' & ' || Player6.PlayerName AS Team3Name,
           3 AS Team3Number
      FROM GameTeams
           LEFT JOIN
           Team Team1 ON GameTeams.CompetitionID = Team1.CompetitionID AND 
                         GameTeams.Team1ID = Team1.TeamID
           LEFT JOIN
           Team Team2 ON GameTeams.CompetitionID = Team2.CompetitionID AND 
                         GameTeams.Team2ID = Team2.TeamID
           LEFT JOIN
           Team Team3 ON GameTeams.CompetitionID = Team3.CompetitionID AND 
                         GameTeams.Team3ID = Team3.TeamID
           LEFT JOIN
           Players Player1 ON Team1.TeamMember1ID = Player1.PlayerID
           LEFT JOIN
           Players Player2 ON Team1.TeamMember2ID = Player2.PlayerID
           LEFT JOIN
           Players Player3 ON Team2.TeamMember1ID = Player3.PlayerID
           LEFT JOIN
           Players Player4 ON Team2.TeamMember2ID = Player4.PlayerID
           LEFT JOIN
           Players Player5 ON Team3.TeamMember1ID = Player5.PlayerID
           LEFT JOIN
           Players Player6 ON Team3.TeamMember2ID = Player6.PlayerID;
