using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Configuration;
using InteractivePeriodicTable.ExtensionMethods;
using InteractivePeriodicTable.Utils;
using System.Data;

namespace InteractivePeriodicTable
{
    public partial class ScoreBoard : Window
    {
        public ScoreBoard()
        {
            InitializeComponent();
            getTop10Players();
        }

        /// <summary>
        ///     Dohvaća 10 najboljih rezultata sa servera.
        ///     Prikazuje ih.
        /// </summary>
        private void getTop10Players()
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString;

            using (dbConnection)
            {
                try
                {
                    dbConnection.Open();
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom otvaranje veze na bazu.");
                }
                try
                {
                    SqlCommand dbCommand = new SqlCommand();
                    dbCommand.CommandText = "SELECT TOP(10) ROW_NUMBER() OVER (ORDER BY Score DESC) AS Position, UserName AS Username, Score FROM UserScore ORDER BY Score DESC;";
                    dbCommand.Connection = dbConnection;

                    using (dbCommand)
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(dbCommand);
                        DataTable userScoreDataTable = new DataTable("UserScore");

                        dataAdapter.Fill(userScoreDataTable);

                        scoreBoard.ItemsSource = userScoreDataTable.DefaultView;

                        dataAdapter.Update(userScoreDataTable);
                    }
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom dohvaćanja podataka iz baze.");
                }
            }

            return;
        }

        /// <summary>
        ///     Gasi prozor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            return;
        }
    }
}
