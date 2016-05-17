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
    ///Interaction logic for DragAndDrop_Metali.xaml
    /// </summary>
    public partial class DragAndDrop_Metali : Page
    {
        private Point startPoint;
        private List<Element> allElements;
        private List<ElementCategory> categories;
        private Dictionary<string, int> correctGrouping = new Dictionary<string, int>();

        public DragAndDrop_Metali(List<Element> argElements, List<ElementCategory> argCategories)
        {
            this.allElements = argElements;
            this.categories = argCategories;

            InitializeComponent();
            StartGame();
        }
        
        private void StartGame()
        {
            //create buttons to be dragged
            DragAndDropDisplay.AddButtons(allElements, DragList);
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
                    listView.Items.Add(element);

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
