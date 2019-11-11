using MyTrackerLibrary;
using MyTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TargetUI
{
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;
        List<int> rounds = new List<int>();
        List<MatchupModel> selectedMatchups = new List<MatchupModel>();
        public TournamentViewerForm(TournamentModel tournamentModel)
        {
            InitializeComponent();

            tournament = tournamentModel;

            LoadFormData();
            LoadRounds();
        }

        private void TournamentViewerForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LoadFormData()
        {
            tournamentName.Text = tournament.TournamentName;
        }

        private void WireUpRoundsLists()
        {
            roundDropDown.DataSource = null;
            roundDropDown.DataSource = rounds;

        }

        private void WireUpMatchupsLists()
        {
            matchupListBox.DataSource = null;
            if (selectedMatchups.Count > 0)
            {
                matchupListBox.DataSource = selectedMatchups;
                matchupListBox.DisplayMember = "DisplayName";
                matchupListBox.SelectedIndex = 0;
            }
        }

        private void LoadRounds()
        {
            rounds = new List<int>();
            rounds.Add(1);
            int currRound = 1;

            foreach(List <MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound > currRound)
                {
                    currRound = matchups.First().MatchupRound;
                    rounds.Add(currRound);
                    
                }

            }

            WireUpRoundsLists();
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups();
            WireUpMatchupsLists();
        }

        private void LoadMatchups()
        {
            int round = (int)roundDropDown.SelectedItem;
            selectedMatchups.Clear();

            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound == round)
                {
                    foreach (MatchupModel m in matchups)
                    {
                        if (m.Winner == null || !unplayedOnlyCheckbox.Checked)
                            selectedMatchups.Add(m);
                    }
                    

                }

            }

            DisplayMatchupInfo();
        }

        private void DisplayMatchupInfo()
        {
            bool visible = (selectedMatchups.Count > 0);

            teamOneName.Visible = visible;
            teamOneScoreLabel.Visible = visible;
            teamOneScoreValue.Visible = visible;
            teamTwoName.Visible = visible;
            teamTwoScoreLabel.Visible = visible;
            teamTwoScoreValue.Visible = visible;
            versusLabel.Visible = visible;
            scoreButton.Visible = visible;



        }

        private void LoadMatchup()
        {
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;

            for (int i = 0; i < m.Entries.Count; i++)
            {
                 if (i==0)
                 {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        teamOneName.Text = m.Entries[0].TeamCompeting.Team_Name;
                        teamOneScoreValue.Text = m.Entries[0].Score.ToString();

                        teamTwoName.Text = "<bye>";
                        teamTwoScoreValue.Text = "0";
                    }
                    else
                    {
                        teamOneName.Text = "Not yet set";
                        teamOneScoreValue.Text = "";
                    }
                 }

                if (i == 1)
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        teamTwoName.Text = m.Entries[1].TeamCompeting.Team_Name;
                        teamTwoScoreValue.Text = m.Entries[1].Score.ToString();
                    }
                    else
                    {
                        teamTwoName.Text = "Not yet set";
                        teamTwoScoreValue.Text = "";
                    }
                }

            }
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (matchupListBox.SelectedItem!=null)
             LoadMatchup();
        }

        private void unplayedOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups();
            WireUpMatchupsLists();
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;
            double teamOneScore = 0;
            double teamTwoScore = 0;

            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);
                        if (scoreValid)
                            m.Entries[0].Score = teamOneScore;
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 1.");
                            return;
                        }
                                          
                    }
                   
                }

                if (i == 1)
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);
                        if (scoreValid)
                            m.Entries[0].Score = teamTwoScore;
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 2.");
                            return;
                        }
                    }
                  
                }

            }

            if (teamOneScore > teamTwoScore) //team one wins
            {
                m.Winner = m.Entries[0].TeamCompeting;
                m.Winner_Id = m.Winner.Id;
            }
            else if (teamTwoScore > teamOneScore)
            {
                m.Winner = m.Entries[1].TeamCompeting;
                m.Winner_Id = m.Winner.Id;
            }
            else
                MessageBox.Show("It's a draw (We don't handle draws)");

            //TODO when you score stay to the same matchup
            foreach(List <MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    foreach(MatchupEntryModel entry in matchup.Entries)
                    {
                        if (entry.ParentMatchup != null)
                        {
                            if (entry.ParentMatchup.Id == m.Id)
                            {
                                entry.TeamCompeting = m.Winner;
                                GlobalConfig.Connection.UpdateMatchup(matchup);
                            }
                        }
                    }
                }
            }
            LoadMatchups();
            WireUpMatchupsLists();

            GlobalConfig.Connection.UpdateMatchup(m);
        }
    }
}
