using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;



namespace InteractivePeriodicTable
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, Brush> previousBackgroundColors = new Dictionary<string, Brush>();
        private Dictionary<string, Brush> previousForegroundColors = new Dictionary<string, Brush>();
       
        public User usr;
        

        public MainWindow()
        {
            InitializeComponent();
            listBox.PreviewKeyDown += new KeyEventHandler(listBox_KeyDownOrUp); //Marko-eventhandler za listbox navigiranje arrowsima
            textBox.PreviewKeyDown += new KeyEventHandler(textBox_KeyDown); //Marko-eventhandler za arrow down
        }

        private void Element_klik(object sender, RoutedEventArgs e)
        {
            Button element = (e.Source as Button);

            //Otvori popup_window za webpage-eve
            PopupWebpage popupWindow = new PopupWebpage(element.Name.ToString());
            popupWindow.Show();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            listBox.Items.Clear();
            

            if (textBox.Text.Trim() != "")
            {
                string regexPattern = (textBox.Text.ToString()) + "\\w*";
                regexPattern = char.ToUpper(regexPattern[0]) + regexPattern.Substring(1); //prvo slovo veliko

                Match match = Regex.Match(ElementNames.allElements, regexPattern);
                while (match.Success && match.Value != "")
                {
                    listBox.Items.Add(match.Value.ToString());
                    listBox.Visibility = Visibility.Visible;

                    match = match.NextMatch();
                }
            }

                if (listBox.Items.IsEmpty || listBox.Items.Count == 119)
                {
                    listBox.Visibility = Visibility.Collapsed;
                    if (listBox.Items.Count == 119) listBox.Items.Clear();
                }
            
            HighlightElementsOnTable();
            OtherButtonsHighlight();
            BringBackColors();
        }

        private void BringBackColors()
        {
            if (listBox.Items.IsEmpty)
            {
                foreach (Button allbuttons in Utils.VisualChildren.FindVisualChildren<Button>(this))
                {
                    if (allbuttons.Background == Brushes.Gainsboro)
                    {
                        allbuttons.Background = previousBackgroundColors[allbuttons.Name];
                    }
                }
            }
        }

        private void HighlightElementsOnTable()
        {
            
            foreach (Button buttonInForm in Utils.VisualChildren.FindVisualChildren<Button>(this))
            {
                if (listBox.Items.Contains(buttonInForm.Name.ToString()))
                {
                    buttonInForm.Background = Brushes.DarkBlue;
                    buttonInForm.Foreground = Brushes.Gold;
                }
                else
                {
                    buttonInForm.Background = previousBackgroundColors[buttonInForm.Name];
                    buttonInForm.Foreground = previousForegroundColors[buttonInForm.Name];
                }
            }
        }

        private void OtherButtonsHighlight()
        {
            
            foreach (Button otherButtonsInForm in Utils.VisualChildren.FindVisualChildren<Button>(this))
            {
                if (listBox.Items.Contains(otherButtonsInForm.Name.ToString())==false)
                {
                    otherButtonsInForm.Background = Brushes.Gainsboro;
                }
               

            }
        }

        private void textBox_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Button buttonInForm in Utils.VisualChildren.FindVisualChildren<Button>(this))
            {
                previousForegroundColors.Add(buttonInForm.Name, buttonInForm.Foreground);
            }
            foreach (Button buttonInForm in Utils.VisualChildren.FindVisualChildren<Button>(this))
            {
                previousBackgroundColors.Add(buttonInForm.Name, buttonInForm.Background);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(Path.GetFullPath(@"sys\login.json"), "{ \"user_name\": \"\", \"password\": \"\", \"score\": 0 }");
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }



       /* TREBA POPRAVITI SLJEDECE: KADA SE SEARCHA NA PRIMJER H...otvori se listbox i mozes keydown-om selectat iteme...
          Kada se sa arrow up vrati natrag u textbox i stisne arrow down...krenes selectati buttone umjesto listbox itema..
          ...poplavi button..neznam zasto krene button-e selectat
          kada npr: input h -> keydown -> fokus je na listboxu -> keyup -> fokusira textbox -> keydown -> krene buttone selectat

         */
        


        //Keydown za searchbox -- Urh
        //capture key-a i prebacivanje fokusa na listbox
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key==Key.Down)
            {
                try
                {
                    string text = listBox.SelectedItem.ToString();
                    HighLightSpecificElement(text);
                    DeSelectOtherElements(text);
                }
                catch{}
                listBox.Focus();
            }
            
   
        }

        //Navigacija po listboxu sa key-evima 
        private void listBox_KeyDownOrUp(object sender, KeyEventArgs e)
        {
            
            if(e.Key==Key.Up && listBox.SelectedIndex==0)
            {
                textBox.Focus();
            }
            if (e.Key == Key.Down && (listBox.SelectedIndex < listBox.Items.Count - 1))
            { 
                string text = listBox.Items[listBox.SelectedIndex + 1].ToString();
                HighLightSpecificElement(text);
                DeSelectOtherElements(text);
            }
            if(e.Key==Key.Up && listBox.SelectedIndex>0)
            {
                string text = listBox.Items[listBox.SelectedIndex - 1].ToString();
                HighLightSpecificElement(text);
                DeSelectOtherElements(text);
            }
            

        }

        private void HighLightSpecificElement(string name)
        {
            foreach (Button buttonInForm in Utils.VisualChildren.FindVisualChildren<Button>(this))
            {
                if (name==buttonInForm.Name.ToString())
                {
                    buttonInForm.Background = Brushes.DarkBlue;
                    buttonInForm.Foreground = Brushes.Gold;
                }
                else
                {
                    buttonInForm.Background = previousBackgroundColors[buttonInForm.Name];
                    buttonInForm.Foreground = previousForegroundColors[buttonInForm.Name];
                }
            }
        }

        private void DeSelectOtherElements(string name)
        {
            foreach (Button otherButtonsInForm in Utils.VisualChildren.FindVisualChildren<Button>(this))
            {
                if (name!=otherButtonsInForm.Name.ToString())
                {
                    otherButtonsInForm.Background = Brushes.Gainsboro;
                }


            }
        }


    }
}
