using MyTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrackerLibrary
{
    public static class TournamentLogic
    {
        /// <summary>
        /// Randomizes the participating teams of a tournament , handles the byes and creates the rounds.
        /// </summary>
        /// <param name="model"></param>
        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));

            CreateOtherRounds(model, rounds);
        }

        /// <summary>
        /// Creates the rest of the rounds of a tournament (Except for the first one)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="rounds"></param>
        private static void CreateOtherRounds(TournamentModel model,int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel();

            while (round<=rounds)
            {
                foreach(MatchupModel match in previousRound)
                {
                    currentMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });//Links this round's matchup with the previous's round matchup for each team

                    if (currentMatchup.Entries.Count>1)//if the matchup is full we add the matchup to the round and move on to the next one
                    {
                        currentMatchup.MatchupRound = round;
                        currentRound.Add(currentMatchup);
                        currentMatchup = new MatchupModel();
                    }
                }

                model.Rounds.Add(currentRound);//add the round to the tournament
                previousRound = currentRound;
                currentRound = new List<MatchupModel>();

                round += 1;
            }

        }
        /// <summary>
        /// Creates the first round of a tournament (Byes are handled in the first round only)
        /// </summary>
        /// <param name="byes"></param>
        /// <param name="teams"></param>
        /// <returns>The first round as a list of MatchupModels</returns>
        private static List <MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> first_round = new List<MatchupModel>();
            MatchupModel curr = new MatchupModel();
             
            foreach (TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team }); //adds the team as an entry for this matchup

                if (byes > 0 || curr.Entries.Count>1) // if there are any byes or if the matchup already has 2 teams the matchup is full and we procced to fill the next one
                {
                    curr.MatchupRound = 1;
                    first_round.Add(curr);
                    curr = new MatchupModel();

                    if (byes > 0)
                        byes--;
                }
            }

            return first_round;
        }

        /// <summary>
        /// Calculates how many byes are needed for this tournament to have each matchup complete. (byes equal to the amount of teams missing for the tournament bracket to be whole) 
        /// </summary>
        /// <param name="rounds"></param>
        /// <param name="numberOfTeams"></param>
        /// <returns>The number of byes</returns>
        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            int byes = 0;
            int totalTeams = 0;

            for (int i=1; i<= rounds; i++)
                totalTeams *= 2;
            

            byes = totalTeams - numberOfTeams;

            return byes;
        }

        /// <summary>
        /// Calculates how many rounds  are gonna be played in a tournament given the number of participating teams
        /// </summary>
        /// <param name="teamCount"></param>
        /// <returns>the number of  rounds </returns>
        private static int FindNumberOfRounds(int teamCount)
        {
            int rounds = 1;
            int val = 2;

             while (val<teamCount)
             {
                rounds += 1;
                val *= 2;
             }

            return rounds;
        }

        /// <summary>
        /// Randomizes the inserted teams so we can create random brackets
        /// </summary>
        /// <param name="teams"></param>
        /// <returns>A list of the inserted teams but in random order</returns>
        private static List <TeamModel> RandomizeTeamOrder(List <TeamModel> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}
