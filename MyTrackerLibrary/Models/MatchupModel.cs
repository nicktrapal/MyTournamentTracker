using System;
using System.Collections.Generic;
using System.Text;

namespace MyTrackerLibrary.Models
{
    public class MatchupModel
    {
        /// <summary>
        /// The unique identifier for the matchup
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represents the 2 teams that are competing in this matchup.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        /// <summary>
        /// Represents the winner of this matchup.
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// Represents the round that this matchup is in.
        /// </summary>
        public int MatchupRound { get; set; }
    }
}
