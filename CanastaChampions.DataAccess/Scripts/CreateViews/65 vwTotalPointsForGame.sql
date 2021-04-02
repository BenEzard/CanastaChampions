CREATE VIEW vwTotalPointsForGame AS

SELECT
    CompetitionID,
    GameID,
    TeamID,
    SUM(TotalPoints) AS TotalPoints
FROM 
    vwPointsPerRound
GROUP BY
    CompetitionID,
    GameID,
    TeamID