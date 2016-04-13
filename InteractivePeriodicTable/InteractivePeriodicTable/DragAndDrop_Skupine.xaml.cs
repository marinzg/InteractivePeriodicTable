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
        private Point startPoint;
        private List<Element> allElements;
        private List<ElementSubcategory> allSubcategories;
        private Dictionary<string, int> correctGrouping = new Dictionary<string, int>();


        public DragAndDrop_Skupine(List<Element> argElements, List<ElementSubcategory> argSubcategories)
        {
            InitializeComponent();
            this.allElements = argElements;
            this.allSubcategories = argSubcategories;
            StartGame();
        }

        private void StartGame()
        {
            //List<ElementSubcategory> tmpSubcategories = new List<ElementSubcategory>();
            List<Element> tmpElements = new List<Element>();
            List<Element> elementsToShow = new List<Element>();
            HashSet<int> randomNumbers = GetRandomNumbers(3, allSubcategories.Count - 1);

            this.groupBoxOne.Header = allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(0)).ElementAt(0).name;
            this.groupBoxTwo.Header = allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(1)).ElementAt(0).name;
            this.groupBoxThree.Header = allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(2)).ElementAt(0).name;

            this.DropListOne.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(0)).ElementAt(0).name, @" ", @"_");
            this.DropListTwo.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(1)).ElementAt(0).name, @" ", @"_");
            this.DropListThree.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(2)).ElementAt(0).name, @" ", @"_");

            Regex.Replace("", @" ", @"_");

            this.labelOnePoints.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(0)).ElementAt(0).name, @" ", @"_");
            this.labelTwoPoints.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(1)).ElementAt(0).name, @" ", @"_");
            this.labelThreePoints.Name = Regex.Replace(allSubcategories.Where(sc => sc.id == randomNumbers.ElementAt(2)).ElementAt(0).name, @" ", @"_");


            tmpElements.AddRange(allElements.Where(el => el.elementSubcategory == randomNumbers.ElementAt(0)));
            tmpElements.AddRange(allElements.Where(el => el.elementSubcategory == randomNumbers.ElementAt(1)));
            tmpElements.AddRange(allElements.Where(el => el.elementSubcategory == randomNumbers.ElementAt(2)));

            randomNumbers = GetRandomNumbers(18, tmpElements.Count - 1, 0);
            foreach(int i in randomNumbers)
            {
                elementsToShow.Add(tmpElements.ElementAt(i));
                Button b = new Button();
                b.Content = tmpElements.ElementAt(i).symbol;
                b.FontSize = 18;
                b.Height = b.Width = 60;
                b.HorizontalContentAlignment = HorizontalAlignment.Center;
                b.VerticalContentAlignment = VerticalAlignment.Center;
                b.Background = Brushes.AliceBlue;
                DragList.Items.Add(b);
            }
        }

        private HashSet<int> GetRandomNumbers(int howMany, int max, int min = 1)
        {
            HashSet<int> randomNumbers = new HashSet<int>();
            Random r = new Random();
            do
            {
                if (randomNumbers.Add(r.Next(min, max)))
                    howMany--;
            } while (howMany > 0);
            return randomNumbers;
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
                if (element.Parent.Equals(DragList))
                {
                    string subcategory = Regex.Replace(Regex.Replace(listView.Name, @"DropList", @"").ToLower(), @"_", @" "); 

                    int subcategoryId = allSubcategories.Where(sc => sc.name.Equals(subcategory)).ElementAt(0).id;
                    int elementSubcategory = allElements.Where(el => el.symbol.Equals(element.Content)).ElementAt(0).elementSubcategory;

                    if(subcategoryId == elementSubcategory)
                    {
                        element.Background = Brushes.LightGreen;
                        if (correctGrouping.ContainsKey(subcategory))
                        {
                            correctGrouping[subcategory] += 1;
                        }
                        else
                        {
                            correctGrouping.Add(subcategory, 1);
                        }
                    }
                    else
                    {
                        element.Background = Brushes.MediumVioletRed;
                        if (correctGrouping.ContainsKey(subcategory))
                        {
                            correctGrouping[subcategory] -= 1;
                        }
                        else
                        {
                            correctGrouping.Add(subcategory, -1);
                        }
                    }

                    element.Height = element.Width = 35;
                    element.FontSize = 13;
                    DragList.Items.Remove(element as Button);
                    listView.Items.Add(element);
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
                    //autoscroll
                    foreach(ListBox lb in Utils.VisualChildren.FindVisualChildren<ListBox>(this))
                    {
                        if (lb.Items.Count > 0)
                            lb.ScrollIntoView(lb.Items[lb.Items.Count - 1]);
                    }
                }



            }
        }
        #endregion
    }
}
