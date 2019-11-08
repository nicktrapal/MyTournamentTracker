using MyTrackerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TargetUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Initialize the database connections
            MyTrackerLibrary.GlobalConfig.InitializeConnections(DatabaseType.TextFile);

            Application.Run(new CreateTournamentForm());
            //Application.Run(new TournamentDashboardForm());
        }
    }
}
