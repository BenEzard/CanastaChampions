CREATE TABLE Team (
    TeamID           INTEGER PRIMARY KEY AUTOINCREMENT,
    CompetitionID    INTEGER REFERENCES Competitions (CompetitionID) 
                             NOT NULL,
    TeamMember1ID    INTEGER REFERENCES Players (PlayerID) 
                             NOT NULL,
    TeamMember2ID    INTEGER REFERENCES Players (PlayerID) 
                             NOT NULL,
    LogicallyDeleted BOOLEAN DEFAULT (0) 
);
