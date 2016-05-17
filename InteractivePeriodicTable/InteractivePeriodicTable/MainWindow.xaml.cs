using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;
using InteractivePeriodicTable.Utils;
using InteractivePeriodicTable.Data;
using InteractivePeriodicTable.ExtensionMethods;

namespace InteractivePeriodicTable
{
    public partial class MainWindow : Window
    {
        #region ČLANSKE VARIJABLE
        private Dictionary<string, Brush> previousBackgroundColors = new Dictionary<string, Brush>();
        private Dictionary<string, Brush> previousForegroundColors = new Dictionary<string, Brush>();
        private AllFacts factCollection = new AllFacts();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            //listBox.PreviewKeyDown += new KeyEventHandler(listBox_KeyDownOrUp); //Marko-eventhandler za listbox navigiranje arrowsima
            //textBox.PreviewKeyDown += new KeyEventHandler(textBox_KeyDown); //Marko-eventhandler za arrow down
            textBox.PreviewKeyDown += new KeyEventHandler(txtSearchTerm_KeyDown);
        }



        #region FACTS
        /// <summary>
        ///     Dohvaća zanimljivosti iz datoteke i serijalizira ih u objekt facts.
        /// </summary>
        private void getFactsFromJSON()
        {
            try
            {
                string json = "";
                using (StreamReader sr = new StreamReader(Pathing.SysDir + "\\facts.json"))
                {
                    json = sr.ReadToEnd();
                }
                factCollection = JsonConvert.DeserializeObject<AllFacts>(json);
            }
            catch (FileNotFoundException fnfe)
            {
                fnfe.ErrorMessageBox("Quiz.json file not found !");
            }
            catch (DirectoryNotFoundException dnfe)
            {
                dnfe.ErrorMessageBox("Directory not found " + Pathing.SysDir);
            }
            catch (IOException ioe)
            {
                ioe.ErrorMessageBox("File reading error");
            }
            catch (Exception ex)
            {
                ex.ErrorMessageBox("Unknown error !");
            }

            return;
        }

        /// <summary>
        ///     Prikazuje nasumičnu zanimljivost.
        ///     Poziva se na klik i hover.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fact_ShowFact(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Pathing.SysDir + "\\facts.json") == false)
            {
                "There are no facts on your local drive! Try to do an update!".Alert();
                return;
            }
            else
            {
                if(factCollection.Facts.Count == 0)
                {
                    getFactsFromJSON();
                }

                Random rand = new Random();
                int no_of_facts = factCollection.Facts.Count;
                int fact_no = rand.Next(0, no_of_facts);

                fact_tip.Text = factCollection.Facts[fact_no].Fact;
                fact_tip.Visibility = Visibility.Visible;
            }

