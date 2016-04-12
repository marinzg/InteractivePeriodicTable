using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IO;

namespace InteractivePeriodicTable
{
    public partial class Quiz : Window
    {
        private QuizQuestions questions = new QuizQuestions();
        private int score = 0;
        private DateTime start;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private Random rand = new Random();
        public Quiz()
        {
            Closing += QuitQuiz;

            ((App)Application.Current).checkQuiz();

            getQuestions();
            InitializeComponent();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);

            start = DateTime.Now;
            dispatcherTimer.Start();

            pickQuestion();
        }
        private void getQuestions()
        {
            string json = "";
            using (StreamReader sr = new StreamReader(Pathing.sysDir + "/quiz.json"))
            {
                json = sr.ReadToEnd();
            }
            questions = JsonConvert.DeserializeObject<QuizQuestions>(json);

            return;
        }
        private void pickQuestion()
        {
            if (this.sp.Children.Count > 0)
            {
                this.sp.Children.Clear();
            }

            scr_lbl.Content = "Score: " + score.ToString();

            byte question_type = (byte)rand.Next(0, 3); // 0->QuizWith4Ans, 1->QuizYesNo, 2->QuizPictures

            if( question_type == 0 )
            {
                int no_QuizWith4Ans = questions.QuizWith4Ans.Count;
                int question_no = rand.Next(0, no_QuizWith4Ans);
                
                renderQuizWith4Ans(question_no);
            }
            else if( question_type == 1 )
            {
                int no_QuizYesNo = questions.QuizYesNo.Count;
                int question_no = rand.Next(0, no_QuizYesNo);

                renderQuizYesNo(question_no);
            }
            else if ( question_type == 2 )
            {
                int no_QuizPictures = questions.QuizPictures.Count;
                int question_no = rand.Next(0, no_QuizPictures);

                renderQuizPictures(question_no);
            }

            return;
        }
        private void renderQuizWith4Ans(int question_no)
        {
            QuizWith4Ans picked_question = questions.QuizWith4Ans[question_no];

            Label question_lbl = new Label();
            question_lbl.Content = picked_question.Question;

            Button A1 = new Button();
            A1.Content = picked_question.A1;

            Button A2 = new Button();
            A2.Content = picked_question.A2;

            Button A3 = new Button();
            A3.Content = picked_question.A3;

            Button A4 = new Button();
            A4.Content = picked_question.A4;

            if(picked_question.Answer == "1")
            {
                A1.Click += correctAns;
                A2.Click += wrongAns;
                A3.Click += wrongAns;
                A4.Click += wrongAns;
            }
            else if (picked_question.Answer == "2")
            {
                A1.Click += wrongAns;
                A2.Click += correctAns;
                A3.Click += wrongAns;
                A4.Click += wrongAns;
            }
            else if (picked_question.Answer == "3")
            {
                A1.Click += wrongAns;
                A2.Click += wrongAns;
                A3.Click += correctAns;
                A4.Click += wrongAns;
            }
            else if (picked_question.Answer == "4")
            {
                A1.Click += wrongAns;
                A2.Click += wrongAns;
                A3.Click += wrongAns;
                A4.Click += correctAns;
            }

            this.sp.Children.Add(question_lbl);
            this.sp.Children.Add(A1);
            this.sp.Children.Add(A2);
            this.sp.Children.Add(A3);
            this.sp.Children.Add(A4);

            return;
        }
        private void renderQuizYesNo(int question_no)
        {
            QuizYesNo picked_question = questions.QuizYesNo[question_no];

            Label question_lbl = new Label();
            question_lbl.Content = picked_question.Question;

            Button A1 = new Button();
            A1.Content = picked_question.A1;

            Button A2 = new Button();
            A2.Content = picked_question.A2;

            if (picked_question.Answer == "1")
            {
                A1.Click += correctAns;
                A2.Click += wrongAns;
            }
            else if (picked_question.Answer == "2")
            {
                A1.Click += wrongAns;
                A2.Click += correctAns;
            }

            this.sp.Children.Add(question_lbl);
            this.sp.Children.Add(A1);
            this.sp.Children.Add(A2);

            return;
        }
        private void renderQuizPictures(int question_no)
        {
            QuizPictures picked_question = questions.QuizPictures[question_no];

            Image image = new Image();
            
            image.Width = 300;
            image.Height = 300;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(Pathing.imgDir + "\\" + picked_question.ImagePath, UriKind.Absolute);
            bi.EndInit();

            image.Source = bi;

            TextBox txbx = new TextBox();
            txbx.Name = "QuizPictures_txbx";

            this.sp.RegisterName(txbx.Name, txbx);

            Button btn = new Button();
            btn.Content = "OK";
            btn.Tag = picked_question.Answer;
            btn.Click += correctPicAns;

            this.sp.Children.Add(image);
            this.sp.Children.Add(txbx);
            this.sp.Children.Add(btn);

            return;
        }
        private void correctPicAns(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Tag != null)
            {
                TextBox txbx = (TextBox)this.sp.FindName("QuizPictures_txbx");
                this.sp.UnregisterName("QuizPictures_txbx");

                if (txbx.Text == btn.Tag.ToString())
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
            pickQuestion();
            return;
        }
        private void wrongAns(object sender, RoutedEventArgs e)
        {
            score -= 1;
            pickQuestion();
            return;
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - start;
            timer.Content = "Left: " + Convert.ToString(30 - elapsed.Seconds) + " s";

            if (elapsed.Seconds >= 30)
            {
                dispatcherTimer.Stop();

                SaveScorePrompt window = new SaveScorePrompt(score);
                window.ShowDialog();

                this.Close();
            }

            return;
        }
        public void QuitQuiz(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
        }
    }
}
