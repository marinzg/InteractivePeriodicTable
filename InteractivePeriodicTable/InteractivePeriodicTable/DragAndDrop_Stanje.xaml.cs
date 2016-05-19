using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using InteractivePeriodicTable.Utils;
using InteractivePeriodicTable.Data;
using System.Windows.Threading;

namespace InteractivePeriodicTable
{
    public partial class DragAndDrop_Stanje : Page
    {
        #region ČLANSKE VARIJABLE
        private Point startPoint;
        private List<Element> allElements;
        private List<Phase> phases;
        private Dictionary<string, int> correctGrouping = new Dictionary<string, int>();
        private List<Button> allButtons = new List<Button>();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DateTime start;
        #endregion

        public DragAndDrop_Stanje(List<Element> argElements, List<Phase> argPhases)
        {
            InitializeComponent();

            this.Unloaded += stopTimer;

            this.allElements = argElements;
            this.phases = argPhases;
            StartGame();
        }

        #region METODE ZA POČETAK I KRAJ IGRE
        private void StartGame()
        {
            //create buttons to be dragged
            DragAndDropDisplay.AddButtons(allElements, DragList, allButtons);

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);

            start = DateTime.Now;
            dispatcherTimer.Start();
        }

        private void GameOver()
        {
            int score = DragAndDropDisplay.GetScore(correctGrouping);

            SaveScorePrompt window = new SaveScorePrompt(score, Game.DragDrop);
            window.ShowDialog();

            DragAndDropDisplay.Clear(this, correctGrouping);
            StartGame();
        }
        #endregion

        #region DOGAĐAJI
        /// <summary>
        ///     Metoda se poziva svakih 50ms odpočetka igre.
        ///     Osvježava prikaz preostalog vremena.
        ///     U slučaju da je vrijeme isteklo, prekida igru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - start;

            string remainingTime = Convert.ToString(Constants.DD_PLAY_TIME - elapsedTime.Seconds);
            timer.Content = "Time left: " + remainingTime + " s";

            if (elapsedTime.Seconds >= Constants.DD_PLAY_TIME)
            {
                dispatcherTimer.Stop();

                GameOver();
            }

            return;
        }

        /// <summary>
        ///     Metoda gasi timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopTimer(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();

            return;
        }

        /// <summary>
        ///     Metoda pohranjuje poziciju miša prilikom klika.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragAndDropHelper.MouseMove(ref startPoint, e);

            return;
        }

        /// <summary>
        ///     Metoda računa poziciju vučenog gumba.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            DragAndDropHelper.CalcPointerPosition(startPoint, (ListBox)sender, e);

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropList_DragEnter(object sender, DragEventArgs e)
        {
            DragAndDropHelper.DragEnter(sender, ref e);

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                Button element = e.Data.GetData("myFormat") as Button;
                ListBox listView = sender as ListBox;

                //there was a bug that tried drop element twice
                if (element.Parent != null && element.Parent.Equals(DragList))
                {
                    string phase = Regex.Replace(listView.Name, @"DropList", @"").ToLower();
                    int phaseId = phases.Where(p => p.name.Equals(phase)).ElementAt(0).id;

                    int elementPhase = allElements.Where(el => el.symbol.Equals(element.Content)).ElementAt(0).phase;

                    //if user sorted correctly
                    if (phaseId == elementPhase)
                    {
                        element.Background = Brushes.LightGreen;
                        DragAndDropDisplay.UpdatePoints(correctGrouping, phase, Constants.POSITIVE_POINT);
                    }
                    //user sorted incorrectly
                    else
                    {
                        element.Background = Brushes.MediumVioletRed;
                        DragAndDropDisplay.UpdatePoints(correctGrouping, phase, Constants.NEGATIVE_POINT);
                    }

                    //resize button  
                    element.Height = element.Width = 35;
                    element.FontSize = 13;

                    //move button
                    DragList.Items.Remove(element as Button);
                    listView.Items.Add(element);

                    DragAndDropDisplay.DisplayUpdatedPoints(correctGrouping, this);
                }
                if (!DragList.HasItems) GameOver();
            }
        }
        #endregion
    }
}
