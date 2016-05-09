using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Configuration;
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
                    using (SqlCommand cmnd = new SqlCommand("SELECT TOP(10) UserName, Score FROM UserScore ORDER BY Score DESC;", conn))
                    {
                        using (SqlDataReader rdr = cmnd.ExecuteReader())
                        {
                            byte i = 1;
                            while (rdr.Read())
                            {
                                Label row = new Label();
                                row.Content = i.ToString() + "." + rdr["UserName"].ToString() + ": " + rdr["Score"].ToString();

                                this.scoreboard.Children.Add(row);
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
