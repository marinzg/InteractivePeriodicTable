﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;


namespace InteractivePeriodicTable
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, Brush> previousBackgroundColors = new Dictionary<string, Brush>();
        private Dictionary<string, Brush> previousForegroundColors = new Dictionary<string, Brush>();
        private AllFacts dyk = new AllFacts();
        public MainWindow()
        {
            getFactsJSON();
            InitializeComponent();


            textBox.PreviewKeyDown += new KeyEventHandler(txtSearchTerm_KeyDown);

           

        }

        private void Element_klik(object sender, RoutedEventArgs e)
        {
            Button element = (e.Source as Button);

            //Otvori popup_window za webpage-eve
            PopupWebpage popupWindow = new PopupWebpage(element.Name.ToString());
            popupWindow.ShowDialog();
        }

        private void Did_you_know_hover(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).checkFacts();

            Random rand = new Random();
            int no_of_facts = dyk.Facts.Count;
            int fact_no = rand.Next(0, no_of_facts);
            
            fact_tip.Text = dyk.Facts[fact_no].Fact;
            return;
        }
        private void getFactsJSON()
        {
            string json = "";
            using (StreamReader sr = new StreamReader(Pathing.sysDir+"\\facts.json"))
            {
                json = sr.ReadToEnd();
            }
            dyk = JsonConvert.DeserializeObject<AllFacts>(json);

            return;
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
                if (otherButtonsInForm.Name.ToString() != "play_quiz" && otherButtonsInForm.Name.ToString() != "show_scoreboard" && otherButtonsInForm.Name.ToString() != "update")
                {
                    if (listBox.Items.Contains(otherButtonsInForm.Name.ToString()) == false)
                    {
                        otherButtonsInForm.Background = Brushes.Gainsboro;
                    }
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


        /*Key down event...prebacuje sada sa textboxa na listbox i selecta lijepo mislim da je ok sada malo se poigrajte -- Urh*/


        private void txtSearchTerm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (listBox.SelectedIndex < (listBox.Items.Count - 1))
                {
                    
                    listBox.SelectedIndex++;
                    HighLightSpecificElement(listBox.SelectedItem.ToString());
                    DeSelectOtherElements(listBox.SelectedItem.ToString());
                }

                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                if (listBox.SelectedIndex > 0)
                {
                    listBox.SelectedIndex--;
                    HighLightSpecificElement(listBox.SelectedItem.ToString());
                    DeSelectOtherElements(listBox.SelectedItem.ToString());
                }
                e.Handled = true;
            }
            else if (e.Key==Key.Enter)
            {
                PopupWebpage popupWindow = new PopupWebpage(listBox.SelectedItem.ToString());
                popupWindow.ShowDialog();
                e.Handled = true;
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
                if(otherButtonsInForm.Name.ToString() != "play_quiz" && otherButtonsInForm.Name.ToString() != "show_scoreboard" && otherButtonsInForm.Name.ToString() != "update")
                {
                    if (name != otherButtonsInForm.Name.ToString())
                    {
                        otherButtonsInForm.Background = Brushes.Gainsboro;
                    }
                }


            }
        }

        private void play_quiz_Click(object sender, RoutedEventArgs e)
        {
            Quiz window = new Quiz();
            window.ShowDialog();

            return;
        }

        private void show_scoreboard_Click(object sender, RoutedEventArgs e)
        {
            ScoreBoard window = new ScoreBoard();
            window.ShowDialog();
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((App)Application.Current).updateFacts();
                ((App)Application.Current).updateQuiz();
                MessageBox.Show("Quiz and Facts updated successfully !", "Information");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return;
        }

        private void Viewbox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}
