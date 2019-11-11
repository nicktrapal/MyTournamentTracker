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
        /// The Id from the database that will be used to identify the winner of this matchup.
        /// </summary>
        public int Winner_Id { get; set; }
        /// <summary>
        /// Represents the winner of this matchup.
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// Represents the round that this matchup is in.
        /// </summary>
        public int MatchupRound { get; set; }

        public string DisplayName
        {
            get
            {
                string output = "";

                foreach (MatchupEntryModel e in Entries)
                {
                    if (e.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                            output = e.TeamCompeting.Team_Name;
                        else
                            output += $" vs. {e.TeamCompeting.Team_Name}";
                    }
                    else
                    {
                        output = "Matchup not yet determined.";
                        break;
                    }

                }

                return output;
            }
        }
    }
}
