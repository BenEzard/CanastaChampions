CREATE VIEW vwRoundScoreSummary AS

WITH

GameTeams_CTE AS (
    SELECT
        vwGameTeam.GameTeamID,
        vwGameTeam.CompetitionID,
        vwGameTeam.GameID,
        vwGameTeam.Team1ID,
        vwGameTeam.Team1Name,
        vwGameTeam.Team2ID,
        vwGameTeam.Team2Name,
        vwGameTeam.Team3ID,
        vwGameTeam.Team3Name
    FROM
        vwGameTeam
)

SELECT
    GameRound.GameRoundID,
    GameRound.CompetitionID,
    GameRound.GameID,
    GameRound.GameRoundNumber,
    GameTeams_CTE.GameTeamID,
    GameTeams_CTE.Team1ID,
    GameTeams_CTE.Team1Name,
    Team1.TotalPoints AS Team1Score,
    GameTeams_CTE.Team2ID,
    GameTeams_CTE.Team2Name,
    Team2.TotalPoints AS Team2Score,
    GameTeams_CTE.Team3ID,
    GameTeams_CTE.Team3Name,
    Team3.TotalPoints AS Team3Score,
    Team1.DealerName,
    Team1.WinningTeam
FROM
    GameRound
LEFT JOIN GameTeams_CTE
    ON GameRound.CompetitionID = GameTeams_CTE.CompetitionID
    AND GameRound.GameID = GameTeams_CTE.GameID
LEFT JOIN vwPointsPerRound Team1
    ON GameTeams_CTE.CompetitionID = Team1.CompetitionID
    AND GameTeams_CTE.GameID = Team1.GameID
    AND GameTeams_CTE.Team1ID = Team1.TeamID
    AND GameRound.GameRoundID = Team1.GameRoundID
LEFT JOIN vwPointsPerRound Team2
    ON GameTeams_CTE.CompetitionID = Team2.CompetitionID
    AND GameTeams_CTE.GameID = Team2.GameID
    AND GameTeams_CTE.Team2ID = Team2.TeamID
    AND GameRound.GameRoundID = Team2.GameRoundID
LEFT JOIN vwPointsPerRound Team3
    ON GameTeams_CTE.CompetitionID = Team3.CompetitionID
    AND GameTeams_CTE.GameID = Team3.GameID
    AND GameTeams_CTE.Team3ID = Team3.TeamID
    AND GameRound.GameRoundID = Team3.GameRoundID
ORDER BY GameRound.CompetitionID,
    GameRound.GameID,
    GameRound.GameRoundNumber