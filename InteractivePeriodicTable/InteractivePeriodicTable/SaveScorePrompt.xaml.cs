using System.Windows;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Input;
using InteractivePeriodicTable.ExtensionMethods;
using InteractivePeriodicTable.Utils;
using InteractivePeriodicTable.Data;

namespace InteractivePeriodicTable
{
    public partial class SaveScorePrompt : Window
    {
        #region ČLANSKE VARIJABLE
        private Game gameType;
        private int scoreToSave;

        private SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString);
        private SqlCommand dbCommand = new SqlCommand();
        #endregion

        public SaveScorePrompt(int ScoreToSave, Game GameType)
        {
            InitializeComponent();

            this.scoreToSave = ScoreToSave;
            this.gameType = GameType;

            score_lbl.Content = "Score: " + scoreToSave.ToString();
        }

        private void saveScore()
        {
            bool isConnected = InternetConnection.IsConnected();
            if (isConnected == false)
            {
                "You are not connected to internet!".Alert();
                return;
            }

            bool canSave = validateUserName();
            if (canSave == false)
            {
                return;
            }

            if (gameType == Game.Quiz)
            {
                saveQuizScore();
            }
            else if (gameType == Game.DragDrop)
            {
                saveDnDScore();
            }
            else
            {
                "Such game does not exist!".Alert();
                this.Close();
            }

            return;
        }
        private void saveQuizScore()
        {
            using (dbConnection)
            {
                try
                {
                    dbConnection.Open();
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("There was an error trying to open connection to database.");
                    return;
                }
                try
                {
                    dbCommand.CommandText = "INSERT INTO UserScoreQuiz (UserName, Score) VALUES (@user, @score);";
                    dbCommand.Connection = dbConnection;

                    using (dbCommand)
                    {
                        dbCommand.Parameters.AddWithValue("@user", username.Text);
                        dbCommand.Parameters.AddWithValue("@score", scoreToSave);

                        dbCommand.ExecuteNonQuery();
                        "Score was successfully submitted!".Notify();
                    }
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("There was an error trying to save score to database.");
                    return;
                }
            }

            this.Close();
            return;
        }
        private void saveDnDScore()
        {
            using (dbConnection)
            {
                try
                {
                    dbConnection.Open();
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("There was an error trying to open connection to database.");
                    return;
                }
                try
                {
                    dbCommand.CommandText = "INSERT INTO UserScoreDnD (UserName, Score) VALUES (@user, @score);";
                    dbCommand.Connection = dbConnection;

                    using (dbCommand)
                    {
                        dbCommand.Parameters.AddWithValue("@user", username.Text);
                        dbCommand.Parameters.AddWithValue("@score", scoreToSave);

                        dbCommand.ExecuteNonQuery();
                        "Score was successfully submitted!".Notify();
                    }
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("There was an error trying to save score to database.");
                    return;
                }
            }

            this.Close();
            return;
        }
        
        private bool validateUserName()
        {
            if (string.IsNullOrWhiteSpace(username.Text))
            {
                "Please enter user name!".Alert();
                return false;
            }
            if (username.Text.Length > 5)
            {
                "Username can be max. 5 characters long!".Alert();
                return false;
            }

            return true;
        }

        #region DOGAĐAJI
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            return;
        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            saveScore();

            return;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (e.Key == Key.Enter)
            {
                saveScore();
            }
            else
            {
                e.Handled = false;
            }

            return;
        }
        #endregion
    }
}