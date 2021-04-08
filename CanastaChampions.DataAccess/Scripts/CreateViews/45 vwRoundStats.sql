CREATE VIEW vwRoundStats AS

WITH

RoundTimes_CTE AS (
    SELECT
        GameRoundID,
        CompetitionID,
        GameID,
        GameRoundNumber,
        StartOfRoundDateTime,
        EndOfRoundDateTime,
        CAST((julianday(EndOfRoundDateTime) - julianday(StartOfRoundDateTime)) * 24 * 60 * 60 AS Integer) AS RoundDateTimeInSeconds
    FROM 
        GameRound
),

QuickestRound_CTE AS (
    SELECT
         CompetitionID,
         MIN(RoundDateTimeInSeconds) AS Value,
         'FASTEST_ROUND' AS label
    FROM
        RoundTimes_CTE
    GROUP BY
        CompetitionID
 ),
 
LongestRound_CTE AS (
    SELECT
         CompetitionID,
         MAX(RoundDateTimeInSeconds) AS Value,
         'LONGEST ROUND' AS label
    FROM
        RoundTimes_CTE
    GROUP BY
        CompetitionID
),

MinPointsPerRound_CTE AS (
    SELECT
        CompetitionID,
        MIN(TotalPoints) AS Value,
        'LOWEST_SCORE_IN_A_ROUND' AS label
    FROm
        vwPointsPerRound
    WHERE
        TotalPoints > 0
    GROUP BY
        CompetitionID
),

MaxPointsPerRound_CTE AS (
    SELECT
        CompetitionID,
        MAX(TotalPoints) AS Value,
        'HIGHEST_SCORE_IN_A_ROUND' AS label
    FROm
        vwPointsPerRound
    GROUP BY
        CompetitionID
),

MaxNaturalCanastas_CTE AS (
    SELECT
        CompetitionID,
        MAX(NaturalCanastaCount) AS Value,
        'MOST_NATURAL_CANASTAS_IN_A_ROUND' AS label
    FROm
        GameRoundScoreDetails
    WHERE
        NaturalCanastaCount > 0
    GROUP BY
        CompetitionID
),

MaxUnnaturalCanastas_CTE AS (
    SELECT
        CompetitionID,
        MAX(UnnaturalCanastaCount) AS Value,
        'MOST_UNNATURAL_CANASTAS_IN_A_ROUND' AS label
    FROm
        GameRoundScoreDetails
    WHERE
        UnnaturalCanastaCount > 0
    GROUP BY
        CompetitionID
),

MaxTotalCanastas_CTE AS (
    SELECT
        CompetitionID,
        MAX(NaturalCanastaCount + UnnaturalCanastaCount) AS Value,
        'MOST_CANASTAS_IN_A_ROUND' AS label
    FROm
        GameRoundScoreDetails
    WHERE
        NaturalCanastaCount + UnnaturalCanastaCount > 0
    GROUP BY
        CompetitionID
),

MaxPenalties_CTE AS (
    SELECT
        CompetitionID,
        MAX(PenaltyCount) AS Value,
        'MOST_PENALTIES_IN_A_ROUND' AS label
    FROm
        GameRoundScoreDetails
    WHERE
        PenaltyCount > 0
    GROUP BY
        CompetitionID
)
 
SELECT * FROM QuickestRound_CTE
UNION SELECT * FROM LongestRound_CTE
UNION SELECT * FROM MaxPointsPerRound_CTE
UNION SELECT * FROM MinPointsPerRound_CTE
UNION SELECT * FROM MaxNaturalCanastas_CTE
UNION SELECT * FROM MaxUnnaturalCanastas_CTE
UNION SELECT * FROM MaxTotalCanastas_CTE
UNION SELECT * FROM MaxPenalties_CTE