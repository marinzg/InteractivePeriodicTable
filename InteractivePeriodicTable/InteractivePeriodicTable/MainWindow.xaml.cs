using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

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
    }
}
