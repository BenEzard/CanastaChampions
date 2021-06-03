WITH

-- Get only games that were completed.
--        Includes who won the game and their ending score.
CompletedGamesOnly_CTE AS (
    SELECT
        vwTotalPointsForGame.CompetitionID,
        vwTotalPointsForGame.GameID,
        vwTotalPointsForGame.TeamID AS WinningTeamID,
        vwTotalPointsForGame.TotalPoints
    FROM
        vwTotalPointsForGame
    INNER JOIN (
        SELECT
            CompetitionID,
            GameID,
            MAX(TotalPoints) as MaxPointsInGame
        FROM
            vwTotalPointsForGame
        GROUP BY
            CompetitionID,
            GameID
        HAVING
            MAX(TotalPoints) >= 5000
    ) subFinishedOnly
    ON vwTotalPointsForGame.CompetitionID = subFinishedOnly.CompetitionID
    AND vwTotalPointsForGame.GameID = subFinishedOnly.GameID
    AND vwTotalPointsForGame.TotalPoints = subFinishedOnly.MaxPointsInGame
),

RoundTimes_CTE AS (
    SELECT
        GameRound.GameRoundID,
        GameRound.CompetitionID,
        GameRound.GameID,
        GameRound.GameRoundNumber,
        GameRound.StartOfRoundDateTime,
        GameRound.EndOfRoundDateTime,
        GameRound.WinningTeamID,
        CAST((julianday(GameRound.EndOfRoundDateTime) - julianday(GameRound.StartOfRoundDateTime)) * 24 * 60 * 60 AS Integer) AS RoundTimeInSeconds
    FROM 
        GameRound
    INNER JOIN CompletedGamesOnly_CTE
        ON GameRound.CompetitionID = CompletedGamesOnly_CTE.CompetitionID
        AND GameRound.GameID = CompletedGamesOnly_CTE.GameID
),

FastestRound_CTE AS (
    SELECT 
        RoundTimes_CTE.CompetitionID,
        RoundTimes_CTE.GameID,
        RoundTimes_CTE.GameRoundNumber,
        RoundTimes_CTE.StartOfRoundDateTime,
        vwTeam.TeamName AS WinningTeamName,
        subFastest.RoundTimeInSeconds,
        subFastest.label
    FROM
        RoundTimes_CTE
    INNER JOIN (    
        SELECT
             CompetitionID,
             MIN(RoundTimeInSeconds) AS RoundTimeInSeconds,
             'FASTEST_ROUND' AS label
        FROM
            RoundTimes_CTE
        GROUP BY
            CompetitionID
    ) subFastest 
        ON RoundTimes_CTE.CompetitionID = subFastest.CompetitionID
        AND RoundTimes_CTE.RoundTimeInSeconds = subFastest.RoundTimeInSeconds
    LEFT JOIN vwTeam ON RoundTimes_CTE.WinningTeamID = vwTeam.TeamID
),
 
SlowestRound_CTE AS (
    SELECT 
        RoundTimes_CTE.CompetitionID,
        RoundTimes_CTE.GameID,
        RoundTimes_CTE.GameRoundNumber,
        RoundTimes_CTE.StartOfRoundDateTime,
        vwTeam.TeamName AS WinningTeamName,
        subSlowest.RoundTimeInSeconds,
        subSlowest.label
    FROM
        RoundTimes_CTE
    INNER JOIN (    
        SELECT
             CompetitionID,
             MAX(RoundTimeInSeconds) AS RoundTimeInSeconds,
             'SLOWEST_ROUND' AS label
        FROM
            RoundTimes_CTE
        GROUP BY
            CompetitionID
    ) subSlowest
        ON RoundTimes_CTE.CompetitionID = subSlowest.CompetitionID
        AND RoundTimes_CTE.RoundTimeInSeconds = subSlowest.RoundTimeInSeconds
    LEFT JOIN vwTeam ON RoundTimes_CTE.WinningTeamID = vwTeam.TeamID
 ),

