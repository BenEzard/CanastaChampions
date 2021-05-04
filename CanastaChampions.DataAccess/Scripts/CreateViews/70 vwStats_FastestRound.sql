CREATE VIEW vwStats_FastestRound AS
WITH RoundTimes_CTE AS (
        SELECT GameRoundID,
               CompetitionID,
               GameID,
               GameRoundNumber,
               StartOfRoundDateTime,
               EndOfRoundDateTime,
               WinningTeamID,
               CAST ( (julianday(EndOfRoundDateTime) - julianday(StartOfRoundDateTime) ) * 24 * 60 * 60 AS INTEGER) AS RoundDateTimeInSeconds
          FROM GameRound
    ),
    QuickestRound_CTE AS (
        SELECT RoundTimes_CTE.CompetitionID,
               RoundTimes_CTE.GameID,
               RoundTimes_CTE.GameRoundNumber,
               RoundTimes_CTE.StartOfRoundDateTime,
               vwTeam.TeamName AS WinningTeamName,
               subFastest.RoundDateTimeInSeconds,
               subFastest.label
          FROM RoundTimes_CTE
               INNER JOIN
               (
                   SELECT CompetitionID,
                          MIN(RoundDateTimeInSeconds) AS RoundDateTimeInSeconds,
                          'FASTEST_ROUND' AS label
                     FROM RoundTimes_CTE
                    GROUP BY CompetitionID
               )
               subFastest ON RoundTimes_CTE.CompetitionID = subFastest.CompetitionID AND 
                             RoundTimes_CTE.RoundDateTimeInSeconds = subFastest.RoundDateTimeInSeconds
               LEFT JOIN
               vwTeam ON RoundTimes_CTE.WinningTeamID = vwTeam.TeamID
    )
    SELECT *
      FROM QuickestRound_CTE;
