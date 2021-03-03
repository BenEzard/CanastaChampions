using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.Data.Models
{
    public class PlayerModel
    {
        public long PlayerID { get; set; } = -1;
        public string PlayerName { get; set; }

        public bool LogicallyDeleted { get; set; }
    }
}
