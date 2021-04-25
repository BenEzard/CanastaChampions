CREATE VIEW vwGameTeam AS

WITH
PenaltyCount_CTE AS (
    SELECT CompetitionID,
           GameID,
           PlayerID,
           SUM(PenaltyCount) AS TotalPenaltyCount
      FROM GameRoundScoreDetails
     GROUP BY CompetitionID,
              GameID,
              PlayerID
)

SELECT GameTeams.GameTeamID,
       GameTeams.CompetitionID,
       GameTeams.GameID,
       GameTeams.Team1ID,
       Team1.TeamMember1ID AS Team1Player1ID,
       Player1.PlayerName AS Team1Player1Name,
       IFNULL(T1P1Penalty.TotalPenaltyCount, 0) AS Team1Player1PenaltyCount,
       Team1.TeamMember2ID AS Team1Player2ID,
       Player2.PlayerName AS Team1Player2Name,
       IFNULL(T1P2Penalty.TotalPenaltyCount, 0) AS Team1Player2PenaltyCount,
       Player1.PlayerName || ' & ' || Player2.PlayerName AS Team1Name,
       1 AS Team1Number,
       GameTeams.Team2ID,
       Team2.TeamMember1ID AS Team2Player1ID,
       Player3.PlayerName AS Team2Player1Name,
       IFNULL(T2P1Penalty.TotalPenaltyCount, 0) AS Team2Player1PenaltyCount,
       Team2.TeamMember2ID AS Team2Player2ID,
       Player4.PlayerName AS Team2Player2Name,
       IFNULL(T2P2Penalty.TotalPenaltyCount, 0) AS Team2Player2PenaltyCount,
       Player3.PlayerName || ' & ' || Player4.PlayerName AS Team2Name,
       2 AS Team2Number,
       GameTeams.Team3ID,
       Team3.TeamMember1ID AS Team3Player1ID,
       Player5.PlayerName AS Team3Player1Name,
       IFNULL(T3P1Penalty.TotalPenaltyCount, 0) AS Team3Player1PenaltyCount,
       Team3.TeamMember2ID AS Team3Player2ID,
       Player6.PlayerName AS Team3Player2Name,
       IFNULL(T3P2Penalty.TotalPenaltyCount, 0) AS Team3Player2PenaltyCount,
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
       Players Player6 ON Team3.TeamMember2ID = Player6.PlayerID
       LEFT JOIN PenaltyCount_CTE T1P1Penalty
                        ON GameTeams.GameID = T1P1Penalty.GameID
                        AND Player1.PlayerID = T1P1Penalty.PlayerID
                    LEFT JOIN PenaltyCount_CTE T1P2Penalty
                        ON GameTeams.GameID = T1P2Penalty.GameID
                        AND Player2.PlayerID = T1P2Penalty.PlayerID
                    LEFT JOIN PenaltyCount_CTE T2P1Penalty
                        ON GameTeams.GameID = T2P1Penalty.GameID
                        AND Player3.PlayerID = T2P1Penalty.PlayerID
                    LEFT JOIN PenaltyCount_CTE T2P2Penalty
                        ON GameTeams.GameID = T2P2Penalty.GameID
                        AND Player4.PlayerID = T2P2Penalty.PlayerID
                    LEFT JOIN PenaltyCount_CTE T3P1Penalty
                        ON GameTeams.GameID = T3P1Penalty.GameID
                        AND Player5.PlayerID = T3P1Penalty.PlayerID
                    LEFT JOIN PenaltyCount_CTE T3P2Penalty
                        ON GameTeams.GameID = T3P2Penalty.GameID
                        AND Player6.PlayerID = T3P2Penalty.PlayerID;