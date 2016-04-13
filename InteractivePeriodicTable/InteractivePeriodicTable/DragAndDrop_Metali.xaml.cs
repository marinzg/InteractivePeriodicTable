using InteractivePeriodicTable.Models;
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

namespace InteractivePeriodicTable
{
    /// <summary>
    /// Interaction logic for DragAndDrop_Metali.xaml
    /// </summary>
    public partial class DragAndDrop_Metali : Page
    {
        Point startPoint;
        List<Element> elements;
        public DragAndDrop_Metali()
        {
            InitializeComponent();
            HashSet<int> randomNumbers = new HashSet<int>();
            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                randomNumbers.Add(rand.Next(elements.Count - 1));
            }
            //ne da mi se više, ovo dole je za probu i nije prava igrica
            foreach (int i in randomNumbers)
            {
                DragList.Items.Add(elements.ElementAt(i).symbol);
            }

            for (int i = 0; i < elements.Count; i++)
            {
                switch (elements.ElementAt(i).elementCategory)
                {
                    case 1:
                        DropListMetals.Items.Add(elements.ElementAt(i).symbol);
                        break;
                    case 2:
                        DropListMetalloids.Items.Add(elements.ElementAt(i).symbol);
                        break;
                    case 3:
                        DropListNonmetals.Items.Add(elements.ElementAt(i).symbol);
                        break;
                }
            }
        }

        public DragAndDrop_Metali(List<Element> elements)
        {
            this.elements = elements;
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
                    string contact = (string)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject("myFormat", contact);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
                catch {}

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
                string contact = e.Data.GetData("myFormat") as string;
                ListBox listView = sender as ListBox;
                if (listView.Items.Contains(contact))
                {
                    listView.Items.Add(contact);
                    DragList.Items.Remove(contact);
                }
                else
                {
                    MessageBox.Show("Wrong box !!!");
                }
            }
        }
        #endregion
    }
}
