using System;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using InteractivePeriodicTable.ExtensionMethods;
using System.Net;
using InteractivePeriodicTable.Models;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InteractivePeriodicTable.Utils
{
    /// <summary>
    ///     Klasa za pretraživanje kontrola na formi.
    /// </summary>
    public static class VisualChildren
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Klasa za upravljanje path-ovima.
    /// </summary>
    public static class Pathing
    {
        public static readonly string LocalDir = setLocalDir();
        public static readonly string ResourcesDir = LocalDir + @"\Resources";
        public static readonly string SysDir = ResourcesDir + @"\Sys";
        public static readonly string QuizImgDir = ResourcesDir + @"\QuizImages";
        
        private static string setLocalDir()
        {
            string tmp = Directory.GetCurrentDirectory();

            //tmp = tmp.Remove(tmp.IndexOf("\\bin"));

            return tmp;
        }
    }

    /// <summary>
    ///     Klasa koja sadrži imena svih elemenata.
    /// </summary>
    public static class ElementNames
    {
        public static string allElements = getAllElements();

        private static string getAllElements()
        {
            string myString = string.Empty;
            string pathToElementNames = Pathing.ResourcesDir + "\\Materijali o elementima\\imena_elemenata.txt";
            try
            {
                using (StreamReader myFile = new StreamReader(pathToElementNames))
                {
                    myString = myFile.ReadToEnd();
                }
            }
            catch (FileNotFoundException fnfe)
            {
                fnfe.ErrorMessageBox("File not found: imena_elemenata.txt !");
            }
            catch (DirectoryNotFoundException dnfe)
            {
                dnfe.ErrorMessageBox("Directory not found: " + pathToElementNames);
            }
            catch (IOException ioe)
            {
                ioe.ErrorMessageBox("Error trying to read from file.");
            }

            return myString;
        }
    }

    /// <summary>
    ///     Sadrži metode za provjeru internet veze.
    /// </summary>
    public static class InternetConnection
    {
        /// <summary>
        ///     Metoda provjerava da li je kompjuter spojen na internet.
        /// </summary>
        /// <returns>
        ///     true -> spojen je na internet
        ///     false -> nije spojen na internet
        /// </returns>
        public static bool IsConnected()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public static class RandomSetGenerator
    {
        /// <summary>
        /// Generira skup veličine howMany međusobno različitih brojeva
        /// iz intervala [min, max]
        /// </summary>
        /// <param name="howMany">Veličina traženog skupa</param>
        /// <param name="max">Gornja granica - uključivo</param>
        /// <param name="min">Donja granica - uključivo</param>
        /// <returns>Prazan skup ako se traženi broj brojeva ne nalazi u
        ///             intervalu [min,max], traženi broj različitih brojeva inače.</returns>
        public static HashSet<int> Generate(int howMany, int max, int min = 1)
        {
            HashSet<int> randomNumbers = new HashSet<int>();
            if (max - min < howMany)
                return randomNumbers;
            Random r = new Random();
            //get howMany (arg) numbers from min to max
            do
            {
                if (randomNumbers.Add(r.Next(min, max))) howMany--;
            } while (howMany > 0);
            return randomNumbers;
        }
    }

    public static class DragAndDropDisplay
    {
        public static void AddButtons(List<Element> tmpElements, ListBox dragList, List<Button> allButtons)
        {
            HashSet<int> randomNumbers = RandomSetGenerator.Generate(Constants.DRAG_CONTAINER_COUNT, tmpElements.Count - 1, 0);
            foreach (int i in randomNumbers)
            {
                Button b = new Button();
                b.Content = tmpElements.ElementAt(i).symbol;
                b.FontSize = 18;
                b.Height = b.Width = 60;
                b.HorizontalContentAlignment = HorizontalAlignment.Center;
                b.VerticalContentAlignment = VerticalAlignment.Center;
                b.Background = Brushes.DarkTurquoise;
                b.FontWeight = FontWeights.SemiBold;
                b.Foreground = Brushes.MidnightBlue;
                allButtons.Add(b);
                dragList.Items.Add(b);
            }
        }
        public static void DisplayUpdatedPoints(Dictionary<string, int> correctGrouping, Page thisPage)
        {
            String s = "";
            foreach (string tmpString in correctGrouping.Keys)
            {
                s = Regex.Replace(tmpString, @" ", @"_");
                foreach (Label l in Utils.VisualChildren.FindVisualChildren<Label>(thisPage))
                {
                    if (l.Name.ToLower().Contains(s))
                    {
                        l.Content = correctGrouping[tmpString];
                    }
                }
            }
            AutoScroll(thisPage);
        }
        public static void UpdatePoints(Dictionary<string,int> correctGrouping, string box, int points)
        {
            if (correctGrouping.ContainsKey(box))
                correctGrouping[box] += points;
            else
                correctGrouping.Add(box, points);
        }
        private static void AutoScroll(Page thisPage)
        {
            foreach (ListBox lb in VisualChildren.FindVisualChildren<ListBox>(thisPage))
            {
                if (lb.Items.Count > 0)
                    lb.ScrollIntoView(lb.Items[lb.Items.Count - 1]);
            }
        }
        public static void Clear(Page thisPage, Dictionary<string,int> correctGrouping)
        {
            //clear listboxes where elements were dropped
            foreach (ListBox l in VisualChildren.FindVisualChildren<ListBox>(thisPage))
                l.Items.Clear();

            List<string> keys = new List<String>();
            foreach (string key in correctGrouping.Keys)
                keys.Add(key);
            foreach (string s in keys)
            {
                correctGrouping[s] = 0;
            }
            DisplayUpdatedPoints(correctGrouping, thisPage);
            correctGrouping.Clear();
        }
        public static int GetScore(Dictionary<string, int> correctGrouping)
        {
            int score = 0;

            //sum score from all categories
            foreach (string key in correctGrouping.Keys)
                score += correctGrouping[key];

            return score;
        }
        public static void PlayCorrectAnwerSound()//trebam još samo pronaći prikladne zvukove i dodati ih
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Pathing.ResourcesDir+"");
            player.Load();
            Task.Factory.StartNew(() => { player.PlaySync(); });
        }
        public static void PlayWrongAnwerSound()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Pathing.ResourcesDir + "");
            player.Load();
            Task.Factory.StartNew(() => { player.PlaySync(); });
        }
    }
}
