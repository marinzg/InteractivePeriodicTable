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
    /// <summary>
    /// Interaction logic for DragAndDrop_Skupine.xaml
    /// </summary>
    public partial class DragAndDrop_Skupine : Page
    {
        #region ČLANSKE VARIJABLE
        private Point startPoint;
        private List<Element> allElements;
        private List<ElementSubcategory> allSubcategories;
        private Dictionary<string, int> correctGrouping = new Dictionary<string, int>();
        private List<Button> allButtons = new List<Button>();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DateTime start;
        #endregion

        public DragAndDrop_Skupine(List<Element> argElements, List<ElementSubcategory> argSubcategories)
        {
            InitializeComponent();

            this.Unloaded += stopTimer;

            this.allElements = argElements;
            this.allSubcategories = argSubcategories;

            StartGame();
        }

        #region METODE ZA POČETAK I KRAJ IGRE
        private void StartGame()
        {
            List<Element> tmpElements = new List<Element>();

            //get 3 random numbers which represent 3 subcategories
            HashSet<int> randomNumbers = RandomSetGenerator.Generate(3, allSubcategories.Count - 1);

            int firstRandomNumber = randomNumbers.ElementAt(0);
            tmpElements.AddRange(allElements.Where(el => el.elementSubcategory == firstRandomNumber));
            string firstSubcategoryName = allSubcategories.Where(sc => sc.id == firstRandomNumber).ElementAt(0).name;
            this.groupBoxOne.Header = firstSubcategoryName;
            this.DropListOne.Name = this.labelOnePoints.Name = Regex.Replace(firstSubcategoryName, @" ", @"_");

            int secondRandomNumber = randomNumbers.ElementAt(1);
            tmpElements.AddRange(allElements.Where(el => el.elementSubcategory == secondRandomNumber));
            string secondSubcategoryName = allSubcategories.Where(sc => sc.id == secondRandomNumber).ElementAt(0).name;
            this.groupBoxTwo.Header = secondSubcategoryName;
            this.DropListTwo.Name = this.labelTwoPoints.Name = Regex.Replace(secondSubcategoryName, @" ", @"_");

            int thirdRandomNumber = randomNumbers.ElementAt(2);
            tmpElements.AddRange(allElements.Where(el => el.elementSubcategory == thirdRandomNumber));
            string thirdSubcategoryName = allSubcategories.Where(sc => sc.id == thirdRandomNumber).ElementAt(0).name;
            this.groupBoxThree.Header = thirdSubcategoryName;
            this.DropListThree.Name = this.labelThreePoints.Name = Regex.Replace(thirdSubcategoryName, @" ", @"_");

            //create buttons to be dragged
            DragAndDropDisplay.AddButtons(tmpElements, DragList, allButtons);

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
                    string subcategory = Regex.Replace(Regex.Replace(listView.Name, @"DropList", @"").ToLower(), @"_", @" ");
                    int subcategoryId = allSubcategories.Where(sc => sc.name.Equals(subcategory)).ElementAt(0).id;
                    int elementSubcategory = allElements.Where(el => el.symbol.Equals(element.Content)).ElementAt(0).elementSubcategory;

                    //if user sorted correctly
                    if (subcategoryId == elementSubcategory)
                    {
                        element.Background = Brushes.LightGreen;
                        DragAndDropDisplay.UpdatePoints(correctGrouping, subcategory, Constants.POSITIVE_POINT);
                    }
                    //user sorted incorrectly
                    else
                    {
                        element.Background = Brushes.MediumVioletRed;
                        DragAndDropDisplay.UpdatePoints(correctGrouping, subcategory, Constants.NEGATIVE_POINT);
                    }

                    //resize button 
                    element.Height = element.Width = 35;
                    element.FontSize = 13;

                    //move button
                    DragList.Items.Remove(element as Button);
                    listView.Items.Add(element);

                    DragAndDropDisplay.DisplayUpdatedPoints(correctGrouping, this);
                    //DisplayUpdatedPoints();
                    //AutoScroll();

                }
                if (!DragList.HasItems) GameOver();
            }
        }
        #endregion
    }
}
