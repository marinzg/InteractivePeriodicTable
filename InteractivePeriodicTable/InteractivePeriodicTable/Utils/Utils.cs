using System;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using InteractivePeriodicTable.ExtensionMethods;
using InteractivePeriodicTable.Data;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Input;

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
        public static readonly string LocalDir = Directory.GetCurrentDirectory();
        public static readonly string ResourcesDir = LocalDir + @"\Resources";
        public static readonly string SysDir = ResourcesDir + @"\Sys";
        public static readonly string QuizImgDir = ResourcesDir + @"\QuizImages";
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
            {
                return randomNumbers;
            }
                
            Random r = new Random();

            do
            {
                if (randomNumbers.Add(r.Next(min, max)))
                {
                    howMany --;
                }
            }
            while (howMany > 0);

            return randomNumbers;
        }
    }

    /// <summary>
    ///     Sadrži metode za upravljanje kontrolama u Drag&Drop igri.
    /// </summary>
    public static class DragAndDropDisplay
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="tmpElements"></param>
        /// <param name="dragList"></param>
        /// <param name="allButtons"></param>
        public static void AddButtons(List<Element> tmpElements, ListBox dragList, List<Button> allButtons)
        {
            HashSet<int> randomNumbers = RandomSetGenerator.Generate(Constants.DRAG_CONTAINER_COUNT, tmpElements.Count - 1, 0);
            foreach (int i in randomNumbers)
            {
                Button elementButton = new Button();
                elementButton.Content = tmpElements.ElementAt(i).symbol;
                elementButton.FontSize = 18;
                elementButton.Height = 60;
                elementButton.Width = 60;
                elementButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                elementButton.VerticalContentAlignment = VerticalAlignment.Center;
                elementButton.Background = Brushes.DarkTurquoise;
                elementButton.FontWeight = FontWeights.SemiBold;
                elementButton.Foreground = Brushes.MidnightBlue;

                allButtons.Add(elementButton);
                dragList.Items.Add(elementButton);
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correctGrouping"></param>
        /// <param name="thisPage"></param>
        public static void DisplayUpdatedPoints(Dictionary<string, int> correctGrouping, Page thisPage)
        {
            string s = string.Empty;
            foreach (string tmpString in correctGrouping.Keys)
            {
                s = Regex.Replace(tmpString, @" ", @"_");

                foreach (Label l in VisualChildren.FindVisualChildren<Label>(thisPage))
                {
                    if (l.Name.ToLower().Contains(s))
                    {
                        l.Content = correctGrouping[tmpString];
                    }
                }
            }

            AutoScroll(thisPage);

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correctGrouping"></param>
        /// <param name="box"></param>
        /// <param name="points"></param>
        public static void UpdatePoints(Dictionary<string,int> correctGrouping, string box, int points)
        {
            if (correctGrouping.ContainsKey(box))
            {
                correctGrouping[box] += points;
            }
            else
            {
                correctGrouping.Add(box, points);
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisPage"></param>
        private static void AutoScroll(Page thisPage)
        {
            foreach (ListBox lb in VisualChildren.FindVisualChildren<ListBox>(thisPage))
            {
                if (lb.Items.Count > 0)
                {
                    int itemPosition = lb.Items.Count - 1;
                    lb.ScrollIntoView( lb.Items[ itemPosition ] );
                }
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisPage"></param>
        /// <param name="correctGrouping"></param>
        public static void Clear(Page thisPage, Dictionary<string,int> correctGrouping)
        {
            foreach (ListBox l in VisualChildren.FindVisualChildren<ListBox>(thisPage))
            {
                l.Items.Clear();
            }

            List<string> keys = new List<string>();
            foreach (string key in correctGrouping.Keys)
            {
                keys.Add(key);
            }
                
            foreach (string s in keys)
            {
                correctGrouping[s] = 0;
            }

            DisplayUpdatedPoints(correctGrouping, thisPage);

            correctGrouping.Clear();

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correctGrouping"></param>
        /// <returns></returns>
        public static int GetScore(Dictionary<string, int> correctGrouping)
        {
            int score = 0;

            foreach (string key in correctGrouping.Keys)
            {
                score += correctGrouping[key];
            }

            return score;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PlayCorrectAnwerSound()//trebam još samo pronaći prikladne zvukove i dodati ih
        {
            SoundPlayer player = new SoundPlayer(Pathing.ResourcesDir+"");
            player.Load();
            Task.Factory.StartNew(() => { player.PlaySync(); });

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PlayWrongAnwerSound()
        {
            SoundPlayer player = new SoundPlayer(Pathing.ResourcesDir + "");
            player.Load();
            Task.Factory.StartNew(() => { player.PlaySync(); });

            return;
        }
    }

    public static class DragAndDropHelper
    {
        /// <summary>
        ///     Helper to search up the VisualTree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="listBox"></param>
        /// <param name="e"></param>
        public static void CalcPointerPosition(Point startPoint, ListBox listBox, MouseEventArgs e)
        {
            // Uzmi trenutnu poziciju miša
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed && (
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Pronađi listbox u koji je odvučen gumb
                ListBoxItem listViewItem = FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                // Pronađi podatke iza ListViewItem-a
                try
                {
                    Button element = (Button)listBox.ItemContainerGenerator.ItemFromContainer(listViewItem);

                    // Pokreni Drag&Drop
                    DataObject dragData = new DataObject("myFormat", element);

                    if (element.IsPressed)
                    {
                        DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                    }
                }
                catch (Exception ex)
                {
                    ex.ErrorMessageBox("There was an error trying to drag/drop button.");
                }
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void DragEnter(object sender, ref DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="e"></param>
        public static void MouseMove(ref Point startPoint, MouseEventArgs e)
        {
            startPoint = e.GetPosition(null);

            return;
        }
    }
}
