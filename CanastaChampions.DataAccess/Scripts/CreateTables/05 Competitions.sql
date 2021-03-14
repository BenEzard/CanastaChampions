CREATE TABLE Competitions (
    CompetitionID    INTEGER       PRIMARY KEY AUTOINCREMENT,
    CompetitionName  VARCHAR (100) NOT NULL,
    FixedTeams       BOOLEAN,
    RandomiseTeams   BOOLEAN,
    LogicallyDeleted BOOLEAN       DEFAULT (0) 
);
