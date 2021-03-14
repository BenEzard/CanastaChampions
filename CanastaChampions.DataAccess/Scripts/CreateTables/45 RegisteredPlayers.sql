CREATE TABLE RegisteredPlayers (
    RegisteredPlayersID INTEGER PRIMARY KEY AUTOINCREMENT,
    CompetitionID       INTEGER REFERENCES Competitions (CompetitionID),
    PlayerID            INTEGER REFERENCES Players (PlayerID),
    Regular             BOOLEAN,
    LogicallyDeleted    BOOLEAN DEFAULT (0) 
);
