using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace InteractivePeriodicTable.Utils
{
    public class Update
    {
        private string getQuizWith4Ans()
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
                                                                     "'\"A1\":' + '\"' + A1 + '\", ' +" +
                                                                     "'\"A2\":' + '\"' + A2 + '\", ' +" +
                                                                     "'\"A3\":' + '\"' + A3 + '\", ' +" +
                                                                     "'\"A4\":' + '\"' + A4 + '\"' +" +
                                                               "'},' AS data " +
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
        public void updateQuiz()
        {
            string path_to_quiz = Pathing.sysDir + "\\quiz.json";

            StringBuilder quiz_json = new StringBuilder();
            quiz_json.Append("{ \"QuizWith4Ans\":" + getQuizWith4Ans() + "," + "\"QuizYesNo\":" + getQuizYesNo() + "," + "\"QuizPictures\":" + getQuizPictures() + "}");

            File.WriteAllText(path_to_quiz, quiz_json.ToString());
        }
        public void updateFacts()
        {
            string path_to_facts = Pathing.sysDir + "\\facts.json";

            StringBuilder facts_json = new StringBuilder();
            facts_json.Append("{ \"Facts\": " + getFacts() + "}");

            File.WriteAllText(path_to_facts, facts_json.ToString());
        }
    }
}
