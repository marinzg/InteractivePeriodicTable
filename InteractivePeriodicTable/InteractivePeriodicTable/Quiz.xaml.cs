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
        /// <summary>
        ///     Sprema sva pitanja za kviz.
        /// </summary>
        private QuizQuestions questions = new QuizQuestions();

        /// <summary>
        ///     Služi kako bi znali kada je počeo kviz.
        /// </summary>
        private DateTime start;

        /// <summary>
        ///     Brojač koji služi kako bi znali koliko je vremena prošlo od početka igre.
        ///     Također služi kako bi svakih 50ms osvježavali prikaz preostalog vremena na ekranu.
        /// </summary>
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        /// <summary>
        ///     Brojač služi kako bi znali kada je prošla 1s od promijene boje Label-e koja prikazuje preostalo vrijeme za igru.
        /// </summary>
        private DispatcherTimer colorChanger = new DispatcherTimer();

        /// <summary>
        ///     Služi kako bi mogli "nasumično" odabirati vrstu i ID pitanja.
        /// </summary>
        private Random rand = new Random();

        /// <summary>
        ///     Sprema trenutni rezultat igrača.
        /// </summary>
        private int score = 0;

        /// <summary>
        ///     Služi kako bi znali da li postoje slike na disku.
        ///     Ako je false, u kvizu se neće pojavljivati pitanja sa slikama.
        ///     Ako je true, u kvizu će se pojavljivati pitanja sa slikama.
        /// </summary>
        private bool hasImages = false;
        #endregion

        /// <summary>
        ///     Registrira događaj gašenja prozora kako bi zaustavili brojač preostalog vremena za igru.
        ///     Učitava pitanja iz quiz.json datoteke u varijablu questions.
        ///     Provjerava da li ima slika na disku te postavlja vrijednost zastavice hasImages.
        ///     Prikazuje prozor.
        ///     Registrira događaj za odbrojavanje proteklog vremena.
        ///     Postavlja da se svakih 50ms osvježava Label-a koja prikazuje preostalo vrijeme za igru.
        ///     Sprema trenutno vrijeme kako bi znali koliko je vremena proteklo od početka.
        ///     Pokreče timer za odborjavanje vremena za igru.
        ///     Prikazuje prvo pitanje.
        /// </summary>
        /// 
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

        #region DOHVAT PITANJA
        /// <summary>
        ///     Čita pitanja za kviz iz datoteke quiz.json te ih deserijalizira u klasu QuizQuestions.
        ///     Obrađuje moguće iznimke.
        /// </summary>
        private void loadQuizQuestionsFromJSON()
        {
            try
            {
                string jsonQuizQuestions = string.Empty;
                using (StreamReader sr = new StreamReader(Pathing.SysDir + "\\quiz.json"))
                {
                    jsonQuizQuestions = sr.ReadToEnd();
                }

                questions = JsonConvert.DeserializeObject<QuizQuestions>(jsonQuizQuestions);

                int questionsCount = questions.QuizPictures.Count + questions.QuizWith4Ans.Count + questions.QuizYesNo.Count;
                if (questionsCount == 0)
                {
                    "There are no questions in quiz.json!".Alert();
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
            catch (Exception ex)
            {
                ex.ErrorMessageBox("Dogodila se pogreška !");
            }

            return;
        }
        #endregion

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
                "Dogodila se pogreška prilikom odabira vrste sljedećeg pitanja !".Alert();
                this.Close();
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

            question.Text = pickedQuestion.Question;

            TextBlock a1QuestionTextBlock = new TextBlock();
            a1QuestionTextBlock.Text = pickedQuestion.A1;
            a1QuestionTextBlock.styleTextBlock();

            Button A1 = new Button();
            A1.Content = a1QuestionTextBlock;
            A1.styleButton();

            TextBlock a2QuestionTextBlock = new TextBlock();
            a2QuestionTextBlock.Text = pickedQuestion.A2;
            a2QuestionTextBlock.styleTextBlock();

            Button A2 = new Button();
            A2.Content = a2QuestionTextBlock;
            A2.styleButton();

            TextBlock a3QuestionTextBlock = new TextBlock();
            a3QuestionTextBlock.Text = pickedQuestion.A3;
            a3QuestionTextBlock.styleTextBlock();

            Button A3 = new Button();
            A3.Content = a3QuestionTextBlock;
            A3.styleButton();

            TextBlock a4QuestionTextBlock = new TextBlock();
            a4QuestionTextBlock.Text = pickedQuestion.A4;
            a4QuestionTextBlock.styleTextBlock();

            Button A4 = new Button();
            A4.Content = a4QuestionTextBlock;
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

            question.Text = pickedQuestion.Question;

            TextBlock a1QuestionTextBlock = new TextBlock();
            a1QuestionTextBlock.Text = pickedQuestion.A1;
            a1QuestionTextBlock.styleTextBlock();

            Button A1 = new Button();
            A1.Content = a1QuestionTextBlock;
            A1.styleButton();

            TextBlock a2QuestionTextBlock = new TextBlock();
            a2QuestionTextBlock.Text = pickedQuestion.A2;
            a2QuestionTextBlock.styleTextBlock();

            Button A2 = new Button();
            A2.Content = a2QuestionTextBlock;
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
            question.Text = "Write what you see in this image";

            QuizPictures pickedQuestion = questions.QuizPictures[questionID];

            BitmapImage imageData = new BitmapImage();
            imageData.BeginInit();
                imageData.UriSource = new Uri(Pathing.QuizImgDir + "\\" + pickedQuestion.ImagePath, UriKind.Absolute);
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
        ///     Postavlja vrijednost varijable ovisno o tome da li postoje slike na disku.
        /// </returns>
        private bool checkImages()
        {
            if ( Directory.Exists(Pathing.QuizImgDir) == false )
            {
                "You are missing quiz images.\nYou won't get questions with images in quiz game.".Notify();
                return false;
            }

            List<string> missingPictures = new List<string>();

            foreach (QuizPictures pictureQuestion in questions.QuizPictures)
            {
                if (File.Exists(Pathing.QuizImgDir + "\\" + pictureQuestion.ImagePath) == false)
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

                string infoText = "You are missing these images:\n" + missingPicturesText.ToString() + "\nYou won't get questions with images in quiz game.";
                infoText.Notify();

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
        /// <summary>
        ///     Poziva se kada se pritisne tipka na tipkovnici.
        ///     Ako je pritisnut Enter: uklanja se KeyDown događaj i simulira se pritisak gumba.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Quiz_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (e.Key == Key.Enter)
            {
                this.KeyDown -= Quiz_KeyDown;

                Button pictureQuestionButton = (Button)this.sp.FindName("QuizPictures_btn");

                pictureQuestionButton.RaiseEvent( new RoutedEventArgs( ButtonBase.ClickEvent ) );
            }
            else
            {
                e.Handled = false;
            }

            return;
        }

        /// <summary>
        ///     Poziva se kod pritiska na gumb QuizPictures_btn ili Enter.
        ///     Uzima se vrijednost upisana u QuizPictures_txbx i uspoređuje sa QuizPictures_btn.Tag atributom.
        ///     Ako su vrijednosti iste, poziva se correctAns metoda, inače se poziva wrongAns metoda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkPicAns(object sender, RoutedEventArgs e)
        {
            this.KeyDown -= Quiz_KeyDown;
            
            Button pictureQuestionButton = (Button)sender;
            this.sp.UnregisterName("QuizPictures_btn");

            if (pictureQuestionButton.Tag != null)
            {
                TextBox pictureQuestionTextBox = (TextBox)this.sp.FindName("QuizPictures_txbx");
                this.sp.UnregisterName("QuizPictures_txbx");

                string userAnswer = pictureQuestionTextBox.Text.ToUpper();
                string correctAnswer = pictureQuestionButton.Tag.ToString().ToUpper();

                if ( userAnswer == correctAnswer )
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

        /// <summary>
        ///     Poziva se ako je pritisnut gumb sa TOČNIM odgovorom.
        ///     Također se poziva ako je upisana vrijednost jednaka TOČNOJ vrijednosti u slučaju pitanja sa slikama.
        ///     Dodaje 1 bod.
        ///     Dodaje 5 sekundi više za igranje.
        ///     Boja preostalo vrijeme u zeleno.
        ///     Registrira da nakon 1 sekunde se boja preostalog vremena vrati u normalno.
        ///     Prikazuje slijedeće pitanje.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void correctAns(object sender, RoutedEventArgs e)
        {
            score += Constants.POSITIVE_POINT;

            start = start.AddSeconds(5);

            timer.Foreground = Brushes.ForestGreen;

            colorChanger.Interval = new TimeSpan(0, 0, 0, 1);
            colorChanger.Tick += colorChanger_Tick;
            colorChanger.Start();

            renderNextQuestion();

            return;
        }

        /// <summary>
        ///     Poziva se ako je pritisnut gumb sa KRIVIM odgovorom.
        ///     Također se poziva ako je upisana vrijednost jednaka KRIVOJ vrijednosti u slučaju pitanja sa slikama.
        ///     Skida 1 bod.
        ///     Skida 5 sekundi od preostalog vremena za igranje.
        ///     Boja preostalo vrijeme u crveno.
        ///     Registrira da nakon 1 sekunde se boja preostalog vremena vrati u normalno.
        ///     Prikazuje slijedeće pitanje.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wrongAns(object sender, RoutedEventArgs e)
        {
            score += Constants.NEGATIVE_POINT;

            start = start.AddSeconds(-5);

            timer.Foreground = Brushes.Firebrick;

            colorChanger.Interval = new TimeSpan(0, 0, 0, 1);
            colorChanger.Tick += colorChanger_Tick;
            colorChanger.Start();

            renderNextQuestion();

            return;
        }

        /// <summary>
        ///     Poziva se svakih 50ms proteklog vremena tijekom igranja kviza.
        ///     Osvježava prikaz preostalog vremena.
        ///     Provjerava da li je prošlo vrijeme dozbvoljeno za igranje.
        ///     Ako je prošlo vrijeme za igru, poziva formu za upis rezultat na server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - start;

            string remainingTime = Convert.ToString(Constants.QUIZ_PLAY_TIME - elapsedTime.Seconds);
            timer.Content = "Time left: " + remainingTime + " s";

            if (elapsedTime.Seconds >= Constants.QUIZ_PLAY_TIME)
            {
                dispatcherTimer.Stop();

                SaveScorePrompt window = new SaveScorePrompt(score);

                this.Close();

                window.ShowDialog();
            }

            return;
        }

        /// <summary>
        ///     Poziva se nakon što je prošla jedna sekunda od promijene boje Label-e za prikaz preostalog vremena.
        ///     Zaustavlja brojač vremena.
        ///     Vraća boju Label-e u staro.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorChanger_Tick(object sender, EventArgs e)
        {
            colorChanger.Stop();

            timer.styleLabel();

            return;
        }

        /// <summary>
        ///     Poziva se prilikom gašenja prozora.
        ///     Služi kako bi zaustavili brojač vremena prilikom izlaska iz kviza.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopTimer(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            return;
        }
        #endregion
    }
}
