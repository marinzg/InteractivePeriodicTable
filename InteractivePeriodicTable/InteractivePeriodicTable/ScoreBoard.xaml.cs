using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Configuration;
using InteractivePeriodicTable.ExtensionMethods;
using InteractivePeriodicTable.Data;
using System.Data;
using System.Collections.Generic;

namespace InteractivePeriodicTable
{
    public partial class ScoreBoard : Window
    {
        public ScoreBoard()
        {
            InitializeComponent();

            bindGameTypeComboBox();
            refreshScoreBoard();
        }

        /// <summary>
        ///     Dohvaća 10 najboljih rezultata u kvizu sa servera.
        ///     Prikazuje ih.
        /// </summary>
        private void getTop10QuizPlayers()
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
                    ex.ErrorMessageBox("There was an error trying to open connection to database.");
                    return;
                }
                try
                {
                    SqlCommand dbCommand = new SqlCommand();
                    dbCommand.CommandText = "SELECT TOP(10) ROW_NUMBER() OVER (ORDER BY Score DESC) AS Position, UserName AS Username, Score FROM UserScoreQuiz ORDER BY Score DESC;";
                    dbCommand.Connection = dbConnection;

                    using (dbCommand)
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(dbCommand);
                        DataTable userScoreDataTable = new DataTable("UserScoreQuiz");

                        dataAdapter.Fill(userScoreDataTable);

                        scoreBoard.ItemsSource = userScoreDataTable.DefaultView;

                        dataAdapter.Update(userScoreDataTable);
                    }
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("There was an error trying to get data from database.");
                    return;
                }
            }

            return;
        }

        /// <summary>
        ///     Dohvaća 10 najboljih rezultata u Drag&Drop-u sa servera.
        ///     Prikazuje ih.
        /// </summary>
        private void getTop10DnDPlayers()
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
                    ex.ErrorMessageBox("There was an error trying to open connection to database.");
                    return;
                }
                try
                {
                    SqlCommand dbCommand = new SqlCommand();
                    dbCommand.CommandText = "SELECT TOP(10) ROW_NUMBER() OVER (ORDER BY Score DESC) AS Position, UserName AS Username, Score FROM UserScoreDnD ORDER BY Score DESC;";
                    dbCommand.Connection = dbConnection;

                    using (dbCommand)
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(dbCommand);
                        DataTable userScoreDataTable = new DataTable("UserScoreDnD");

                        dataAdapter.Fill(userScoreDataTable);

                        scoreBoard.ItemsSource = userScoreDataTable.DefaultView;

                        dataAdapter.Update(userScoreDataTable);
                    }
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("There was an error trying to get data from database.");
                    return;
                }
            }

            return;
        }

        /// <summary>
        ///     Uklanja izvor podataka za prikaz igrača.
        ///     Na temelju ComboBox-a ponovo puni podatke za odabranu igru.
        /// </summary>
        private void refreshScoreBoard()
        {
            scoreBoard.ItemsSource = null;

            ComboBoxPairs selectedGameType = (ComboBoxPairs)gameTypeComboBox.SelectedItem;

            Game _key = selectedGameType._Key;

            if (_key == Game.Quiz)
            {
                getTop10QuizPlayers();
            }
            else if(_key == Game.DragDrop)
            {
                getTop10DnDPlayers();
            }
            else
            {
                "There is no such game.".Alert();
                this.Close();
            }

            return;
        }

        /// <summary>
        ///     Popunjava gameTypeComboBox sa svim igrama za koje možemo vidjeti igrače.
        /// </summary>
        private void bindGameTypeComboBox()
        {
            List<ComboBoxPairs> listOfGames = new List<ComboBoxPairs>();
            listOfGames.Add(new ComboBoxPairs(Game.Quiz, "Quiz"));
            listOfGames.Add(new ComboBoxPairs(Game.DragDrop, "Drag & Drop"));

            gameTypeComboBox.DisplayMemberPath = "_Value";
            gameTypeComboBox.SelectedValuePath = "_Value";
            gameTypeComboBox.ItemsSource = listOfGames;

            gameTypeComboBox.SelectedIndex = 0;

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

        /// <summary>
        ///     Poziva se kada se promjeni odabir igre.
        ///     Prikazuje igrače ovisno o odabranoj igri.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refreshScoreBoard();

            return;
        }
    }
}
