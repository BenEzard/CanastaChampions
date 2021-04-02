CREATE VIEW vwPointsPerRound AS

WITH

Totals_CTE AS (
    SELECT
        CompetitionID,
        GameID,
        GameRoundID,
        TeamID,
        SUM(CuttingBonusCount) AS CuttingBonusCount,
        SUM(PenaltyCount) AS PenaltyCount,
        SUM(NaturalCanastaCount) AS NaturalCanastaCount,
        SUM(UnnaturalCanastaCount) AS UnnaturalCanastaCount,
        SUM(RedThreeCount) AS RedThreeCount,
        SUM(PointsOnHand) AS PointsOnHand,
        SUM(FinishingBonus) AS FinishingBonus
    FROM
        GameRoundScoreDetails
    GROUP BY
        CompetitionID,
        GameID,
        GameRoundID,
        TeamID
),

Points_CTE AS (
    SELECT
        CompetitionID,
        GameID,
        GameRoundID,
        TeamID,
        CuttingBonusCount * 100 AS CuttingBonusPoints,
        PenaltyCount * -100  AS PenaltyPoints,
        NaturalCanastaCount * 500 AS NaturalCanastaPoints,
        UnnaturalCanastaCount * 300 AS UnnaturalCanastaPoints,
        CASE
            WHEN (NaturalCanastaCount + UnnaturalCanastaCount) = 0 AND RedThreeCount < 4 THEN RedThreeCount * -100
            WHEN (NaturalCanastaCount + UnnaturalCanastaCount) = 0 AND RedThreeCount = 4 THEN RedThreeCount * -200
            WHEN (NaturalCanastaCount + UnnaturalCanastaCount) > 0 AND RedThreeCount < 4 THEN RedThreeCount * 100
            WHEN (NaturalCanastaCount + UnnaturalCanastaCount) > 0 AND RedThreeCount = 4 THEN RedThreeCount * 200
        END AS RedThreePoints,
        CASE
            WHEN (NaturalCanastaCount + UnnaturalCanastaCount) = 0 THEN PointsOnHand * -1
            WHEN (NaturalCanastaCount + UnnaturalCanastaCount) > 0 THEN PointsOnHand
        END AS PointsOnHand,
        FinishingBonus * 100 AS FinishingPoints
    FROM
        Totals_CTE
)

SELECT
    Points_CTE.CompetitionID,
    Points_CTE.GameID,
    Points_CTE.GameRoundID,
    GameRound.GameRoundNumber,
    Points_CTE.TeamID,
    vwTeam.TeamName,
    CuttingBonusPoints,
    PenaltyPoints,
    NaturalCanastaPoints,
    UnnaturalCanastaPoints,
    RedThreePoints,
    PointsOnHand,
    FinishingPoints,
    CuttingBonusPoints + PenaltyPoints + NaturalCanastaPoints + UnnaturalCanastaPoints + RedThreePoints + PointsOnHand + FinishingPoints AS TotalPoints,
    WinningTeam.TeamName AS WinningTeam,
    Players.PlayerName AS DealerName
FROM
    Points_CTE
LEFT JOIN vwTeam 
    ON Points_CTE.TeamID = vwTeam.TeamID
LEFT JOIN vwTeam WinningTeam
    ON Points_CTE.TeamID = WinningTeam.TeamID   
LEFT JOIN GameRound
    ON Points_CTE.GameRoundID = GameRound.GameRoundID
LEFT JOIN Players
    ON GameRound.DealerID = Players.PlayerID