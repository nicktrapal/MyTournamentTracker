using System;
using System.Collections.Generic;
using System.Text;
using MyTrackerLibrary.Models;
using MyTrackerLibrary.DataAccess.TextHelpers;
using System.Linq;

namespace MyTrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PersonModels.csv";
        private const string TeamsFile = "TeamModels.csv";
        private const string TournamentsFile = "TournamentModels.csv";
        private const string MatchupsFile = "MatchupModels.csv";
        private const string MatchupEntriesFile = "MatchupEntryModels.csv";


        public PersonModel CreatePerson(PersonModel model)
        {
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
            int currentId = 1;
            if (people.Count > 0)
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            model.Id = currentId;



            people.Add(model);
            people.SaveToPersonFile(PeopleFile);
            return model;
        }

        //TODO wire up the CreatePrize for text files.
        public PrizeModel CreatePrize(PrizeModel model)
        {
            //Load the text file and convert the text to List <PrizeModel>
            List <PrizeModel> prizes= PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            //Find the max ID
            int currentId = 1;
            if (prizes.Count>0)
                 currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            model.Id = currentId;


            //Add the new record with the new ID (max + 1)
            prizes.Add(model);

            //Convert the prizes to list <string>
            //Save the list<string> to the text file 
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
        }

        public TeamModel CreateTeam(TeamModel model)
        {
           
            List<TeamModel> teams =TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
           
            int currentId = 1;
            if (teams.Count > 0)
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamFile(TeamsFile);

            return model;
        }

        //TODO Refactor the rest of the functions return types
        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = TournamentsFile.FullFilePath().LoadFile().ConvertToTournamentModels(TeamsFile,PeopleFile,PrizesFile);

            int currentId = 1;
            if (tournaments.Count > 0)
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            model.Id = currentId;

            model.SaveRoundsToFile(MatchupsFile, MatchupEntriesFile);

            tournaments.Add(model);

            tournaments.SaveToTournamentFile(TournamentsFile);

        }

        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            return TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
            
        }

        public List<TournamentModel> GetTournament_All()
        {
            return  TournamentsFile.FullFilePath().LoadFile().ConvertToTournamentModels(TeamsFile, PeopleFile, PrizesFile);

        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }
    }
}
