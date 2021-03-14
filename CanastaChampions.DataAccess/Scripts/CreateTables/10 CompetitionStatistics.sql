CREATE TABLE CompetitionStatistics (
    CompetitionStatisticsID INTEGER      PRIMARY KEY AUTOINCREMENT,
    CompetitionID           INTEGER      REFERENCES Competitions (CompetitionID),
    GameID                  INTEGER      REFERENCES Game (GameID),
    RoundID                 INTEGER      REFERENCES GameRound (GameRoundID),
    StatisticName           VARCHAR (50),
    WinningValue            VARCHAR,
    TeamID                  INTEGER      REFERENCES Team (TeamID),
    PlayerID                INTEGER      REFERENCES Players (PlayerID),
    DateTimeSet             DATETIME
);
