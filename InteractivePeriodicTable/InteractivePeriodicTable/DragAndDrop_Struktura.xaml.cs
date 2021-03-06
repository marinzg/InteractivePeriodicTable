﻿using System;
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
    public partial class DragAndDrop_Struktura : Page
    {
        #region ČLANSKE VARIJABLE
        private Point startPoint;
        private List<Element> allElements;
        private List<CrystalStructure> allSubcategories;
        private Dictionary<string, int> correctGrouping = new Dictionary<string, int>();
        private List<Button> allButtons = new List<Button>();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DateTime start;
        #endregion

        public DragAndDrop_Struktura(List<Element> argElements, List<CrystalStructure> argSubcategories)
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
            HashSet<int> randomNumbers = Utils.RandomSetGenerator.Generate(3, allSubcategories.Count - 1);

            //display name of each subcategory
            this.groupBoxOne.Header = allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(0)).ElementAt(0).name;
            this.groupBoxTwo.Header = allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(1)).ElementAt(0).name;
            this.groupBoxThree.Header = allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(2)).ElementAt(0).name;

            //rename drop lists so furher evaluation could be possible
            this.DropListOne.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(0)).ElementAt(0).name, @" ", @"_");
            this.DropListTwo.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(1)).ElementAt(0).name, @" ", @"_");
            this.DropListThree.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(2)).ElementAt(0).name, @" ", @"_");

            //rename labels so you know which label represents which subcategory
            this.labelOnePoints.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(0)).ElementAt(0).name, @" ", @"_");
            this.labelTwoPoints.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(1)).ElementAt(0).name, @" ", @"_");
            this.labelThreePoints.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(2)).ElementAt(0).name, @" ", @"_");

            //add elements only from these 3 subcategories
            tmpElements.AddRange(allElements.Where(el => el.elementSubcategory == randomNumbers.ElementAt(0)));
            tmpElements.AddRange(allElements.Where(el => el.elementSubcategory == randomNumbers.ElementAt(1)));
            tmpElements.AddRange(allElements.Where(el => el.elementSubcategory == randomNumbers.ElementAt(2)));


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
                        //UpdatePoints(subcategory, 1);
                        DragAndDropDisplay.UpdatePoints(correctGrouping, subcategory, Constants.POSITIVE_POINT);
                    }
                    //user sorted incorrectly
                    else
                    {
                        element.Background = Brushes.MediumVioletRed;
                        //UpdatePoints(subcategory, -1);
                        DragAndDropDisplay.UpdatePoints(correctGrouping, subcategory, Constants.NEGATIVE_POINT);
                    }

                    //resize button 
                    element.Height = element.Width = 35;
                    element.FontSize = 13;

                    //move button
                    DragList.Items.Remove(element as Button);
                    listView.Items.Add(element);

                    //DisplayUpdatedPoints();
                    DragAndDropDisplay.DisplayUpdatedPoints(correctGrouping, this);
                    //AutoScroll();

                }
                if (!DragList.HasItems) GameOver();
            }
        }
        #endregion
    }
}
