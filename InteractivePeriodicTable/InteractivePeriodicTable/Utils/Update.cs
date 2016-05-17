using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using InteractivePeriodicTable.ExtensionMethods;
using System;

namespace InteractivePeriodicTable.Utils
{
    public class Update
    {


        #region METODA VRAĆA REZULTAT UPITA KAO JSON NIZ
        /// <summary>
        ///     Metoda vrši upit nad bazom podataka.
        ///     Provjerava jeli došlo do iznimke.
        /// </summary>
        /// <param name="dataSelect">
        ///     Upit koji želim vršit nad abazom podataka.
        /// </param>
        /// <returns>
        ///     Rezultat upita nad bazom podataka formatiran kao JSON niz.
        /// </returns>
        private string getDataInJson(string dataSelect)
        {
            StringBuilder data = new StringBuilder();
            data.Append("[");

            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = ConfigurationManager.ConnectionStrings["PPIJ"].ConnectionString;

            using (dbConnection)
            {
                try
                {
                    dbConnection.Open();
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("There was an unexpected error trying to connect to server!");
                }
                try
                {
                    SqlCommand dbCommand = new SqlCommand();
                    dbCommand.CommandText = dataSelect;
                    dbCommand.Connection = dbConnection;

                    using (dbCommand)
                    {
                        using (SqlDataReader dataReader = dbCommand.ExecuteReader())
                        {
                            if (dataReader.HasRows == false)
                            {
                                return "[]";
                            }
                            while (dataReader.Read())
                            {
                                data.Append(dataReader["data"].ToString());
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    ex.ErrorMessageBox("There was an unexpected error trying to fetch data from server!");
                }
            }

            data.Remove(data.Length - 1, 1);
            data.Append("]");

            return data.ToString();
        }
        #endregion




        #region KVIZ & ZANIMLJIVOSTI
        /// <summary>
        ///     Metoda vrši upit nad bazom podataka.
        /// </summary>
        /// <returns>
        ///     Vraća sva pitanja sa 4 moguća odgovora formatirana kao JSON niz.
        /// </returns>
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

            string quizData = getDataInJson(quizSelect);

            return quizData;
        }

        /// <summary>
        ///     Metoda vrši upit nad bazom podataka.
        /// </summary>
        /// <returns>
        ///     Vraća sva pitanja sa Da/Ne odgovorima formatirana kao JSON niz.
        /// </returns>
        private string getQuizYesNo()
        {
            string quizSelect = "SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +" +
                                            "'\"Question\":' + '\"' + Question + '\",' +" +
                                            "'\"Answer\":' + '\"' + CAST(Answer AS CHAR(1)) + '\",' +" +
                                            "'\"A1\":' + '\"' + A1 + '\",' +" +
                                            "'\"A2\":' + '\"' + A2 + '\"' +" +
                                        "'},' AS data " +
                                "FROM QuizYesNo";

            string quizData = getDataInJson(quizSelect);

            return quizData;
        }

        /// <summary>
        ///     Metoda vrši upit nad bazom podataka.
        /// </summary>
        /// <returns>
        ///     Vraća sva pitanja sa slikama formatirana kao JSON niz.
        /// </returns>
        private string getQuizPictures()
        {
            string quizSelect = "SELECT '{' + '\"ID\":' + CAST(ID AS VARCHAR(4)) + ',' +" +
                                            "'\"ImagePath\":' + '\"' + ImagePath + '\",' +" +
                                            "'\"Answer\":' + '\"' + CAST(Answer AS NVARCHAR(100)) + '\"' +" +
                                        "'},' AS data " +
                                "FROM QuizPictures";

            string quizData = getDataInJson(quizSelect);

            return quizData;
        }

        /// <summary>
        ///     Metoda vrši upit nad bazom podataka.
        /// </summary>
        /// <returns>
        ///     Vraća sve zanimljivosti formatirane kao JSON niz.
        /// </returns>
        private string getFacts()
        {
            string factsSelect = "SELECT '{ \"Fact\":' + '\"' + Fact + '\"},' AS data " +
                                "FROM DidYouKnow";

            string factsData = getDataInJson(factsSelect);

            return factsData;
        }
        #endregion



        #region JAVNE METODE ZA OSVJEŽAVANJE KVIZA I ZANIMLJIVOSTI
        /// <summary>
        ///     Zapisuje na disk sva pitanja za kviz u formatu .json
        /// </summary>
        public void updateQuiz()
        {
            string quizWith4Ans = getQuizWith4Ans();
            string quizYesNo = getQuizYesNo();
            string quizPictures = getQuizPictures();

            string jsonQuiz = "{ \"QuizWith4Ans\":" + quizWith4Ans + "," +
                                "\"QuizYesNo\":" + quizYesNo + "," +
                                "\"QuizPictures\":" + quizPictures + "}";
            try
            {
                Directory.CreateDirectory(Pathing.SysDir);

                string pathToQuiz = Pathing.SysDir + "\\quiz.json";
                File.WriteAllText(pathToQuiz, jsonQuiz);
            }
            catch (FileNotFoundException fnfe)
            {
                fnfe.ErrorMessageBox("File not found: quiz.json !");
            }
            catch (DirectoryNotFoundException dnfe)
            {
                dnfe.ErrorMessageBox("Directory not found: " + Pathing.SysDir + "!");
            }
            catch (IOException ioe)
            {
                ioe.ErrorMessageBox("Error trying to read/write from quiz.json file!");
            }
            catch(Exception ex)
            {
                ex.ErrorMessageBox("There was an unexpected error trying to update quiz!");
            }

            return;
        }

        /// <summary>
        ///     Zapisuje na disk sve zanimljivosti u formatu .json
        /// </summary>
        public void updateFacts()
        {
            string facts = getFacts();

            string jsonFacts = "{ \"Facts\": " + facts + "}";

            try
            {
                Directory.CreateDirectory(Pathing.SysDir);

                string pathToFacts = Pathing.SysDir + "\\facts.json";
                File.WriteAllText(pathToFacts, jsonFacts);
            }
            catch (FileNotFoundException fnfe)
            {
                fnfe.ErrorMessageBox("File not found: facts.json !");
            }
            catch (DirectoryNotFoundException dnfe)
            {
                dnfe.ErrorMessageBox("Directory not found: " + Pathing.SysDir + "!");
            }
            catch (IOException ioe)
            {
                ioe.ErrorMessageBox("Error trying to read/write from facts.json file!");
            }
            catch (Exception ex)
            {
                ex.ErrorMessageBox("There was an unexpected error trying to update facts!");
            }

            return;
        }
        #endregion


    }
}
