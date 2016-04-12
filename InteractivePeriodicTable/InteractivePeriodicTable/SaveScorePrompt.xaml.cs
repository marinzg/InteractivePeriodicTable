using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;

namespace InteractivePeriodicTable
{
    public partial class SaveScorePrompt : Window
    {
        public int score;
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
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            return;
        }
        private void save_Click(object sender, RoutedEventArgs e)
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
                conn.Open();

                using (SqlCommand cmnd = new SqlCommand("INSERT INTO UserScore (UserName, Score) VALUES (@user, @score);", conn))
                {
                    cmnd.Parameters.AddWithValue("@user", username.Text);
                    cmnd.Parameters.AddWithValue("@score", score);

                    try
                    {
                        cmnd.ExecuteNonQuery();
                        MessageBox.Show("Score was successfully submitted !", "Information");
                    }
                    catch(SqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                        return;
                    }

                    this.Close();
                }
            }

            return;
        }
    }
}