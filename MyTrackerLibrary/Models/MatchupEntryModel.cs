using System;
using System.Collections.Generic;
using System.Text;

namespace MyTrackerLibrary.Models
{
    public class MatchupEntryModel
    {
        /// <summary>
        /// The unique identifier for the matchup entry
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The ID that will be used to identify the competing team
        /// </summary>
        public int TeamCompeting_Id { get; set; }
        /// <summary>
        /// Represents one team in the matchup.
        /// </summary>
        public TeamModel TeamCompeting { get; set; }
        /// <summary>
        /// Represents the score for the particular team.
        /// </summary>
        public double Score { get; set; }
        /// <summary>
        /// The ID that will be used to identify the parent matchup.
        /// </summary>
        public int ParentMatchup_Id { get; set; }
        /// <summary>
        /// Represents the matchup that this team came from as the winner.
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }
       
    }
}
