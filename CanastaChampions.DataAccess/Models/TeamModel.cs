namespace CanastaChampions.DataAccess.Models
{
    public class TeamModel
    {
        public long GameTeamID { get; set; }
        public long TeamID { get; set; }
        public long TeamPlayer1ID { get; set; }
        public long TeamPlayer2ID { get; set; }
        public string TeamName { get; set; }
    }
}
