CREATE VIEW vwGetGameNumber AS

SELECT
    CompetitionID,
    COUNT(*)+1 AS NextGameNumber
FROM
    Game
GROUP BY
    CompetitionID