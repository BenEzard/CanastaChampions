CREATE VIEW vwTeam AS

SELECT Team.TeamID,
           Team.CompetitionID,
           Team.TeamMember1ID,
           Player1.PlayerName AS Player1Name,
           Team.TeamMember2ID,
           Player2.PlayerName AS Player2Name,
           Player1.PlayerName || ' & ' ||Player2.PlayerName AS TeamName
      FROM Team
           INNER JOIN
           Players Player1 ON Team.TeamMember1ID = Player1.PlayerID AND 
                              Player1.LogicallyDeleted = 0
           INNER JOIN
           Players Player2 ON Team.TeamMember2ID = Player2.PlayerID AND 
                              Player2.LogicallyDeleted = 0
     WHERE Team.LogicallyDeleted = 0