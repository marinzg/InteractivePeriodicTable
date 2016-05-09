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
                    using (SqlCommand cmnd = new SqlCommand("SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +" +
                                                                     "'\"Question\":' + '\"' + Question + '\",' +" +
                                                                     "'\"Answer\":' + '\"' + CAST(Answer AS CHAR(1)) + '\",' +" +
                                                                     "'\"A1\":' + '\"' + A1 + '\", ' +" +
                                                                     "'\"A2\":' + '\"' + A2 + '\", ' +" +
                                                                     "'\"A3\":' + '\"' + A3 + '\", ' +" +
                                                                     "'\"A4\":' + '\"' + A4 + '\"' +" +
                                                               "'},' AS data " +
                                                            "FROM QuizWith4Ans", conn))
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
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom dohvaćanja podataka iz baze.");
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
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom dohvaćanja podataka iz baze.");
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
                    using (SqlCommand cmnd = new SqlCommand("SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +" +
                                                                    "'\"ImagePath\":' + '\"' + ImagePath + '\",' +" +
                                                                    "'\"Answer\":' + '\"' + CAST(Answer AS NVARCHAR(100)) + '\"' +" +
                                                                "'},' AS data " +
                                                        "FROM QuizPictures", conn))
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
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom dohvaćanja podataka iz baze.");
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
                try
                {
                    conn.Open();
                }
                catch(SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom otvaranje veze na bazu.");
                }
                try
                {
                    using (SqlCommand cmnd = new SqlCommand("SELECT '{ \"Fact\":' + '\"' + Fact + '\"},' AS data " + "FROM DidYouKnow", conn))
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
                catch(SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom dohvaćanja podataka iz baze.");
                }
            }

            data.Remove(data.Length - 1, 1);
            data.Append("]");

            return data.ToString();
        }

        public void updateQuiz()
        {
            string pathToQuiz = Pathing.sysDir + "\\quiz.json";
            string jsonQuiz = "{ \"QuizWith4Ans\":" + getQuizWith4Ans() + "," +
                                "\"QuizYesNo\":" + getQuizYesNo() + "," +
                                "\"QuizPictures\":" + getQuizPictures() + "}";

            File.WriteAllText(pathToQuiz, jsonQuiz);

            return;
        }
        public void updateFacts()
        {
            string pathToFacts = Pathing.sysDir + "\\facts.json";
            string jsonFacts = "{ \"Facts\": " + getFacts() + "}";

            File.WriteAllText(pathToFacts, jsonFacts);

            return;
        }
    }
}
