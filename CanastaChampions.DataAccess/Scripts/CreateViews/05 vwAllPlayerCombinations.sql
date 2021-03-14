CREATE VIEW vwAllPlayerCombinations AS

    SELECT DISTINCT RegisteredPlayers.CompetitionID,
                    PlayerID,
                    PlayerID2
      FROM RegisteredPlayers
           CROSS JOIN
           (
               SELECT CompetitionID,
                      PlayerID AS PlayerID2
                 FROM RegisteredPlayers
                WHERE LogicallyDeleted = 0
           )
           Sub
     WHERE RegisteredPlayers.LogicallyDeleted = 0 AND 
           RegisteredPlayers.CompetitionID = Sub.CompetitionID AND 
           RegisteredPlayers.PlayerID <> Sub.PlayerID2;
