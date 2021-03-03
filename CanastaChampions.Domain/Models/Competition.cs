using CanastaChampions.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.Domain.Models
{
    public class Competition
    {
        public long CompetitionID { get; set; } = -1;
        public string CompetitionName { get; set; } = "";
        public bool FixedTeams { get; set; } = false;

        /// <summary>
        /// Identify whether this Competition has randomised teams or not.
        /// </summary>
        public bool RandomiseTeams { get; set; } = true;

        //public List<Player> RegisteredPlayers { get; set; }

        public Competition() { }

        public Competition(CompetitionModel cm)
        {
            CompetitionID = cm.CompetitionID;
            CompetitionName = cm.CompetitionName;
            FixedTeams = cm.FixedTeams;
            RandomiseTeams = cm.RandomiseTeams;
        }

        public CompetitionModel GetCompetitionModel()
        {
            CompetitionModel cm = new CompetitionModel();
            cm.CompetitionID = CompetitionID;
            cm.CompetitionName = CompetitionName;
            cm.FixedTeams = FixedTeams;
            cm.RandomiseTeams = RandomiseTeams;

            return cm;
        }
    }
}
