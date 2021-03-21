CREATE TABLE GameTeams (
    GameTeamID    INTEGER PRIMARY KEY AUTOINCREMENT,
    CompetitionID INTEGER REFERENCES Competitions (CompetitionID) 
                          NOT NULL,
    GameID        INTEGER REFERENCES Game (GameID)
                          NOT NULL,
    Team1ID       INTEGER REFERENCES Team (TeamID) 
                          NOT NULL,
    Team2ID       INTEGER REFERENCES Team (TeamID) 
                          NOT NULL,
    Team3ID               REFERENCES Team (TeamID) 
);
