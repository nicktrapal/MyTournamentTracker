using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyTrackerLibrary;
using MyTrackerLibrary.DataAccess;
using MyTrackerLibrary.Models;
using MyTrackerUI;

namespace TargetUI
{
    public partial class CreatePrizeForm : Form
    {
        IPrizeRequester callingForm;
        public CreatePrizeForm(IPrizeRequester caller)
        {
            InitializeComponent();

            callingForm = caller;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void orLabel_Click(object sender, EventArgs e)
        {

        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PrizeModel model = new PrizeModel(placeNumberValue.Text, placeNameValue.Text, prizeAmountValue.Text, prizePercentageValue.Text);

                
                GlobalConfig.Connection.CreatePrize(model);

                callingForm.PrizeComplete(model);

                this.Close();

                //placeNameValue.Text = "";
                //placeNumberValue.Text = "";
                //prizeAmountValue.Text = "0";
                //prizePercentageValue.Text = "0";
                
            }
            else
            {
                MessageBox.Show("This form has invalid information , please check it and try again");
            }

        }
        private bool ValidateForm()
        {
            bool output = true;
            int placeNumber = 0;
            bool placeNumberValidNumber = int.TryParse(placeNumberValue.Text, out placeNumber);

            if (placeNumberValidNumber == false)
                output = false;
            if (placeNumber < 1)
                output = false;
            if (placeNameValue.Text.Length == 0)
                output = false;

            decimal prizeAmount = 0;
            int prizePercentage = 0;


            bool prizeAmountValid= decimal.TryParse(prizeAmountValue.Text, out prizeAmount);
            bool prizePercentageValid= int.TryParse(prizePercentageValue.Text, out prizePercentage);

            if (!prizeAmountValid || !prizePercentageValid)
                output = false;

            if (prizeAmount <= 0 && prizePercentage <= 0)
                output = false;

            if (prizePercentage<0 || prizePercentage>100)
                output = false;

            return output;
        }
    }
}
