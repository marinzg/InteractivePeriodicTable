using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Configuration;
using InteractivePeriodicTable.ExtensionMethods;
using InteractivePeriodicTable.Data;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Windows.Data;

namespace InteractivePeriodicTable
{
    public partial class ScoreBoard : Window
    {
        #region ČLANSKE VARIJABLE
        /// <summary>
        ///     Služi da znamo da li se odvio konstruktor do kraja.
        /// </summary>
        private bool firstLoad = true;
        #endregion

        public ScoreBoard()
        {
            InitializeComponent();

            bindGameTypeComboBox();
            refreshScoreBoard();

            firstLoad = false;
        }

        #region METODE ZA DOHVAT I PRIKAZ PODATAKA
        /// <summary>
        ///     Dohvaća 10 najboljih rezultata u kvizu sa servera.
        ///     Prikazuje ih.
        /// </summary>
        private void getTop10QuizPlayers()
        {
            string selectCommand = "SELECT TOP(10) ROW_NUMBER() OVER (ORDER BY Score DESC) AS Position, UserName AS Username, Score FROM UserScoreQuiz ORDER BY Score DESC;";
            string tableName = "UserScoreQuiz";

            getTop10Players(selectCommand, tableName);

            if (firstLoad == false)
            {
                resizeScoreBoardColumns();
            }

            return;
        }

        /// <summary>
        ///     Dohvaća 10 najboljih rezultata u Drag&Drop-u sa servera.
        ///     Prikazuje ih.
        /// </summary>
        private void getTop10DnDPlayers()
        {
            string selectCommand = "SELECT TOP(10) ROW_NUMBER() OVER (ORDER BY Score DESC) AS Position, UserName AS Username, Score FROM UserScoreDnD ORDER BY Score DESC;";
            string tableName = "UserScoreDnD";

            getTop10Players(selectCommand, tableName);

            resizeScoreBoardColumns();

            return;
        }

        /// <summary>
        ///     Metoda postavlja podatke iz kojeg DataGrid scoreBoard čita i prikazuje.
        /// </summary>
        /// <param name="selectCommand">
        ///     Naredba za dohvat top 10 igrača neke igre.
        /// </param>
        /// <param name="tableName">
        ///     Naziv tabele iz koje dohvaćamo top 10 igrača..
        /// </param>
        private void getTop10Players(string selectCommand, string tableName)
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
                    dbCommand.CommandText = selectCommand;
                    dbCommand.Connection = dbConnection;

                    using (dbCommand)
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(dbCommand);
                        DataTable userScoreDataTable = new DataTable(tableName);

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
        #endregion

        #region POMOĆNE METODE
        /// <summary>
        ///     Metoda postavlja širine kolona tako da popune cijeli Datagrid.
        /// </summary>
        private void resizeScoreBoardColumns()
        {
            scoreBoard.Columns[0].Width = new DataGridLength(0.2, DataGridLengthUnitType.Star);
            scoreBoard.Columns[1].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
            scoreBoard.Columns[2].Width = new DataGridLength(0.2, DataGridLengthUnitType.Star);

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
        #endregion


        #region DOGAĐAJI
        /// <summary>
        ///     Metoda mjenja širinu kolona kada su učitani podaci u scoreBoard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scoreBoard_Loaded(object sender, RoutedEventArgs e)
        {
            resizeScoreBoardColumns();

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
        #endregion
    }
}
