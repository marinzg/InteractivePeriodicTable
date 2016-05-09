using System.Windows;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Input;
using InteractivePeriodicTable.Utils;

namespace InteractivePeriodicTable
{
    public partial class SaveScorePrompt : Window
    {
        private int score;
        public SaveScorePrompt(int scr)
        {
            this.score = scr;
            InitializeComponent();
            renderScore();
        }

        private void renderScore()
        {
            score_lbl.Content = "Score: " + score.ToString();
            return;
        }
        private void saveScore()
        {
            if (string.IsNullOrWhiteSpace(username.Text))
            {
                MessageBox.Show("Please enter user name !", "Error");
                return;
            }
            if (username.Text.Length > 20)
            {
                MessageBox.Show("Please enter shorter user name !", "Error");
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom otvaranje veze na bazu.");
                }
                try
                {
                    using (SqlCommand cmnd = new SqlCommand("INSERT INTO UserScore (UserName, Score) VALUES (@user, @score);", conn))
                    {
                        cmnd.Parameters.AddWithValue("@user", username.Text);
                        cmnd.Parameters.AddWithValue("@score", score);

                        cmnd.ExecuteNonQuery();
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