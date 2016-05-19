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
        /// <summary>
        ///     Sprema key-value parove element button-a i njihovih boja pozadine prije nego se boja promijenila.
        /// </summary>
        private Dictionary<string, Brush> previousBackgroundColors = new Dictionary<string, Brush>();

        /// <summary>
        ///     Sprema key-value parove element button-a i njihovih boja teksta prije nego se boja promijenila.
        /// </summary>
        private Dictionary<string, Brush> previousForegroundColors = new Dictionary<string, Brush>();

        /// <summary>
        ///     Sprema sve zanimljivosti kako bi ih mogli prikazati.
        /// </summary>
        private AllFacts factCollection = new AllFacts();
        #endregion

        /// <summary>
        ///     Povezuje textbox za pretraživanje elemenata i KeyDown događaj.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            textBox.PreviewKeyDown += new KeyEventHandler(txtSearchTerm_KeyDown);
        }

        #region METODA ZA UČITAVANJE ZANIMLJIVOSTI
        /// <summary>
        ///     Dohvaća zanimljivosti iz datoteke i serijalizira ih u objekt facts.
        /// </summary>
        private void getFactsFromJSON()
        {
            string json = string.Empty;
            string pathToFacts = Pathing.SysDir + "\\facts.json";
            try
            {
                using (StreamReader sr = new StreamReader(pathToFacts))
                {
                    json = sr.ReadToEnd();
                }

                factCollection = JsonConvert.DeserializeObject<AllFacts>(json);
            }
            catch (FileNotFoundException fnfe)
            {
                fnfe.ErrorMessageBox("facts.json file not found !");
            }
            catch (DirectoryNotFoundException dnfe)
            {
                dnfe.ErrorMessageBox("Directory not found " + pathToFacts);
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
        #endregion

        #region DOGAĐAJI
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
                fact_tip.Text = "There are no facts on your local drive! Try to do an update!";
                fact_tip.Visibility = Visibility.Visible;
            }
            else
            {
                if (factCollection.Facts.Count == 0)
                {
                    getFactsFromJSON();
                }

                Random rand = new Random();
                int no_of_facts = factCollection.Facts.Count;
                int fact_no = rand.Next(0, no_of_facts);

                try
                {
                    fact_tip.Text = factCollection.Facts[fact_no].Fact;
                }
                catch (ArgumentOutOfRangeException ioor)
                {
                    ioor.ErrorMessageBox("There are no facts on your local drive!\nfacts.json is empty.");
                    return;
                }

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
                    getFactsFromJSON();
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
            Button element = e.Source as Button;
            try
            {
                PopupWebpage popupWindow = new PopupWebpage(element.Name);
                popupWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.ErrorMessageBox("Error while trying to open element information");
            }

            return;
        }

        /// <summary>
        ///     Metoda uklanja fokus sa search textbox-a.
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
                ex.ErrorMessageBox("Error while trying to open element information");
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
            ListBox chosenItem = e.Source as ListBox;
            string elementName = chosenItem.SelectedValue.ToString();

            highLightSpecificElement(elementName);
            deSelectOtherElements(elementName);

            return;
        }

        /// <summary>
        /// Mimic "searchboxa" koristenjem listbox+textbox pošto wpf nema textbox sa autocomplete ko u Winform... Textchanged event se fire-a kako upisujemo text pretrazuje listu sa svim elementima i izdvaja match-eve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            listBox.Items.Clear();

            if (string.IsNullOrWhiteSpace(textBox.Text) == false)
            {
                string regexPattern = textBox.Text + "\\w*";
                regexPattern = char.ToUpper(regexPattern[0]) + regexPattern.Substring(1); //prvo slovo veliko

                Match match = Regex.Match(ElementNames.allElements, regexPattern);
                while (match.Success && match.Value != "")
                {
                    listBox.Items.Add(match.Value);

                    match = match.NextMatch();
                }

                listBox.Visibility = Visibility.Visible;
                listBox.IsEnabled = true;
            }

            if (listBox.Items.IsEmpty)
            {
                listBox.Visibility = Visibility.Collapsed;
            }

            if (listBox.Items.Count == 119)
            {
                if (listBox.Items.Count == 119)
                {
                    listBox.Items.Clear();
                }
            }

            highlightElementsOnTable();
            otherButtonsHighlight();
            bringBackColors();

            return;
        }

        /// <summary>
        ///     Metoda sprema boje svih element buttona kako bi kasnije mogli vratiti im boje na staro.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Button elementButton in VisualChildren.FindVisualChildren<Button>(this))
            {
                previousForegroundColors.Add(elementButton.Name, elementButton.Foreground);
                previousBackgroundColors.Add(elementButton.Name, elementButton.Background);
            }

            return;
        }

        /// <summary>
        ///     Navigacija unutar listboxa sa key-up down.
        ///     Moguć prelazak iz listboxa unutar textboxa sa key up i nastavak ubacivanja inputa te izlazak iz textboxa i prelazak u listbox sa keydown eventom.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearchTerm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                int numberOfElementsInListBox = listBox.Items.Count - 1;
                if (listBox.SelectedIndex < numberOfElementsInListBox)
                {
                    listBox.SelectedIndex++;
                    highLightSpecificElement(listBox.SelectedItem.ToString());
                    deSelectOtherElements(listBox.SelectedItem.ToString());
                }

                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                if (listBox.SelectedIndex > 0)
                {
                    listBox.SelectedIndex--;
                    highLightSpecificElement(listBox.SelectedItem.ToString());
                    deSelectOtherElements(listBox.SelectedItem.ToString());
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
                    ex.ErrorMessageBox("Error while trying to open element information");
                }
            }
            else
            {
                e.Handled = false;
            }

            return;
        }

        /// <summary>
        ///     Metoda se poziva kada korisnik odluči igrati Drag&Drop igru.
        ///     Pokušava otvoriti Drag&Drop igru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            return;
        }

        /// <summary>
        ///     Metoda se poziva kada je promijenjen odabir u listbox-u.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox chosenItem = e.Source as ListBox;

            if (chosenItem.Items.Count != 0)
            {
                highLightSpecificElement(chosenItem.SelectedValue.ToString());
                deSelectOtherElements(chosenItem.SelectedValue.ToString());
            }
            else
            {
                listBox.Visibility = Visibility.Collapsed;
                listBox.Items.Clear();
            }

            return;
        }
        #endregion

        #region METODE ZA OZNAČAVANJE/ODZNAČAVANJE GUMBIJU
        /// <summary>
        ///     Metoda vraća stare boje buttonima.
        /// </summary>
        private void bringBackColors()
        {
            if (listBox.Items.IsEmpty)
            {
                foreach (Button elementButton in VisualChildren.FindVisualChildren<Button>(this))
                {
                    if (elementButton.Background == Brushes.Gainsboro)
                    {
                        elementButton.Background = previousBackgroundColors[ elementButton.Name ];
                    }
                }
            }

            return;
        }

        /// <summary>
        ///     Metoda označava buttone iz searchboxa.
        /// </summary>
        private void highlightElementsOnTable()
        {
            foreach (Button elementButton in VisualChildren.FindVisualChildren<Button>(this))
            {
                if ( listBox.Items.Contains(elementButton.Name) == true )
                {
                    elementButton.Background = Brushes.DarkBlue;
                    elementButton.Foreground = Brushes.Gold;
                }
                else
                {
                    elementButton.Background = previousBackgroundColors[elementButton.Name];
                    elementButton.Foreground = previousForegroundColors[elementButton.Name];
                }
            }

            return;
        }

        /// <summary>
        ///     Metoda postavlja boju buttona koji nisu u listbox-u u sivo.
        /// </summary>
        private void otherButtonsHighlight()
        {
            foreach (Button elementButton in VisualChildren.FindVisualChildren<Button>(this))
            {
                if (elementButton.Name != "play_quiz" &&
                    elementButton.Name != "show_scoreboard" &&
                    elementButton.Name != "update" &&
                    elementButton.Name != "DragDropGames" &&
                    listBox.Items.Contains(elementButton.Name) == false)
                {
                    elementButton.Background = Brushes.Gainsboro;
                }
            }

            return;
        }

        /// <summary>
        ///     Metoda označava element button čiji naziv smo predali kao parametar.
        /// </summary>
        /// <param name="name">
        ///     Jedinstveno označava element button.
        /// </param>
        private void highLightSpecificElement(string elementName)
        {
            foreach (Button elementButton in VisualChildren.FindVisualChildren<Button>(this))
            {
                if (elementName == elementButton.Name)
                {
                    elementButton.Background = Brushes.DarkBlue;
                    elementButton.Foreground = Brushes.Gold;
                }
                else
                {
                    elementButton.Background = previousBackgroundColors[elementButton.Name];
                    elementButton.Foreground = previousForegroundColors[elementButton.Name];
                }
            }

            return;
        }

        /// <summary>
        ///     Metoda odznačava ostale element buttone kada je odredjeni odabran "gray out"
        /// </summary>
        /// <param name="name">
        ///     Jedinstveno označava element button.
        /// </param>
        private void deSelectOtherElements(string elementName)
        {
            foreach (Button elementButton in VisualChildren.FindVisualChildren<Button>(this))
            {
                if (elementButton.Name != "play_quiz" &&
                    elementButton.Name != "show_scoreboard" &&
                    elementButton.Name != "update" &&
                    elementButton.Name != "DragDropGames" &&
                    elementName != elementButton.Name)
                {
                    elementButton.Background = Brushes.Gainsboro;
                }
            }

            return;
        }
        #endregion
    }
}
