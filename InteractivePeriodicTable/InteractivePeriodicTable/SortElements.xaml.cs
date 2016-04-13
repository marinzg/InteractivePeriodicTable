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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using InteractivePeriodicTable.Models;

namespace InteractivePeriodicTable
{
    /// <summary>
    /// Interaction logic for SortElements.xaml
    /// </summary>
    public partial class SortElements : Window
    {
      
        public SortElements()
        {
            InitializeComponent();
        }

        private void DragAndDropMetali(object sender, RoutedEventArgs e)
        {
            Navigation.Content = new DragAndDrop_Metali();
        }

        private void DragAndDropStanje(object sender, RoutedEventArgs e)
        {
            Navigation.Content = new DragAndDrop_Stanje();
        }

        private void DragAndDropStruktura(object sender, RoutedEventArgs e)
        {
            Navigation.Content = new DragAndDrop_Struktura();
        }

        private void DragAndDropSkupine(object sender, RoutedEventArgs e)
        {
            Navigation.Content = new DragAndDrop_Skupine();
        }

      
    }
}
