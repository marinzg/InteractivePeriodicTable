using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private List<Phase> phases;
        private Dictionary<string, int> correctGrouping = new Dictionary<string, int>();
        public DragAndDrop_Stanje(List<Element> argElements, List<Phase> argPhases)
        {
            InitializeComponent();
            
            this.allElements = argElements;
            this.phases = argPhases;
            StartGame();
        }

        private void StartGame()
        {
            List<Element> tmpElements = GetElements();
            foreach(Element e in tmpElements)
            {
                Button b = new Button();
                b.Content = e.symbol;
                b.FontSize = 18;
                b.Height = b.Width = 60;
                b.HorizontalContentAlignment = HorizontalAlignment.Center;
                b.VerticalContentAlignment = VerticalAlignment.Center;
                b.Background = Brushes.AliceBlue;
                DragList.Items.Add(b);
            }
        }

        private List<Element> GetElements()
        {
            List<Element> tmpElements = new List<Element>();
            Random r = new Random();
            HashSet<int> indexes = new HashSet<int>();
            int numberOfElements = 18;
            do
            {
                bool result = indexes.Add(r.Next(allElements.Count - 1));
                if (result)
                    numberOfElements--;
            } while (numberOfElements != 0);
            foreach(int i in indexes)
            {
                tmpElements.Add(allElements.ElementAt(i));
            }
            return tmpElements;
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
                if(element.Parent.Equals(DragList))
                {
                    string phase = Regex.Replace(listView.Name, @"DropList", @"").ToLower();
                    int phaseId = phases.Where(p => p.name.Equals(phase)).ElementAt(0).id;
                    int elementPhase = allElements.Where(el => el.symbol.Equals(element.Content)).ElementAt(0).phase;
                    if (phaseId == elementPhase)
                    {
                        element.Background = Brushes.LightGreen;
                        if(correctGrouping.ContainsKey(phase))
                        {
                            correctGrouping[phase] += 1;
                        }
                        else
                        {
                            correctGrouping.Add(phase, 1);
                        }
                    }
                    else
                    {
                        element.Background = Brushes.MediumVioletRed;
                        if (correctGrouping.ContainsKey(phase))
                        {
                            correctGrouping[phase] -= 1;
                        }
                        else
                        {
                            correctGrouping.Add(phase, -1);
                        }
                    }
                        
                    element.Height = element.Width = 35;
                    element.FontSize = 13;
                    DragList.Items.Remove(element as Button);
                    listView.Items.Add(element);
                    foreach(string s in correctGrouping.Keys)
                    {
                        foreach (Label l in Utils.VisualChildren.FindVisualChildren<Label>(this))
                        {
                            if(l.Name.ToLower().Contains(s))
                            {
                                l.Content = correctGrouping[s];
                            }
                        }
                    }
                    //autoscroll
                    if(DropListSolid.Items.Count > 0)
                        DropListSolid.ScrollIntoView(DropListSolid.Items[DropListSolid.Items.Count - 1]);
                    if(DropListGas.Items.Count > 0)
                        DropListGas.ScrollIntoView(DropListGas.Items[DropListGas.Items.Count - 1]);
                    if(DropListLiquid.Items.Count > 0)
                        DropListLiquid.ScrollIntoView(DropListLiquid.Items[DropListLiquid.Items.Count - 1]);
                }
                
                    
                
            }
        }
        #endregion


    }

}
