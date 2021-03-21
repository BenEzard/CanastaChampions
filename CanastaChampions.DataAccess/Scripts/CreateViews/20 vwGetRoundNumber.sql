CREATE VIEW vwGetRoundNumber AS

SELECT
    CompetitionID,
    GameID,
    COUNT(*) AS ThisRoundNumber,
    COUNT(*)+1 AS NextRoundNumber
FROM
    GameRound
GROUP BY
    CompetitionID,
    GameID