using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Collections.Generic;
using InteractivePeriodicTable.Utils;
using InteractivePeriodicTable.Data;
using InteractivePeriodicTable.ExtensionMethods;
using System.Windows.Controls.Primitives;

namespace InteractivePeriodicTable
{
    public partial class Quiz : Window
    {
        #region ČLANSKE VARIJABLE
        private QuizQuestions questions = new QuizQuestions();
        private DateTime start;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DispatcherTimer colorChanger = new DispatcherTimer();
        private Random rand = new Random();
        private int score = 0;
        private bool hasImages = false;
        private string quizPath = Pathing.SysDir + "/quiz.json";
        #endregion

        public Quiz()
        {
            this.Closing += stopTimer;

            loadQuizQuestionsFromJSON();

            hasImages = checkImages();

            InitializeComponent();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);

            start = DateTime.Now;
            dispatcherTimer.Start();

            renderNextQuestion();
        }

        /// <summary>
        ///     Čita pitanja za kviz iz datoteke quiz.json te ih deserijalizira u klasu QuizQuestions.
        ///     Obrađuje moguće iznimke.
        /// </summary>
        private void loadQuizQuestionsFromJSON()
        {
            try
            {
                string jsonQuizQuestions = string.Empty;
                using (StreamReader sr = new StreamReader( quizPath ))
                {
                    jsonQuizQuestions = sr.ReadToEnd();
                }

                questions = JsonConvert.DeserializeObject<QuizQuestions>(jsonQuizQuestions);

                int questionsCount = questions.QuizPictures.Count + questions.QuizWith4Ans.Count + questions.QuizYesNo.Count;
                if (questionsCount == 0)
                {
                    MessageBox.Show("Ne posotji niti jedno pitanje.");
                    this.Close();
                }
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
            catch(Exception ex)
            {
                ex.ErrorMessageBox("Dogodila se pogreška !");
            }

            return;
        }

        #region ODABIR PITANJA
        /// <summary>
        ///     Nasumično prikazuje slijedeće pitanje na ekran.
        /// </summary>
        private void renderNextQuestion()
        {
            clearPreviousQuestion();
            refreshScore();

            byte questionType = pickQuestionType();

            if (questionType == 3)
            {
                ErrorHandle.ErrorMessageBox(null, "Dogodila se pogreška prilikom odabira vrste sljedećeg pitanja !");
            }

            int questionID = pickQuestionID(questionType);

            if (questionType == 0)
            {
                renderQuizWith4Ans(questionID);
            }
            else if (questionType == 1)
            {
                renderQuizYesNo(questionID);
            }
            else
            {
                renderQuizPictures(questionID);
            }

            return;
        }

        /// <summary>
        ///     Odabire ID pitanja za prikaz.
        /// </summary>
        /// <param name="questionType">
        ///     Vrsta pitanja čiji ID metoda vraća.
        /// </param>
        /// <returns></returns>
        private int pickQuestionID(byte questionType)
        {
            int numberOfQuestions = -1;
            int questionID = -1;

            if (questionType == 0)
            {
                numberOfQuestions = questions.QuizWith4Ans.Count;
                questionID = rand.Next(0, numberOfQuestions);
            }
            else if (questionType == 1)
            {
                numberOfQuestions = questions.QuizYesNo.Count;
                questionID = rand.Next(0, numberOfQuestions);
            }
            else
            {
                numberOfQuestions = questions.QuizPictures.Count;
                questionID = rand.Next(0, numberOfQuestions);
            }

            return questionID;
        }

        /// <summary>
        ///     Metoda nasumično odabire vrstu pitanja za prikaz.
        /// </summary>
        /// <returns>
        ///     Vrsta pitanja.
        /// </returns>
        private byte pickQuestionType()
        {
            byte questionType = 3; // 0->QuizWith4Ans, 1->QuizYesNo, 2->QuizPictures, 3->Pogreška

            if (hasImages == true)
            {
                questionType = (byte)rand.Next(0, 3);
            }
            else
            {
                questionType = (byte)rand.Next(0, 2);
            }

            return questionType;
        }
        #endregion

        #region PRIKAZ PITANJA
        /// <summary>
        ///     Metoda dinamički prikazuje pitanje sa 4 moguća odgovora na ekranu.
        /// </summary>
        /// <param name="questionID">
        ///     ID pitanja za prikaz.
        /// </param>
        private void renderQuizWith4Ans(int questionID)
        {
            QuizWith4Ans pickedQuestion = questions.QuizWith4Ans[questionID];

            question.Content = pickedQuestion.Question;

            Button A1 = new Button();
            A1.Content = pickedQuestion.A1;
            A1.styleButton();

            Button A2 = new Button();
            A2.Content = pickedQuestion.A2;
            A2.styleButton();

            Button A3 = new Button();
            A3.Content = pickedQuestion.A3;
            A3.styleButton();

            Button A4 = new Button();
            A4.Content = pickedQuestion.A4;
            A4.styleButton();

            if (pickedQuestion.Answer == "1")
            {
                A1.Click += correctAns;
                A2.Click += wrongAns;
                A3.Click += wrongAns;
                A4.Click += wrongAns;
            }
            else if (pickedQuestion.Answer == "2")
            {
                A1.Click += wrongAns;
                A2.Click += correctAns;
                A3.Click += wrongAns;
                A4.Click += wrongAns;
            }
            else if (pickedQuestion.Answer == "3")
            {
                A1.Click += wrongAns;
                A2.Click += wrongAns;
                A3.Click += correctAns;
                A4.Click += wrongAns;
            }
            else if (pickedQuestion.Answer == "4")
            {
                A1.Click += wrongAns;
                A2.Click += wrongAns;
                A3.Click += wrongAns;
                A4.Click += correctAns;
            }

            placeButtonsRandomly(new List<Button>() { A1, A2, A3, A4 });

            return;
        }

        /// <summary>
        ///     Metoda dinamički prikazuje pitanje sa Da/Ne odgovorima na ekranu.
        /// </summary>
        /// <param name="questionID">
        ///     ID pitanja za prikaz.
        /// </param>
        private void renderQuizYesNo(int questionID)
        {
            QuizYesNo pickedQuestion = questions.QuizYesNo[questionID];

            question.Content = pickedQuestion.Question;

            Button A1 = new Button();
            A1.Content = pickedQuestion.A1;
            A1.styleButton();

            Button A2 = new Button();
            A2.Content = pickedQuestion.A2;
            A2.styleButton();

            if (pickedQuestion.Answer == "1")
            {
                A1.Click += correctAns;
                A2.Click += wrongAns;
            }
            else if (pickedQuestion.Answer == "2")
            {
                A1.Click += wrongAns;
                A2.Click += correctAns;
            }

            placeButtonsRandomly(new List<Button>() { A1, A2 });

            return;
        }

        /// <summary>
        ///     Metoda dinamički prikazuje pitanje sa slikom na ekranu.
        ///     Registrira KeyDown događaj kako bi korisnik mogao sa Enter gumbom potvrditi napisano rješenje.
        /// </summary>
        /// <param name="questionID">
        ///     ID pitanja za prikaz.
        /// </param>
        private void renderQuizPictures(int questionID)
        {
            question.Content = "Write what you see in this image";

            QuizPictures pickedQuestion = questions.QuizPictures[questionID];

            BitmapImage imageData = new BitmapImage();
            imageData.BeginInit();
            imageData.UriSource = new Uri(Pathing.ImgDir + "\\" + pickedQuestion.ImagePath, UriKind.Absolute);
            imageData.EndInit();

            Image image = new Image();
            image.Width = 300;
            image.Source = imageData;

            this.sp.Children.Add(image);

            TextBox answerTextBox = new TextBox();
            answerTextBox.Name = "QuizPictures_txbx";
            answerTextBox.styleTextBox();

            this.sp.RegisterName(answerTextBox.Name, answerTextBox);
            this.sp.Children.Add(answerTextBox);

            answerTextBox.Focus();

            Button checkButton = new Button();
            checkButton.Content = "OK";
            checkButton.Name = "QuizPictures_btn";
            checkButton.styleButton();
            checkButton.Tag = pickedQuestion.Answer;
            checkButton.Click += checkPicAns;

            this.sp.RegisterName(checkButton.Name, checkButton);
            this.sp.Children.Add(checkButton);

            this.KeyDown += Quiz_KeyDown;

            return;
        }
        #endregion

        #region POMOĆNE METODE
        /// <summary>
        ///     Metoda provjerava da li na disku postoje slike potrebene za kviz.
        ///     Ako neke slike nedostaju, javlja poruku i otvara kviz bez pitanja sa slikama.
        /// </summary>
        /// <returns>
        ///     Postavlja vrijedonst varijable ovisno o tome da li postoje slike na disku.
        /// </returns>
        private bool checkImages()
        {
            List<string> missingPictures = new List<string>();

            foreach (QuizPictures pictureQuestion in questions.QuizPictures)
            {
                if (File.Exists(Pathing.ImgDir + "\\" + pictureQuestion.ImagePath) == false)
                {
                    missingPictures.Add(pictureQuestion.ImagePath);
                }
            }

            if (missingPictures.Count > 0)
            {
                StringBuilder missingPicturesText = new StringBuilder();
                foreach (string imageName in missingPictures)
                {
                    missingPicturesText.AppendLine(imageName);
                }

                MessageBox.Show("You are missing these images:\n" + missingPicturesText.ToString() +
                                "\n You won't get questions with images in quiz game.", "Missing images !");

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Čisti StackPanel od svih kontrola.
        /// </summary>
        private void clearPreviousQuestion()
        {
            if (this.sp.Children.Count > 0)
            {
                this.sp.Children.Clear();
            }

            return;
        }

        /// <summary>
        ///     Osvježava prikaz rezultata na ekranu.
        /// </summary>
        private void refreshScore()
        {
            scr_lbl.Content = "Score: " + score.ToString();

            return;
        }

        /// <summary>
        ///     Nasumično prikazuje gumbe za odgovore.
        /// </summary>
        /// <param name="buttonsToPlace">
        ///     Lista gumbiju koje moramo nasumično posložiti na ekranu.
        /// </param>
        private void placeButtonsRandomly(List<Button> buttonsToPlace)
        {
            List<byte> orderedButtons = new List<byte>();
            int numberOfButtons = buttonsToPlace.Count;

            while (orderedButtons.Count < numberOfButtons)
            {
                byte orderNumber = (byte)rand.Next(0, numberOfButtons);

                if (orderedButtons.Exists(x => x == orderNumber) == false)
                {
                    orderedButtons.Add(orderNumber);
                }
            }

            foreach (byte buttonNumber in orderedButtons)
            {
                this.sp.Children.Add(buttonsToPlace[buttonNumber]);
            }

            return;
        }
        #endregion

        #region DOGAĐAJI
        private void Quiz_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (e.Key == Key.Enter)
            {
                this.KeyDown -= Quiz_KeyDown;

                Button btn = (Button)this.sp.FindName("QuizPictures_btn");

                btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                e.Handled = false;
            }

            return;
        }
        private void checkPicAns(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            this.sp.UnregisterName("QuizPictures_btn");

            if (btn.Tag != null)
            {
                TextBox txbx = (TextBox)this.sp.FindName("QuizPictures_txbx");
                this.sp.UnregisterName("QuizPictures_txbx");

                if (txbx.Text.ToUpper() == btn.Tag.ToString().ToUpper())
                {
                    correctAns(sender, e);
                }
                else
                {
                    wrongAns(sender, e);
                }
            }

            return;
        }
        private void correctAns(object sender, RoutedEventArgs e)
        {
            score += 1;

            start = start.AddSeconds(5);

            timer.Foreground = Brushes.Green;

            colorChanger.Interval = new TimeSpan(0, 0, 0, 1);
            colorChanger.Tick += colorChanger_Tick;
            colorChanger.Start();

            renderNextQuestion();

            return;
        }
        private void wrongAns(object sender, RoutedEventArgs e)
        {
            score -= 1;

            start = start.AddSeconds(-5);

            timer.Foreground = Brushes.Red;

            colorChanger.Interval = new TimeSpan(0, 0, 0, 1);
            colorChanger.Tick += colorChanger_Tick;
            colorChanger.Start();

            renderNextQuestion();

            return;
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - start;
            timer.Content = "Time left: " + Convert.ToString(30 - elapsed.Seconds) + " s";

            if (elapsed.Seconds >= 30)
            {
                dispatcherTimer.Stop();

                SaveScorePrompt window = new SaveScorePrompt(score);

                this.Close();

                window.ShowDialog();
            }

            return;
        }
        private void colorChanger_Tick(object sender, EventArgs e)
        {
            colorChanger.Stop();

            timer.styleLabel();

            return;
        }
        private void stopTimer(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            return;
        }
        #endregion
    }
}
