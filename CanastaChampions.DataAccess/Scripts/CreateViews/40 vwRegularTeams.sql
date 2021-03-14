CREATE VIEW vwRegularTeams AS

WITH Competitions_CTE AS (
        SELECT Competitions.CompetitionID,
               Competitions.CompetitionName
          FROM Competitions
         WHERE FixedTeams = 1 AND 
               LogicallyDeleted = 0
    )

    SELECT DISTINCT
        GameTeams.CompetitionID,
        Competitions_CTE.CompetitionName,
        GameTeams.Team1ID,
        Team1.TeamMember1ID AS Team1_Member1ID,
        Player1.PlayerName AS Team1_Member1_Name,
        Team1.TeamMember2ID AS Team1_Member2ID,
        Player2.PlayerName AS Team1_Member2_Name,
        GameTeams.Team2ID,
        Team2.TeamMember1ID AS Team2_Member1ID,
        Player3.PlayerName AS Team2_Member1_Name,
        Team2.TeamMember2ID AS Team2_Member2ID,
        Player4.PlayerName AS Team2_Member2_Name,
        GameTeams.Team3ID,
        Team3.TeamMember1ID AS Team3_Member1ID,
        Player5.PlayerName AS Team3_Member1_Name,
        Team3.TeamMember2ID AS Team3_Member2ID,
        Player6.PlayerName AS Team3_Member2_Name
    FROM GameTeams
        INNER JOIN Competitions_CTE ON GameTeams.CompetitionID = Competitions_CTE.CompetitionID
        LEFT JOIN Team Team1 ON GameTeams.Team1ID = Team1.TeamID AND 
                        GameTeams.CompetitionID = Team1.CompetitionID
        LEFT JOIN Team Team2 ON GameTeams.Team2ID = Team2.TeamID AND 
                        GameTeams.CompetitionID = Team2.CompetitionID
        LEFT JOIN Team Team3 ON GameTeams.Team3ID = Team3.TeamID AND 
                        GameTeams.CompetitionID = Team3.CompetitionID
        LEFT JOIN Players Player1 ON Team1.TeamMember1ID = Player1.PlayerID
        LEFT JOIN Players Player2 ON Team1.TeamMember2ID = Player2.PlayerID
        LEFT JOIN Players Player3 ON Team2.TeamMember1ID = Player3.PlayerID
        LEFT JOIN Players Player4 ON Team2.TeamMember2ID = Player4.PlayerID
        LEFT JOIN Players Player5 ON Team3.TeamMember1ID = Player5.PlayerID
        LEFT JOIN Players Player6 ON Team3.TeamMember2ID = Player6.PlayerID;
