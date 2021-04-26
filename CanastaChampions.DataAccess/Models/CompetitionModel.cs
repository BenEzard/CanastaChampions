using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.DataAccess.Models
{
    public class CompetitionModel
    {
        public long CompetitionID { get; set; }
        public string CompetitionName { get; set; }
        public string CompetitionImage { get; set; }
        public bool TeamsAreFixed { get; set; }
        public bool TeamsAreRandomised { get; set; }
        public bool LogicallyDeleted { get; set; }

        public CompetitionModel()
        {

        }

        public CompetitionModel(string competitionName, bool teamsAreFixed, bool teamsAreRandomised)
        {
            CompetitionName = competitionName;
            TeamsAreFixed = teamsAreFixed;
            TeamsAreRandomised = teamsAreRandomised;
            LogicallyDeleted = false;
        }   
        
        public CompetitionModel(string competitionName, string competitionImage, bool teamsAreFixed, bool teamsAreRandomised)
        {
            CompetitionName = competitionName;
            CompetitionImage = competitionImage;
            TeamsAreFixed = teamsAreFixed;
            TeamsAreRandomised = teamsAreRandomised;
            LogicallyDeleted = false;
        }
    }
}
