using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using InteractivePeriodicTable.Models;
using System.Text.RegularExpressions;

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
        #endregion
        
        public DragAndDrop_Skupine(List<Element> argElements, List<ElementSubcategory> argSubcategories)
        {
            InitializeComponent();

            this.allElements = argElements;
            this.allSubcategories = argSubcategories;

            StartGame();
        }

        private void StartGame()
        {
            List<Element> tmpElements = new List<Element>();
            //List<Element> elementsToShow = new List<Element>();

            //get 3 random numbers which represent 3 subcategories
            HashSet<int> randomNumbers = GetRandomNumbers(3, allSubcategories.Count - 1);

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

            //select 18 random elements
            randomNumbers = GetRandomNumbers(18, tmpElements.Count - 1, 0);
            //create buttons to be dragged
            foreach (int i in randomNumbers)
            {
                //elementsToShow.Add(tmpElements.ElementAt(i));
                Button elementButton = new Button();
                elementButton.Content = tmpElements.ElementAt(i).symbol;
                elementButton.FontSize = 18;
                elementButton.Height = 60;
                elementButton.Width = 60;
                elementButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                elementButton.VerticalContentAlignment = VerticalAlignment.Center;
                elementButton.Background = Brushes.DarkTurquoise;
                elementButton.FontWeight = FontWeights.SemiBold;
                elementButton.Foreground = Brushes.MidnightBlue;
                DragList.Items.Add(elementButton);
            }

            return;
        }

        /// <summary>
        ///     Metoda generira određen broj nasumičnih brojeva.
        /// </summary>
        /// <param name="howMany">
        ///     Koliko nasumičnih brojeva treba generirati.
        /// </param>
        /// <param name="max">
        ///     Najveći dopušteni generirani broj.
        /// </param>
        /// <param name="min">
        ///     Najmanji dopušteni generirani broj.
        /// </param>
        /// <returns>
        ///     HashSet nasumičnih brojeva.
        /// </returns>
        private HashSet<int> GetRandomNumbers(int howMany, int max, int min = 1)
        {
            HashSet<int> randomNumbers = new HashSet<int>();
            Random randomGenerator = new Random();

            do
            {
                int randomNumber = randomGenerator.Next(min, max);
                if (randomNumbers.Add(randomNumber) == true)
                {
                    howMany--;
                }

            }
            while (howMany > 0);

            return randomNumbers;
        }

        private void Clear()
        {
            //clear listboxes where elements were dropped
            foreach (ListBox l in Utils.VisualChildren.FindVisualChildren<ListBox>(this))
                l.Items.Clear();
            List<string> keys = new List<String>();
            foreach (string key in correctGrouping.Keys)
                keys.Add(key);
            foreach (string s in keys)
            {
                correctGrouping[s] = 0;
            }
            DisplayUpdatedPoints();
            correctGrouping.Clear();
        }

        private void GameOver()
        {
            int score = 0;

            //sum score from all categories
            foreach (string key in correctGrouping.Keys)
                score += correctGrouping[key];

            SaveScorePrompt window = new SaveScorePrompt(score);
            window.ShowDialog();

            Clear();
            StartGame();
        }

        #region drag&drop implementation (from net)
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }
        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed && (
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListBox listView = sender as ListBox;
                ListBoxItem listViewItem =
                    FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                try
                {
                    Button element = (Button)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject("myFormat", element);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
                catch { }

            }
        }
        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void DropList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") ||
                sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

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
                    if(subcategoryId == elementSubcategory)
                    {
                        element.Background = Brushes.LightGreen;
                        UpdatePoints(subcategory, 1);
                    }
                    //user sorted incorrectly
                    else
                    {
                        element.Background = Brushes.MediumVioletRed;
                        UpdatePoints(subcategory, -1);
                    }

                    //resize button 
                    element.Height = element.Width = 35;
                    element.FontSize = 13;

                    //move button
                    DragList.Items.Remove(element as Button);
                    listView.Items.Add(element);
                    
                    DisplayUpdatedPoints();
                    AutoScroll();
                    
                }
                if (!DragList.HasItems) GameOver();
            }
        }
        #endregion

        private void AutoScroll()
        {
            foreach (ListBox lb in Utils.VisualChildren.FindVisualChildren<ListBox>(this))
            {
                if (lb.Items.Count > 0)
                    lb.ScrollIntoView(lb.Items[lb.Items.Count - 1]);
            }
        }

        private void DisplayUpdatedPoints()
        {
            string s = "";
            foreach (string myString in correctGrouping.Keys)
            {
                s = Regex.Replace(myString, @" ", @"_");
                foreach (Label l in Utils.VisualChildren.FindVisualChildren<Label>(this))
                {
                    if (l.Name.ToLower().Contains(s))
                    {
                        l.Content = correctGrouping[myString];
                    }
                }
            }
        }

        private void UpdatePoints(string subcategory, int points)
        {
            if (correctGrouping.ContainsKey(subcategory))
                correctGrouping[subcategory] += points;
            else
                correctGrouping.Add(subcategory, points);
        }
    }
}