            return;
        }

        /// <summary>
        ///     Sakriva trenutnu zanimljivost.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fact_Leave(object sender, RoutedEventArgs e)
        {
            fact_tip.Visibility = Visibility.Hidden;

            return;
        }
        #endregion




        #region DOGAĐAJI
        /// <summary>
        ///     Poziva se kada netko hoće update-ati kviz i zanimljivosti.
        ///     Ako ima veze s internetom, skidaju se kviz i zanimljivosti.
        ///     Ako nema veze s internetom javlja se poruka o pogrešci.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InternetConnection.IsConnected() == true)
                {
                    Update up = new Update();
                    up.updateQuiz();
                    up.updateFacts();
                    "Quiz questions and answers were succesfully updated!".Notify();
                }
                else
                {
                    "You are not connected to internet!".Alert();
                }
            }
            catch (Exception ex)
            {
                ex.ErrorMessageBox("Error while trying to update quiz and facts questions");
            }

            return;
        }

        /// <summary>
        ///     Provjerava da li postoji quiz.json na disku.
        ///     Ako ne postoji, zabranjuje igranje kviza.
        ///     Ako postoji, omogućuje igranje kviza.
        /// </summary>
        private void playQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Pathing.SysDir + "\\quiz.json") == false)
            {
                "Questions do not exist on your local drive, try to do an Update!".Alert();
                return;
            }

            Quiz window = new Quiz();
            window.ShowDialog();

            return;
        }

        /// <summary>
        ///     Ako postoji internet veza prikazuje Scoreboard.
        ///     Inače javlja obavijest.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showScoreBoard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InternetConnection.IsConnected() == true)
                {
                    ScoreBoard window = new ScoreBoard();
                    window.ShowDialog();
                }
                else
                {
                    "You are not connected to internet!".Alert();
                }
            }
            catch (Exception ex)
            {
                ex.ErrorMessageBox("Error trying to open scoreboard!");
            }

            return;
        }

        /// <summary>
        ///     Ako je kliknut button koji fire-a event Element_klik.
        ///     Otvara se nova forma (popup window) koja loada informacije o elementu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Element_klik(object sender, RoutedEventArgs e)
        {
            Button element = (e.Source as Button);

            try
            {
                PopupWebpage popupWindow = new PopupWebpage(element.Name.ToString());
                popupWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.ErrorMessageBox("Error while trying to open element information");
            }

            return;
        }

        /// <summary>
        ///     Uklanja fokus sa search textbox-a
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();

            return;
        }

        /// <summary>
        ///     Poziva se ako dvaput kliknemo na element prikazan u DropDownListi iz searchbox-a.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PopupWebpage popupWindow = new PopupWebpage(listBox.SelectedItem.ToString());
                popupWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.ErrorMessageBox("Error while trying to open element information!");
            }

            return;
        }

        /// <summary>
        ///     Ukoliko prijeđemo mišem određeni element, označiti će se.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBox chosenItem = (e.Source as ListBox);
            string elementName = chosenItem.SelectedValue.ToString();

            HighLightSpecificElement(elementName);
            DeSelectOtherElements(elementName);

            return;
        }
        #endregion

        #region Highlighting i searchbox funkcionalnost

        /// <summary>
        /// Mimic "searchboxa" koristenjem listbox+textbox pošto wpf nema textbox sa autocomplete ko u Winform... Textchanged event se fire-a kako upisujemo text pretrazuje listu sa svim elementima i izdvaja match-eve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    listBox.IsEnabled = true;

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

        /// <summary>
        /// vraca stare boje buttonima
        /// </summary>
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

        /// <summary>
        /// highlighta buttone iz searchboxa
        /// </summary>
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


        /// <summary>
        /// ostali buttoni su "gray-ed out"
        /// </summary>
        private void OtherButtonsHighlight()
        {

            foreach (Button otherButtonsInForm in Utils.VisualChildren.FindVisualChildren<Button>(this))
            {
                if (otherButtonsInForm.Name.ToString() != "play_quiz" && otherButtonsInForm.Name.ToString() != "show_scoreboard" && otherButtonsInForm.Name.ToString() != "update" && otherButtonsInForm.Name.ToString() != "DragDropGames")
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

        

        /// <summary>
        /// Navigacija unutar listboxa sa key-up down... Moguc prelazak iz listboxa unutar textboxa sa key up i nastavak ubacivanja inputa te izlazak iz textboxa i prelazak u listbox sa keydown eventom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            else if (e.Key == Key.Enter)
            {
                try
                {
                    PopupWebpage popupWindow = new PopupWebpage(listBox.SelectedItem.ToString());
                    popupWindow.Show();
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    ErrorHandle.ErrorMessageBox(ex, "Error while trying to open element information");
                }
            }
        }

        /// <summary>
        /// highlighta tocno odabrani element
        /// </summary>
        /// <param name="name"></param>
        private void HighLightSpecificElement(string name)
        {
            foreach (Button buttonInForm in Utils.VisualChildren.FindVisualChildren<Button>(this))
            {
                if (name == buttonInForm.Name.ToString())
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

        /// <summary>
        /// de selekcija ostlaih elemenata kada je odredjeni odabran "gray out"
        /// </summary>
        /// <param name="name"></param>
        private void DeSelectOtherElements(string name)
        {
            foreach (Button otherButtonsInForm in Utils.VisualChildren.FindVisualChildren<Button>(this))
            {
                if (otherButtonsInForm.Name.ToString() != "play_quiz" && otherButtonsInForm.Name.ToString() != "show_scoreboard" && otherButtonsInForm.Name.ToString() != "update" && otherButtonsInForm.Name.ToString() != "DragDropGames")
                {
                    if (name != otherButtonsInForm.Name.ToString())
                    {
                        otherButtonsInForm.Background = Brushes.Gainsboro;
                    }
                }


            }
        }


        private void play_DragDrop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SortElements window = new SortElements();
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.ErrorMessageBox("Can not open Sort elements game!");
            }
        }


       
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox chosenItem = (e.Source as ListBox);
          
            if (chosenItem.Items.Count != 0)
            {
                HighLightSpecificElement(chosenItem.SelectedValue.ToString());
                DeSelectOtherElements(chosenItem.SelectedValue.ToString());
            }
            else
            {
                listBox.Visibility = Visibility.Collapsed;
                listBox.Items.Clear();
            }
        }

        #endregion



    }
}
