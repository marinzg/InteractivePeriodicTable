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
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace InteractivePeriodicTable
{
    /// <summary>
    /// Interaction logic for loginPrompt.xaml
    /// </summary>
    public partial class LoginPrompt : Window
    {
        public LoginPrompt()
        {
            InitializeComponent();
        }

        private void prijava_btn_Click(object sender, RoutedEventArgs e)
        {
            //lol
            User usr = new User();
            usr.user_name = user.Text;
            usr.password = pass.Text;
            usr.score = -1;

            string json = JsonConvert.SerializeObject(usr);
            File.WriteAllText(System.IO.Path.GetFullPath(@"sys\login.json"), json);

            MainWindow mw = new MainWindow();
            mw.usr = usr;
            mw.Show();

            Close();

            //using (SqlConnection conn = new SqlConnection(""))
            //{
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmnd = new SqlCommand("SELECT Score FROM PPIJ.dbo.user_score WHERE User_name = @name AND Password = @pass;", conn))
            //        {
            //            cmnd.Parameters.AddWithValue("@name", usr.user_name);
            //            cmnd.Parameters.AddWithValue("@pass", usr.password);

            //            object s = cmnd.ExecuteScalar();
            //            if(s != null)
            //            {
            //                usr.score = (int)s;
            //                string json = JsonConvert.SerializeObject(usr);
            //                File.WriteAllText(System.IO.Path.GetFullPath(@"sys\login.json"), json);

            //                MainWindow mw = new MainWindow();
            //                mw.usr = usr;
            //                mw.Show();
            //            }
            //            else
            //            {
            //                MessageBox.Show("Upisani račun ne postoji !", "Obavijest", MessageBoxButton.OK);
            //            }
            //        }
            //    }
            //    catch(Exception ex)
            //    {
            //        MessageBox.Show("Dogodila se pogreška !", "Oops");
            //    }
            //}
        }

        private void izlaz_btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
