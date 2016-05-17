using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using InteractivePeriodicTable.Models;
using InteractivePeriodicTable.Utils;

namespace InteractivePeriodicTable
{
    /// <summary>
    /// Interaction logic for DragAndDrop_Struktura.xaml
    /// </summary>
    public partial class DragAndDrop_Struktura : Page
    {
        private Point startPoint;
        private List<Element> allElements;
        private List<CrystalStructure> allSubcategories;
        private Dictionary<string, int> correctGrouping = new Dictionary<string, int>();
        private List<Button> allButtons = new List<Button>();

        public DragAndDrop_Struktura(List<Element> argElements, List<CrystalStructure> argSubcategories)
        {
            InitializeComponent();

            this.allElements = argElements;
            this.allSubcategories = argSubcategories;
            StartGame();
        }

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
        }

        private void GameOver()
        {
            int score = DragAndDropDisplay.GetScore(correctGrouping);

            SaveScorePrompt window = new SaveScorePrompt(score);
            window.ShowDialog();

            DragAndDropDisplay.Clear(this, correctGrouping);
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
