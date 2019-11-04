using System;
using System.Collections.Generic;
using System.Text;

namespace MyTrackerLibrary.Models
{
    public class TeamModel
    {
        public int Id { get; set; }

        /// <summary>
        /// Represents the name of this team.
        /// </summary>
        public String Team_Name { get; set; }

        /// <summary>
        /// Represents the members that are part of this team.
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
       
    }
}
