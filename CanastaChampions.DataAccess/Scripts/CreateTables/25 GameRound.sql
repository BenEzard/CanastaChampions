CREATE TABLE GameRound (
    GameRoundID          INTEGER  PRIMARY KEY AUTOINCREMENT,
    CompetitionID        INTEGER  REFERENCES Competitions (CompetitionID),
    GameID               INTEGER  REFERENCES Game (GameID),
    GameRoundNumber      INTEGER  NOT NULL,
    StartOfRoundDateTime DATETIME NOT NULL,
    EndOfRoundDateTime   DATETIME,
    DealerID             INTEGER  REFERENCES Players (PlayerID) 
                                  NOT NULL,
    WinningTeamID        INTEGER  REFERENCES Team (TeamID) 
);
