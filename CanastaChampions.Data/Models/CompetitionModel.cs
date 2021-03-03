using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.Data.Models
{
    public class CompetitionModel
    {
        public long CompetitionID { get; set; } = -1;

        public string CompetitionName { get; set; }

        /// <summary>
        /// This is the list of players who *can* play within this competition.
        /// Players may be in 1 or more competitions.
        /// </summary>
        public List<PlayerModel> RegisteredPlayers { get; set; } = new List<PlayerModel>();

        /// <summary>
        /// Identify whether this Competition has fixed teams or not.
        /// (A fixed Team is where the same players always play together).
        /// </summary>
        public bool FixedTeams { get; set; }

        /// <summary>
        /// Identify whether this Competition has randomised teams or not.
        /// </summary>
        public bool RandomiseTeams { get; set; }

        public bool LogicallyDeleted { get; set; }
    }
}
