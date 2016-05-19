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
using InteractivePeriodicTable.ExtensionMethods;
using System.Windows.Threading;

namespace InteractivePeriodicTable
{
    public partial class DragAndDrop_Metali : Page
    {
        #region ČLANSKE VARIJABLE
        private Point startPoint;
        private List<Element> allElements;
        private List<ElementCategory> categories;
        private Dictionary<string, int> correctGrouping = new Dictionary<string, int>();
        private List<Button> allButtons = new List<Button>();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private DateTime start;
        #endregion

        public DragAndDrop_Metali(List<Element> argElements, List<ElementCategory> argCategories)
        {
            this.allElements = argElements;
            this.categories = argCategories;

            InitializeComponent();

            this.Unloaded += stopTimer;

            StartGame();
        }

        #region METODE ZA POČETAK I KRAJ IGRE
        /// <summary>
        ///     Metoda postavlja gumbe elemenata.
        ///     Pokreće brojač vremena,
        /// </summary>
        private void StartGame()
        {
            DragAndDropDisplay.AddButtons(allElements, DragList, allButtons);

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);

            start = DateTime.Now;
            dispatcherTimer.Start();

            return;
        }

        /// <summary>
        ///     Metoda računa ukupni rezultat.
        ///     Pokreće ekran za spremanje rezultata.
        ///     Čisti ekran od prošle igre.
        ///     Započinje novu igru.
        /// </summary>
        private void GameOver()
        {
            int score = DragAndDropDisplay.GetScore(correctGrouping);

            SaveScorePrompt window = new SaveScorePrompt(score, Game.DragDrop);
            window.ShowDialog();

            DragAndDropDisplay.Clear(this, correctGrouping);

            StartGame();

            return;
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
                ListBox listBox = sender as ListBox;

                //there was a bug that tried drop element twice
                if (element.Parent != null && element.Parent.Equals(DragList))
                {
                    string subcategory = Regex.Replace(Regex.Replace(listBox.Name, @"DropList", @"").ToLower(), @"_", @" ");
                    int subcategoryId = categories.Where(sc => sc.name.Equals(subcategory)).ElementAt(0).id;
                    int elementSubcategory = allElements.Where(el => el.symbol.Equals(element.Content)).ElementAt(0).elementCategory;

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
                    listBox.Items.Add(element);

                    DragAndDropDisplay.DisplayUpdatedPoints(correctGrouping, this);
                }

                if (DragList.HasItems == false)
                {
                    GameOver();
                }
            }

            return;
        }
        #endregion
    }
}
