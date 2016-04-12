using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Text;

namespace InteractivePeriodicTable
{
    [Serializable]
    public class QuizWith4Ans
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string A1 { get; set; }
        public string A2 { get; set; }
        public string A3 { get; set; }
        public string A4 { get; set; }
    }
    [Serializable]
    public class QuizYesNo
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string A1 { get; set; }
        public string A2 { get; set; }
    }
    [Serializable]
    public class QuizPictures
    {
        public int ID { get; set; }
        public string ImagePath { get; set; }
        public string Answer { get; set; }
    }
    [Serializable]
    public class QuizQuestions
    {
        public List<QuizWith4Ans> QuizWith4Ans = new List<QuizWith4Ans>();
        public List<QuizYesNo> QuizYesNo = new List<QuizYesNo>();
        public List<QuizPictures> QuizPictures = new List<QuizPictures>();
    }
    [Serializable]
    public class Facts
    {
        public string Fact { get; set; }
    }
    [Serializable]
    public class AllFacts
    {
        public List<Facts> Facts = new List<Facts>();
    }
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            checkQuiz();
            checkFacts();
            MainWindow mainView = new MainWindow();
            mainView.Show();
        }
        private void checkQuiz()
        {
            string path_to_quiz = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Sys/quiz.json";

            if (!File.Exists(path_to_quiz))
            {
                StringBuilder quiz_json = new StringBuilder();
                quiz_json.Append("{ \"QuizWith4Ans\":" + getQuizWith4Ans() + "," + "\"QuizYesNo\":" + getQuizYesNo() + "," + "\"QuizPictures\":" + getQuizPictures() + "}");

                File.WriteAllText(path_to_quiz, quiz_json.ToString());
            }
        }
        private void checkFacts()
        {
            string path_to_facts = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Sys/facts.json";

            if (!File.Exists(path_to_facts))
            {
                StringBuilder facts_json = new StringBuilder();
                facts_json.Append("{ \"Facts\": " + getFacts() + "}");

                File.WriteAllText(path_to_facts, facts_json.ToString());
            }
        }
        private string getQuizWith4Ans()
        {
            StringBuilder data = new StringBuilder();
            data.Append("[");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmnd = new SqlCommand(
                                                        "SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +"+
                                                                     "'\"Question\":' + '\"' + Question + '\",' +" +
                                                                     "'\"Answer\":' + '\"' + CAST(Answer AS CHAR(1)) + '\",' +" +
                                                                     "'\"A1\":' + '\"' + A1 + '\", ' +" +
                                                                     "'\"A2\":' + '\"' + A2 + '\", ' +" +
                                                                     "'\"A3\":' + '\"' + A3 + '\", ' +" +
                                                                     "'\"A4\":' + '\"' + A4 + '\"' +" +
                                                               "'},' AS data "+
                                                        "FROM QuizWith4Ans"
                                                        , conn))
                {
                    using (SqlDataReader rdr = cmnd.ExecuteReader())
                    {
                        if (rdr.HasRows == false)
                        {
                            return "[]";
                        }
                        while (rdr.Read())
                        {
                            data.Append(rdr["data"].ToString());
                        }
                    }
                }
            }

            data.Remove(data.Length - 1, 1);
            data.Append("]");

            return data.ToString();
        }
        private string getQuizYesNo()
        {
            StringBuilder data = new StringBuilder();
            data.Append("[");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmnd = new SqlCommand(
                                                        "SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +" +
                                                                    "'\"Question\":' + '\"' + Question + '\",' +" +
                                                                    "'\"Answer\":' + '\"' + CAST(Answer AS CHAR(1)) + '\",' +" +
                                                                    "'\"A1\":' + '\"' + A1 + '\",' +" +
                                                                    "'\"A2\":' + '\"' + A2 + '\"' +" +
                                                                "'},' AS data " +
                                                        "FROM QuizYesNo"
                                                        , conn))
                {
                    using (SqlDataReader rdr = cmnd.ExecuteReader())
                    {
                        if (rdr.HasRows == false)
                        {
                            return "[]";
                        }
                        while (rdr.Read())
                        {
                            data.Append(rdr["data"].ToString());
                        }
                    }
                }
            }

            data.Remove(data.Length - 1, 1);
            data.Append("]");

            return data.ToString();
        }
        private string getQuizPictures()
        {
            StringBuilder data = new StringBuilder();
            data.Append("[");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmnd = new SqlCommand(
                                                        "SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +" +
                                                                    "'\"ImagePath\":' + '\"' + ImagePath + '\",' +" +
                                                                    "'\"Answer\":' + '\"' + CAST(Answer AS CHAR(1)) + '\"' +" +
                                                                "'},' AS data " +
                                                        "FROM QuizPictures"
                                                        , conn))
                {
                    using (SqlDataReader rdr = cmnd.ExecuteReader())
                    {
                        if (rdr.HasRows == false)
                        {
                            return "[]";
                        }
                        while (rdr.Read())
                        {
                            data.Append(rdr["data"].ToString());
                        }
                    }
                }
            }

            data.Remove(data.Length - 1, 1);
            data.Append("]");

            return data.ToString();
        }
        private string getFacts()
        {
            StringBuilder data = new StringBuilder();
            data.Append("[");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmnd = new SqlCommand(
                                                        "SELECT '{ \"Fact\":' + '\"' + Fact + '\"},' AS data " +
                                                        "FROM DidYouKnow"
                                                        , conn))
                {
                    using (SqlDataReader rdr = cmnd.ExecuteReader())
                    {
                        if (rdr.HasRows == false)
                        {
                            return "[]";
                        }
                        while (rdr.Read())
                        {
                            data.Append(rdr["data"].ToString());
                        }
                    }
                }
            }

            data.Remove(data.Length - 1, 1);
            data.Append("]");

            return data.ToString();
        }
    }
}
