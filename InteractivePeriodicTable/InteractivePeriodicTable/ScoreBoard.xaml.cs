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
using System.Data.SqlClient;
using System.Configuration;

namespace InteractivePeriodicTable
{
    public partial class ScoreBoard : Window
    {
        public ScoreBoard()
        {
            InitializeComponent();
            getTop10();
        }
        private void getTop10()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmnd = new SqlCommand("SELECT TOP(10) UserName, Score FROM UserScore ORDER BY Score DESC;", conn))
                {
                    using (SqlDataReader rdr = cmnd.ExecuteReader())
                    {
                        byte i = 1;
                        while (rdr.Read())
                        {
                            Label row = new Label();
                            row.Content = i.ToString() +"." + rdr["UserName"].ToString() + ": " + rdr["Score"].ToString();

                            this.scoreboard.Children.Add(row);
                            i++;
                        }
                    }
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
