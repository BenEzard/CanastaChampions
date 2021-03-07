using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.DataAccess.Models
{
    public class PlayerModel
    {
        public long PlayerID { get; set; } = -1;
        public string PlayerName { get; set; }

        /// <summary>
        /// Will be populated as needed.
        /// </summary>
        public long CompetitionID { get; set; }

        public bool LogicallyDeleted { get; set; }
    }
}
