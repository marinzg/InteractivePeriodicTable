using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Configuration;
using InteractivePeriodicTable.ExtensionMethods;
using InteractivePeriodicTable.Utils;

namespace InteractivePeriodicTable
{
    public partial class ScoreBoard : Window
    {
        public ScoreBoard()
        {
            InitializeComponent();

            getTop10Players();
        }

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
                    dbCommand.CommandText = "SELECT TOP(10) UserName, Score FROM UserScore ORDER BY Score DESC;";
                    dbCommand.Connection = dbConnection;

                    using (dbCommand)
                    {
                        using (SqlDataReader dataReader = dbCommand.ExecuteReader())
                        {
                            byte i = 1;
                            while (dataReader.Read())
                            {
                                Label userNameScore = new Label();
                                userNameScore.Content = i.ToString() + "." + dataReader["UserName"].ToString() + ": " + dataReader["Score"].ToString();

                                this.scoreboard.Children.Add(userNameScore);
                                i++;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom dohvaćanja podataka iz baze.");
                }
            }

            return;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            return;
        }
    }
}
