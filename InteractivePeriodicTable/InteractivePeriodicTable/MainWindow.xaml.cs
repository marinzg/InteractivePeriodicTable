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
using System.IO;

namespace InteractivePeriodicTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            
        }

        private void Element_klik(object sender, RoutedEventArgs e)
        {
            string[] tekst1 = System.IO.File.ReadAllLines(@"C:\Users\Marko\Source\Repos\InteractivePeriodicTable\InteractivePeriodicTable\InteractivePeriodicTable\Notepad_resursi+ErazDB\broj_ime_simbol.txt");
            string[] tekst2 = System.IO.File.ReadAllLines(@"C:\Users\Marko\Source\Repos\InteractivePeriodicTable\InteractivePeriodicTable\InteractivePeriodicTable\Notepad_resursi+ErazDB\elementi.txt");
            var element = (e.Source as Button);

            foreach (string i in tekst1)
            {
                

                if (i.Contains(element.Name.ToString().Trim()))
                {
                    string brojstr = i.Substring(0, 2).Trim();
                    Console.WriteLine(brojstr);
                    int broj=Convert.ToInt32(brojstr);
                    element.Content = tekst2[broj-1];
                }
            }


        }



    }
}
