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

namespace InteractivePeriodicTable
{
    public partial class Quiz : Window
    {
        private QuizQuestions questions = new QuizQuestions();
        private int score = 0;
        private bool hasImages = false;
        private DateTime start;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DispatcherTimer colorChanger = new DispatcherTimer();
        private Random rand = new Random();
        public Quiz()
        {
            this.Closing += stopTimer;

            getQuestionsFromJSON();

            hasImages = checkImages();

            InitializeComponent();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);

            start = DateTime.Now;
            dispatcherTimer.Start();

            pickQuestion();
        }
        private void getQuestionsFromJSON()
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

            byte question_type = 0;
            if (hasImages == true)
            {
                question_type = (byte)rand.Next(0, 3); // 0->QuizWith4Ans, 1->QuizYesNo, 2->QuizPictures
            }
            else
            {
                question_type = (byte)rand.Next(0, 2); // 0->QuizWith4Ans, 1->QuizYesNo, 2->QuizPictures
            }

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
            question_lbl.Foreground = Brushes.Blue;
            question_lbl.FontSize = 14;
            question_lbl.FontWeight = FontWeights.SemiBold;

            Button A1 = new Button();
            A1.Content = picked_question.A1;
            styleButton(A1);

            Button A2 = new Button();
            A2.Content = picked_question.A2;
            styleButton(A2);

            Button A3 = new Button();
            A3.Content = picked_question.A3;
            styleButton(A3);

            Button A4 = new Button();
            A4.Content = picked_question.A4;
            styleButton(A4);

            if (picked_question.Answer == "1")
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

            addButtonsRandomly(new List<Button>() { A1,A2,A3,A4 });

            return;
        }
        private void renderQuizYesNo(int question_no)
        {
            QuizYesNo picked_question = questions.QuizYesNo[question_no];

            Label question_lbl = new Label();
            question_lbl.Content = picked_question.Question;
            question_lbl.Foreground = Brushes.Blue;
            question_lbl.FontSize = 14;
            question_lbl.FontWeight = FontWeights.SemiBold;

            Button A1 = new Button();
            A1.Content = picked_question.A1;
            styleButton(A1);

            Button A2 = new Button();
            A2.Content = picked_question.A2;
            styleButton(A2);

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
            addButtonsRandomly(new List<Button>() { A1, A2 });

            return;
        }
        private void renderQuizPictures(int question_no)
        {
            QuizPictures picked_question = questions.QuizPictures[question_no];

            Label lbl = new Label();
            lbl.Content = "Write what you see in this image";
            lbl.FontSize = 14;
            lbl.FontWeight = FontWeights.SemiBold;
            lbl.Foreground = Brushes.Blue;

            Image image = new Image();
            image.Width = 300;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(Pathing.imgDir + "\\" + picked_question.ImagePath, UriKind.Absolute);
            bi.EndInit();

            image.Source = bi;

            TextBox txbx = new TextBox();
            txbx.Name = "QuizPictures_txbx";
            txbx.TextAlignment = TextAlignment.Center;
            txbx.Width = 120;
            txbx.Margin = new Thickness(0, 5, 0, 5);

            this.sp.RegisterName(txbx.Name, txbx);

            Button btn = new Button();
            btn.Content = "OK";
            styleButton(btn);
            btn.Tag = picked_question.Answer;
            btn.Click += checkPicAns;

            this.sp.Children.Add(lbl);
            this.sp.Children.Add(image);
            this.sp.Children.Add(txbx);
            this.sp.Children.Add(btn);

            txbx.Focus();

            return;
        }
        private bool checkImages()
        {
            List<string> missingPictures = new List<string>();

            foreach (QuizPictures qp in questions.QuizPictures)
            {
                if (File.Exists(Pathing.imgDir + "\\" + qp.ImagePath) == false)
                {
                    missingPictures.Add(qp.ImagePath);
                }
            }

            if (missingPictures.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in missingPictures)
                {
                    sb.AppendLine(s);
                }
                MessageBox.Show("You are missing these images:\n" + sb.ToString() + "\n You won't get questions with images in quiz game.", "Missing images !");

                return false;
            }

            return true;
        }
        private void styleButton(Button btn)
        {
            btn.Width = 120;
            btn.Height = 35;
            btn.Margin = new Thickness(0,5,0,5);
            btn.Background = Brushes.DeepSkyBlue;
            btn.Foreground = Brushes.Blue;
            return;
        }
        private void addButtonsRandomly(List<Button> btns)
        {
            List<byte> order = new List<byte>();

            while(order.Count < btns.Count)
            {
                byte order_no = (byte)rand.Next(0, btns.Count);

                if (order.Exists(x => x == order_no) == false)
                {
                    order.Add(order_no);
                }
            }

            foreach (byte b in order)
            {
                this.sp.Children.Add(btns[b]);
            }

            return;
        }

        private void checkPicAns(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            styleButton(btn);

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
            
            colorChanger.Interval = new TimeSpan(0,0,0,1);
            colorChanger.Tick += colorChanger_Tick;
            colorChanger.Start();

            pickQuestion();

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

            pickQuestion();

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

            timer.Foreground = Brushes.Black;

            return;
        }
        private void stopTimer(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            return;
        }
    }
}
