CREATE VIEW vwPointsPerGame AS

SELECT
    CompetitionID,
    GameID,
    TeamID,
    TeamNames,
    SUM(TotalPoints) AS TotalPoints,
    CASE
        WHEN SUM(TotalPoints) < 1500 THEN 50
        WHEN SUM(TotalPoints) < 3000 THEN 90
        WHEN SUM(TotalPoints) < 5000 THEN 120
    END AS TargetPoints
FROM
    vwPointsPerRound
GROUP BY
    CompetitionID,
    GameID,
    TeamID,
    TeamNames