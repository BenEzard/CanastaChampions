CREATE TABLE StatisticType (
    StatisticTypeID    INTEGER       PRIMARY KEY AUTOINCREMENT,
    StatisticName      VARCHAR (50)  NOT NULL
                                     UNIQUE,
    TextualDescription VARCHAR (200),
    CalculateWhen          VARCHAR (50)  NOT NULL,
    IsPositive         BOOLEAN       NOT NULL
                                     DEFAULT (1) 
);
