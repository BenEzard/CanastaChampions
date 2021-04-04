CREATE VIEW vwCurrentAndNextDealer AS

WITH 
GameTeam_CTE AS (
    SELECT 
        GameID,
        Team1Player1ID,
        Team1Player2ID,
        Team2Player1ID,
        Team2Player2ID,
        Team3Player1ID,
        Team3Player2ID
    FROM 
        vwGameTeam
),

Sub_CTE AS (
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
           CASE
               WHEN CurrentDealerTeam1.GameID IS NOT NULL THEN 1
               WHEN CurrentDealerTeam2.GameID IS NOT NULL THEN 2
               WHEN CurrentDealerTeam3.GameID IS NOT NULL THEN 3
           END AS CurrentDealer_TeamNumber,
           GamePlayerPositions2.PlayerID AS NextDealerPlayerID,
           Player2.PlayerName AS NextDealer_Name,
           CASE
               WHEN NextDealerTeam1.GameID IS NOT NULL THEN 1
               WHEN NextDealerTeam2.GameID IS NOT NULL THEN 2
               WHEN NextDealerTeam3.GameID IS NOT NULL THEN 3
           END AS NextDealer_TeamNumber
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
           Players Player2 ON GamePlayerPositions2.PlayerID = Player2.PlayerID
           LEFT JOIN GameTeam_CTE CurrentDealerTeam1 ON (GamePlayerPositions.GameID = CurrentDealerTeam1.GameID AND (Player1.PlayerID = CurrentDealerTeam1.Team1Player1ID OR Player1.PlayerID = CurrentDealerTeam1.Team1Player2ID))
           LEFT JOIN GameTeam_CTE CurrentDealerTeam2 ON (GamePlayerPositions.GameID = CurrentDealerTeam2.GameID AND (Player1.PlayerID = CurrentDealerTeam2.Team2Player1ID OR Player1.PlayerID = CurrentDealerTeam2.Team2Player2ID))
           LEFT JOIN GameTeam_CTE CurrentDealerTeam3 ON (GamePlayerPositions.GameID = CurrentDealerTeam3.GameID AND (Player1.PlayerID = CurrentDealerTeam3.Team3Player1ID OR Player1.PlayerID = CurrentDealerTeam3.Team3Player2ID))
           LEFT JOIN GameTeam_CTE NextDealerTeam1 ON (GamePlayerPositions.GameID = NextDealerTeam1.GameID AND (Player2.PlayerID = NextDealerTeam1.Team1Player1ID OR Player2.PlayerID = NextDealerTeam1.Team1Player2ID))
           LEFT JOIN GameTeam_CTE NextDealerTeam2 ON (GamePlayerPositions.GameID = NextDealerTeam2.GameID AND (Player2.PlayerID = NextDealerTeam2.Team2Player1ID OR Player2.PlayerID = NextDealerTeam2.Team2Player2ID))
           LEFT JOIN GameTeam_CTE NextDealerTeam3 ON (GamePlayerPositions.GameID = NextDealerTeam3.GameID AND (Player2.PlayerID = NextDealerTeam3.Team3Player1ID OR Player2.PlayerID = NextDealerTeam3.Team3Player2ID))