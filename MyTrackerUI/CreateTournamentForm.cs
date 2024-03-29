﻿using MyTrackerLibrary;
using MyTrackerLibrary.Models;
using MyTrackerUI;
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
    public partial class CreateTournamentForm : Form,IPrizeRequester,ITeamRequester
    {

        private List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        private List<TeamModel> selectedTeams = new List<TeamModel>();
        private List<PrizeModel> selectedPrizes = new List<PrizeModel>();


        public CreateTournamentForm()
        {
            InitializeComponent();
            WireUpLists();
        }

        private void WireUpLists()
        {
            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "Team_Name";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "Team_Name";

            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        private bool ValidateForm()
        {
            decimal fee = 0;

            if (tournamentNameValue.Text.Length==0)
            {
                MessageBox.Show("You need to enter a valid Tournament Name",
                    "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (tournamentTeamsListBox.Items.Count <= 1)
            {
                MessageBox.Show("You need to enter at least 2 teams to the tournament",
                    "Not enough teams", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            bool feeAcceptable = decimal.TryParse(entryFeeValue.Text, out fee);

            if (!feeAcceptable)
            {
                MessageBox.Show("You need to enter a valid Entry Fee.",
                    "Invalid Fee", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }


        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;

            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);

                WireUpLists();
            }
        }

        private void createPrizebutton_Click(object sender, EventArgs e)
        {
            // Call the CreatePrizeForm
            CreatePrizeForm form = new CreatePrizeForm(this);
            form.Show();
            
        }

        public void PrizeComplete(PrizeModel model)
        {
            //Get back from the form a PrizeModel
            //Take the PrizeModel and put it into our list of selected prizes

            selectedPrizes.Add(model);
            WireUpLists();
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm form = new CreateTeamForm(this);
            form.Show();

        }

        private void removeSelectedPlayersButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentTeamsListBox.SelectedItem;

            if (t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);

                WireUpLists();
            }
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;

            if (p != null)
            {
                selectedPrizes.Remove(p);
                
                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            
            if (!ValidateForm()) //Validate data
                return;
            
            TournamentModel tm = new TournamentModel(); //Create Tournament model
            tm.TournamentName = tournamentNameValue.Text;
            tm.EntryFee = decimal.Parse(entryFeeValue.Text);

            
            
            tm.Prizes = selectedPrizes; //Create all of the prizes entries
            tm.EnteredTeams = selectedTeams; //Create all of team entries

            TournamentLogic.CreateRounds(tm); //Wire the matchups

            GlobalConfig.Connection.CreateTournament(tm);
          
            TournamentViewerForm form = new TournamentViewerForm(tm);
            form.Show();
            this.Close();


        }
    }
}
