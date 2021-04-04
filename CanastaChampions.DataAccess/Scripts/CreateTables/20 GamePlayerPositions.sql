CREATE TABLE GamePlayerPositions (
    GamePlayerPositionID INTEGER PRIMARY KEY AUTOINCREMENT,
    CompetitionID        INTEGER REFERENCES Competitions (CompetitionID) 
                                 NOT NULL,
    GameID               INTEGER REFERENCES Game (GameID) 
                                 NOT NULL,
    PositionNumber       INTEGER NOT NULL,
    PlayerID             INTEGER REFERENCES Players (PlayerID) 
                                 NOT NULL,
    TeamID               INTEGER REFERENCES Team (TeamID) 
                                 NOT NULL,
    DealerFlag           BOOLEAN DEFAULT (0) 
                                 NOT NULL
);
