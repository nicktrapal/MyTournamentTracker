using System;
using System.Collections.Generic;
using System.Text;

namespace MyTrackerLibrary.Models
{
    public class TournamentModel
    {
        /// <summary>
        /// The unique identifier for the tournament
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represents the name of the tournament.
        /// </summary>
        public String TournamentName { get; set; }
        /// <summary>
        /// Represents the amount of money you have to pay to enter the tournament.
        /// </summary>
        public decimal EntryFee { get; set; }
        /// <summary>
        /// Represents the teams that have entered the tournament.
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();
        /// <summary>
        /// Represents the prizes the tournament offers to winners.
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        /// <summary>
        /// Represents the rounds that are gonna be played in the tournament.
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
    }
}
