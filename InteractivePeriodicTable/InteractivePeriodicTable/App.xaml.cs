using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Newtonsoft.Json;

namespace InteractivePeriodicTable
{
    [Serializable]
    public class User
    {
        public string user_name { get; set; }
        public string password { get; set; }
        public Int32 score { get; set; }
    }
    public partial class App : Application
    {
        private User usr = new User();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (checkLogin() == true)
            {
                LoginPrompt w = new LoginPrompt();
                w.Show();
            }
            else
            {
                MainWindow mainView = new MainWindow();
                mainView.usr = usr;
                mainView.Show();
            }
        }
        private bool checkLogin() // provjerava da li je logiran
        {
            string json = "";
            using (StreamReader sr = new StreamReader(PathingHelper.localDir+@"\sys\login.json"))
            {
                json = sr.ReadToEnd();
            }
            usr = JsonConvert.DeserializeObject<User>(json);
            return (string.IsNullOrWhiteSpace(usr.user_name) || string.IsNullOrWhiteSpace(usr.password));
        }
    }
}
