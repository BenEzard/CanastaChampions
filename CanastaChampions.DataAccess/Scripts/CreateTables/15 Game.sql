CREATE TABLE Game (
    GameID                INTEGER      PRIMARY KEY AUTOINCREMENT,
    CompetitionID         INTEGER      REFERENCES Competitions (CompetitionID) 
                                       NOT NULL,
    CompetitionGameNumber INTEGER      NOT NULL,
    Location              VARCHAR (50),
    GameStartDateTime     DATETIME     NOT NULL,
    GameEndDateTime       DATETIME,
    LogicallyDeleted      BOOLEAN      DEFAULT (0) 
);
