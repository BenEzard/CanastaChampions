CREATE TABLE Players (
    PlayerID         INTEGER       PRIMARY KEY AUTOINCREMENT,
    PlayerName       VARCHAR (100) NOT NULL,
    LogicallyDeleted BOOLEAN       DEFAULT (0) 
);
