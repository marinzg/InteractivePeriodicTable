using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace InteractivePeriodicTable.Utils
{
    public class Update
    {
        private SqlConnection conn;
        private SqlCommand cmnd;
        private SqlDataReader rdr;
        private string connectionString = ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString;

        private string getDataInJson( string quizSelect )
        {
            StringBuilder quizData = new StringBuilder();
            quizData.Append("[");

            using (conn = new SqlConnection( connectionString ))
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
                    cmnd = new SqlCommand();
                    cmnd.Connection = conn;
                    cmnd.CommandText = quizSelect;

                    using (cmnd)
                    {
                        using (rdr = cmnd.ExecuteReader())
                        {
                            if (rdr.HasRows == false)
                            {
                                return "[]";
                            }
                            while (rdr.Read())
                            {
                                quizData.Append(rdr["data"].ToString());
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("Dogodila se pogreška prilikom dohvaćanja podataka iz baze.");
                }
            }

            quizData.Remove(quizData.Length - 1, 1);
            quizData.Append("]");

            return quizData.ToString();
        }

        private string getQuizWith4Ans()
        {
            string quizSelect = "SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +" +
                                            "'\"Question\":' + '\"' + Question + '\",' +" +
                                            "'\"Answer\":' + '\"' + CAST(Answer AS CHAR(1)) + '\",' +" +
                                            "'\"A1\":' + '\"' + A1 + '\", ' +" +
                                            "'\"A2\":' + '\"' + A2 + '\", ' +" +
                                            "'\"A3\":' + '\"' + A3 + '\", ' +" +
                                            "'\"A4\":' + '\"' + A4 + '\"' +" +
                                    "'},' AS data " +
                                "FROM QuizWith4Ans";

            string quizData = getDataInJson( quizSelect );

            return quizData;
        }
        private string getQuizYesNo()
        {
            string quizSelect = "SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +" +
                                            "'\"Question\":' + '\"' + Question + '\",' +" +
                                            "'\"Answer\":' + '\"' + CAST(Answer AS CHAR(1)) + '\",' +" +
                                            "'\"A1\":' + '\"' + A1 + '\",' +" +
                                            "'\"A2\":' + '\"' + A2 + '\"' +" +
                                        "'},' AS data " +
                                "FROM QuizYesNo";

            string quizData = getDataInJson( quizSelect );

            return quizData;
        }
        private string getQuizPictures()
        {
            string quizSelect = "SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +" +
                                            "'\"ImagePath\":' + '\"' + ImagePath + '\",' +" +
                                            "'\"Answer\":' + '\"' + CAST(Answer AS NVARCHAR(100)) + '\"' +" +
                                        "'},' AS data " +
                                "FROM QuizPictures";

            string quizData = getDataInJson( quizSelect );

            return quizData;
        }
        private string getFacts()
        {
            string factsSelect = "SELECT '{ \"Fact\":' + '\"' + Fact + '\"},' AS data " +
                                "FROM DidYouKnow";

            string factsData = getDataInJson( factsSelect );

            return factsData;
        }

        public void updateQuiz()
        {
            string pathToQuiz = Pathing.SysDir + "\\quiz.json";

            string quizWith4Ans = getQuizWith4Ans();
            string quizYesNo = getQuizYesNo();
            string quizPictures = getQuizPictures();

            string jsonQuiz = "{ \"QuizWith4Ans\":" + quizWith4Ans + "," +
                                "\"QuizYesNo\":" + quizYesNo + "," +
                                "\"QuizPictures\":" + quizPictures + "}";
            try
            {
                File.WriteAllText(pathToQuiz, jsonQuiz);
            }
            catch (FileNotFoundException fnfe)
            {
                fnfe.ErrorMessageBox("Nije pronađena datoteka quiz.json !");
            }
            catch (DirectoryNotFoundException dnfe)
            {
                dnfe.ErrorMessageBox("Nije pronađen direktorij " + Pathing.SysDir);
            }
            catch (IOException ioe)
            {
                ioe.ErrorMessageBox("Greška prilikom čitanja iz datoteke.");
            }
            
            return;
        }
        public void updateFacts()
        {
            string pathToFacts = Pathing.SysDir + "\\facts.json";
            string jsonFacts = "{ \"Facts\": " + getFacts() + "}";
            try
            {
                File.WriteAllText(pathToFacts, jsonFacts);
            }
            catch (FileNotFoundException fnfe)
            {
                fnfe.ErrorMessageBox("Nije pronađena datoteka quiz.json !");
            }
            catch (DirectoryNotFoundException dnfe)
            {
                dnfe.ErrorMessageBox("Nije pronađen direktorij " + Pathing.SysDir);
            }
            catch (IOException ioe)
            {
                ioe.ErrorMessageBox("Greška prilikom čitanja iz datoteke.");
            }

            return;
        }
    }
}
