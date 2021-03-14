CREATE VIEW vwCurrentAndNextDealer AS

WITH Sub_CTE AS (
        SELECT GamePlayerPositions.GameID,
               subCurrent.CurrentDealerPosition,
               CASE WHEN PositionNumber < MaxNumber THEN PositionNumber + 1 ELSE 1 END AS NextDealerPosition
          FROM GamePlayerPositions
               LEFT JOIN
               (
                   SELECT GameID,
                          PositionNumber AS CurrentDealerPosition
                     FROM GamePlayerPositions
                    WHERE DealerFlag = 1
                    GROUP BY GameID
               )
               AS subCurrent ON GamePlayerPositions.GameID = subCurrent.GameID
               LEFT JOIN
               (
                   SELECT GameID,
                          MAX(PositionNumber) AS MaxNumber
                     FROM GamePlayerPositions
                    GROUP BY GameID
               )
               AS subMax ON GamePlayerPositions.GameID = subMax.GameID
         WHERE DealerFlag = 1
    )
    SELECT Sub_CTE.GameID,
           GamePlayerPositions.PlayerID AS CurrentDealerPlayerID,
           Player1.PlayerName AS CurrentDealer_Name,
           GamePlayerPositions2.PlayerID AS NextDealerPlayerID,
           Player2.PlayerName AS NextDealer_Name
      FROM Sub_CTE
           LEFT JOIN
           GamePlayerPositions ON GamePlayerPositions.GameID = Sub_CTE.GameID AND 
                                  GamePlayerPositions.PositionNumber = Sub_CTE.CurrentDealerPosition
           LEFT JOIN
           GamePlayerPositions GamePlayerPositions2 ON GamePlayerPositions2.GameID = Sub_CTE.GameID AND 
                                                       GamePlayerPositions2.PositionNumber = Sub_CTE.NextDealerPosition
           LEFT JOIN
           Players Player1 ON GamePlayerPositions.PlayerID = Player1.PlayerID
           LEFT JOIN
           Players Player2 ON GamePlayerPositions2.PlayerID = Player2.PlayerID;
