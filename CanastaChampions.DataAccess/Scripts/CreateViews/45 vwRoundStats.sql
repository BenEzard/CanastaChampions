CREATE VIEW vwRoundStats AS

WITH

RoundSpeed_CTE AS (
    SELECT
        GameRoundID,
        CompetitionID,
        GameID,
        GameRoundNumber,
        (strftime('%s',EndOfRoundDateTime) - strftime('%s',StartOfRoundDateTime)) AS RoundTime_Seconds
    FROM
        GameRound
)

SELECT
    subMax.CompetitionID,
    subMax.GameID,
    subMax.MaxTotalPoints,
    vwPointsPerRound.GameRoundID,
    vwPointsPerRound.TeamID,
    vwPointsPerRound.TeamNames
FROM (
    SELECT
        CompetitionID,
        GameID,
        MAX(TotalPoints) AS MaxTotalPoints
    FROM
        vwPointsPerRound
    GROUP BY
        CompetitionID,
        GameID
    ) subMax
LEFT JOIN vwPointsPerRound
    ON subMax.GameID = vwPointsPerRound.GameID
    AND subMax.MaxTotalPoints = vwPointsPerRound.TotalPoints