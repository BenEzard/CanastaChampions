CREATE VIEW vwIncompleteGameInfo AS

WITH
Game_CTE AS (
    SELECT
        CompetitionID,
        MAX(GameID) AS MostRecent_GameID
    FROM
        Game
    WHERE
        GameEndDateTime IS NULL
    GROUP BY
        CompetitionID
),

GameRound_CTE AS (
    SELECT
        GameRound.CompetitionID,
        GameRound.GameID,
        MAX(GameRoundID) AS MostRecent_GameRoundID
    FROM 
        Game_CTE
    INNER JOIN GameRound 
        ON Game_CTE.CompetitionID = GameRound.CompetitionID
        AND Game_CTE.MostRecent_GameID = GameRound.GameID
    WHERE
        EndOfRoundDateTime IS NULL
    GROUP BY
        GameRound.CompetitionID,
        GameRound.GameID
)

SELECT
    Game_CTE.CompetitionID,
    Game_CTE.MostRecent_GameID,
    IFNULL(GameRound_CTE.MostRecent_GameRoundID, 0) AS MostRecent_GameRoundID
FROM
    Game_CTE
LEFT JOIN GameRound_CTE 
    ON Game_CTE.CompetitionID = GameRound_CTE.CompetitionID
    AND Game_CTE.MostRecent_GameID = GameRound_CTE.GameID