GameTimes_CTE AS ( 
    SELECT
        Game.CompetitionID,
        Game.GameID,
        Game.CompetitionGameNumber,
        Game.GameStartDateTime,
        CAST((julianday(Game.GameEndDateTime) - julianday(Game.GameStartDateTime)) * 24 * 60 * 60 AS Integer) AS GameTimeInSeconds
    FROM
        Game
        INNER JOIN CompletedGamesOnly_CTE
            ON Game.CompetitionID = CompletedGamesOnly_CTE.CompetitionID
            AND Game.GameID = CompletedGamesOnly_CTE.GameID
),

SlowestGame_CTE AS (
    SELECT 
        GameTimes_CTE.CompetitionID,
        GameTimes_CTE.GameID,
        GameTimes_CTE.CompetitionGameNumber,
        GameTimes_CTE.GameStartDateTime,
        vwTeam.TeamName AS WinningTeamName,
        subSlowest.GameTimeInSeconds,
        subSlowest.label
    FROM
        GameTimes_CTE
    INNER JOIN (    
        SELECT
             CompetitionID,
             MAX(GameTimeInSeconds) AS GameTimeInSeconds,
             'SLOWEST_GAME' AS label
        FROM
            GameTimes_CTE
        GROUP BY
            CompetitionID
    ) subSlowest
        ON GameTimes_CTE.CompetitionID = subSlowest.CompetitionID
        AND GameTimes_CTE.GameTimeInSeconds = subSlowest.GameTimeInSeconds
    LEFT JOIN CompletedGamesOnly_CTE
        ON GameTimes_CTE.CompetitionID = CompletedGamesOnly_CTE.CompetitionID
        AND GameTimes_CTE.GameID = CompletedGamesOnly_CTE.GameID
    LEFT JOIN vwTeam ON CompletedGamesOnly_CTE.WinningTeamID = vwTeam.TeamID
 ),
 
FastestGame_CTE AS (
    SELECT 
        GameTimes_CTE.CompetitionID,
        GameTimes_CTE.GameID,
        GameTimes_CTE.CompetitionGameNumber,
        GameTimes_CTE.GameStartDateTime,
        vwTeam.TeamName AS WinningTeamName,
        subFastest.GameTimeInSeconds,
        subFastest.label
    FROM
        GameTimes_CTE
    INNER JOIN (    
        SELECT
             CompetitionID,
             MIN(GameTimeInSeconds) AS GameTimeInSeconds,
             'FASTEST_GAME' AS label
        FROM
            GameTimes_CTE
        GROUP BY
            CompetitionID
    ) subFastest
        ON GameTimes_CTE.CompetitionID = subFastest.CompetitionID
        AND GameTimes_CTE.GameTimeInSeconds = subFastest.GameTimeInSeconds
    LEFT JOIN CompletedGamesOnly_CTE
        ON GameTimes_CTE.CompetitionID = CompletedGamesOnly_CTE.CompetitionID
        AND GameTimes_CTE.GameID = CompletedGamesOnly_CTE.GameID
    LEFT JOIN vwTeam ON CompletedGamesOnly_CTE.WinningTeamID = vwTeam.TeamID
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
 
 
SELECT * FROM FastestRound_CTE
UNION SELECT * FROM SlowestRound_CTE
UNION SELECT * FROM SlowestGame_CTE
UNION SELECT * FROM FastestGame_CTE


/*

/*MaxPointsPerRound_CTE AS (
    SELECT
        CompetitionID,
        MAX(TotalPoints) AS Value,
        'HIGHEST_SCORE_IN_A_ROUND' AS label
    FROm
        vwPointsPerRound
    GROUP BY
        CompetitionID
),

/*MaxNaturalCanastas_CTE AS (
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

/*MaxUnnaturalCanastas_CTE AS (
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

/*MaxTotalCanastas_CTE AS (
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

/*MaxPenalties_CTE AS (
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
)*/
 
/*UNION SELECT * FROM MaxPointsPerRound_CTE
UNION SELECT * FROM MinPointsPerRound_CTE
UNION SELECT * FROM MaxNaturalCanastas_CTE
UNION SELECT * FROM MaxUnnaturalCanastas_CTE
UNION SELECT * FROM MaxTotalCanastas_CTE
UNION SELECT * FROM MaxPenalties_CTE*/


/*CREATE VIEW vwRoundStats AS

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
UNION SELECT * FROM MaxPenalties_CTE*/