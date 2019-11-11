using System;
using System.Collections.Generic;
using System.Text;
using MyTrackerLibrary.Models;

namespace MyTrackerLibrary.DataAccess
{
    public interface IDataConnection
    {

        PrizeModel CreatePrize(PrizeModel model);
        
        PersonModel CreatePerson(PersonModel model);

        TeamModel CreateTeam(TeamModel model);

        void CreateTournament(TournamentModel model);

        void UpdateMatchup(MatchupModel model);

        List<PersonModel> GetPerson_All();

        List<TeamModel> GetTeam_All();

        List<TournamentModel> GetTournament_All();


    }
}
