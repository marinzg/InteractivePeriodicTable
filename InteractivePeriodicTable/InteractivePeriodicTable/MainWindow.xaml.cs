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
using System.Text.RegularExpressions;


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
            
            
            
            IEnumerable<string> TestText = new List<string>() { "text1", "text", "text" };
            //OVO JE TEST ZA SEARCHBOX kada ukucavam te..tex..searchbox bi trebo izbacivati ove 3 rijeci


            //browser.LoadCompleted += browser_LoadCompleted;

        }

    


        private void Element_klik(object sender, RoutedEventArgs e)
        {
           
            
                var element = (e.Source as Button);
                
                //Otvori popup_window za webpage-eve
                var popup_window = new Popup_webpage(element.Name.ToString());
                popup_window.Show();
           
        
            //EXAMPLE PATH: C:\\Users\\Marko\\Source\\Repos\\InteractivePeriodicTable\\InteractivePeriodicTable\\InteractivePeriodicTable\\Notepad_resursi+ErazDB\\Web_pages\\Arsenic - Wikipedia, the free encyclopedia.mht
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            listBox.Items.Clear();

            var regexPattern = (textBox.Text.ToString())+"\\w+";
            regexPattern = char.ToUpper(regexPattern[0]) + regexPattern.Substring(1); //prvo slovo veliko

            Match match = Regex.Match(ElementNames.allElements, regexPattern);
            while (match.Success)
            {
                listBox.Items.Add(match.Value.ToString());
                match = match.NextMatch();
                listBox.Visibility = Visibility.Visible;
            }

            if (listBox.Items.IsEmpty || listBox.Items.Count==119)
            {
                listBox.Visibility = Visibility.Collapsed;
            }
        }

    }
   
}
