CREATE TABLE GameRoundScoreDetails (
    GameRoundScoreID      INTEGER PRIMARY KEY AUTOINCREMENT,
    CompetitionID         INTEGER REFERENCES Competitions (CompetitionID) 
                                  NOT NULL,
    GameID                INTEGER REFERENCES Game (GameID) 
                                  NOT NULL,
    GameRoundID           INTEGER REFERENCES GameRound (GameRoundID) 
                                  NOT NULL,
    TeamID                INTEGER REFERENCES Team (TeamID) 
                                  NOT NULL,
    PlayerID              INTEGER REFERENCES Players (PlayerID),
    CuttingBonusCount     INTEGER DEFAULT (0),
    PenaltyCount          INTEGER DEFAULT (0),
    NaturalCanastaCount   INTEGER DEFAULT (0),
    UnnaturalCanastaCount INTEGER DEFAULT (0),
    RedThreeCount         INTEGER DEFAULT (0),
    PointsOnHand          INTEGER DEFAULT (0),
    FinishingBonus        INTEGER DEFAULT (0) 
);
