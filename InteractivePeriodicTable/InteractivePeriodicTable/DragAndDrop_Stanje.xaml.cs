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
    /// Interaction logic for DragAndDrop_Stanje.xaml
    /// </summary>
    public partial class DragAndDrop_Stanje : Page
    {
        private Point startPoint;
        private List<Element> allElements;
        private List<Models.Models> phases;
        private Dictionary<string, int> correctGrouping = new Dictionary<string, int>();

        
        public DragAndDrop_Stanje(List<Element> argElements, List<Models.Models> argPhases)
        {
            InitializeComponent();
            
            this.allElements = argElements;
            this.phases = argPhases;
            StartGame();
        }
        
        private void StartGame()
        {
            List<Element> tmpElements = GetElements();

            //create buttons to be dragged
            foreach(Element e in tmpElements)
            {
                Button b = new Button();
                b.Content = e.symbol;
                b.FontSize = 18;
                b.Height = b.Width = 60;
                b.HorizontalContentAlignment = HorizontalAlignment.Center;
                b.VerticalContentAlignment = VerticalAlignment.Center;
                b.Background = Brushes.DarkTurquoise;
                b.FontWeight = FontWeights.SemiBold;
                b.Foreground = Brushes.MidnightBlue;
                DragList.Items.Add(b);
            }
        }
        
        private List<Element> GetElements()
        {
            List<Element> tmpElements = new List<Element>();
            HashSet<int> indexes = new HashSet<int>();          //set can only hold unique elements
            Random r = new Random();            
            int numberOfElements = 18;

            //get 18 unique numbers
            do {
                if (indexes.Add(r.Next(allElements.Count - 1))) numberOfElements--;
            } while (numberOfElements != 0);

            foreach (int i in indexes)
                tmpElements.Add(allElements.ElementAt(i));

            return tmpElements;
        }

        private void Clear()
        {
            //clear listboxes where elements were dropped
            foreach (ListBox l in Utils.VisualChildren.FindVisualChildren<ListBox>(this))
                l.Items.Clear();
            List<string> keys = new List<String>();
            foreach (string key in correctGrouping.Keys)
                keys.Add(key);
            foreach(string s in keys)
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
            foreach(string key in correctGrouping.Keys)
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
                    string phase = Regex.Replace(listView.Name, @"DropList", @"").ToLower();
                    int phaseId = phases.Where(p => p.name.Equals(phase)).ElementAt(0).id;
                    int elementPhase = allElements.Where(el => el.symbol.Equals(element.Content)).ElementAt(0).phase;

                    //if user sorted correctly
                    if (phaseId == elementPhase)
                    {
                        element.Background = Brushes.LightGreen;
                        UpdatePoints(phase, +1);
                    }
                    //user sorted incorrectly
                    else
                    {
                        element.Background = Brushes.MediumVioletRed;
                        UpdatePoints(phase, -1);
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
            //auto scroll all the lists
            foreach (ListBox lb in Utils.VisualChildren.FindVisualChildren<ListBox>(this))
            {
                if (lb.Items.Count > 0)
                    lb.ScrollIntoView(lb.Items[lb.Items.Count - 1]);
            }
        }

        private void DisplayUpdatedPoints()
        {
            foreach (string s in correctGrouping.Keys)
            {
                foreach (Label l in Utils.VisualChildren.FindVisualChildren<Label>(this))
                {
                    if (l.Name.ToLower().Contains(s))
                    {
                        l.Content = correctGrouping[s];
                    }
                }
            }
        }

        private void UpdatePoints(string phase, int points)
        {
            if (correctGrouping.ContainsKey(phase))
                correctGrouping[phase] += points;
            else
                correctGrouping.Add(phase, points);
        }
    }
}
