﻿using System.Windows;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Input;
using InteractivePeriodicTable.ExtensionMethods;
using InteractivePeriodicTable.Utils;

namespace InteractivePeriodicTable
{
    public partial class SaveScorePrompt : Window
    {
        #region ČLANSKE VARIJABLE
        private int score;
        #endregion

        public SaveScorePrompt(int scr)
        {
            this.score = scr;

            InitializeComponent();

            score_lbl.Content = "Score: " + score.ToString();
        }

        private void saveScore()
        {
            if(InternetConnection.IsConnected() == false)
            {
                MessageBox.Show("Nemogu se spojiti na server !", "Pogreška");
                return;
            }

            if (string.IsNullOrWhiteSpace(username.Text))
            {
                MessageBox.Show("Please enter user name !", "Error");
                return;
            }
            if (username.Text.Length > 20)
            {
                MessageBox.Show("Username can be max. 20 characters long !", "Error");
                return;
            }

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
                    dbCommand.CommandText = "INSERT INTO UserScore (UserName, Score) VALUES (@user, @score);";
                    dbCommand.Connection = dbConnection;

                    using (dbCommand)
                    {
                        dbCommand.Parameters.AddWithValue("@user", username.Text);
                        dbCommand.Parameters.AddWithValue("@score", score);

                        dbCommand.ExecuteNonQuery();
                        MessageBox.Show("Score was successfully submitted !", "Information");
                    }
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom spremanja podataka u bazu.");
                }
            }

            this.Close();
            return;
        }

        #region EVENTI
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