CREATE VIEW vwRegisteredPlayers AS

SELECT
    RegisteredPlayers.CompetitionID,
    RegisteredPlayers.PlayerID,
    Players.PlayerName
FROM
    RegisteredPlayers
LEFT JOIN Players
    ON RegisteredPlayers.PlayerID = Players.PlayerID
WHERE
    RegisteredPlayers.LogicallyDeleted = 0
    AND Players.LogicallyDeleted = 0
ORDER BY RegisteredPlayers.Regular DESC