using System;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using InteractivePeriodicTable.ExtensionMethods;

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
        public static readonly string ResourcesDir = LocalDir + "\\Resources";
        public static readonly string SysDir = ResourcesDir + "\\Sys\\";
        public static readonly string ImgDir = ResourcesDir + "\\Images\\Quiz\\";

        private static string setLocalDir()
        {
            string tmp = Directory.GetCurrentDirectory();

            tmp = tmp.Remove(tmp.IndexOf("\\bin"));

            return tmp;
        }
    }

    /// <summary>
    ///     Klasa koja sadrži imena svih elemenata.
    /// </summary>
    public static class ElementNames
    {
        public static string allElements = getAllElements(Pathing.LocalDir);

        private static string getAllElements(string path)
        {
            string myString = string.Empty;
            try
            {
                using (StreamReader myFile = new StreamReader(Pathing.ResourcesDir + "\\Materijali o elementima\\imena_elemenata.txt"))
                {
                    myString = myFile.ReadToEnd();
                }
            }
            catch (FileNotFoundException fnfe)
            {
                fnfe.ErrorMessageBox("Nije pronađena datoteka quiz.json !");
            }
            catch (DirectoryNotFoundException dnfe)
            {
                dnfe.ErrorMessageBox("Nije pronađen direktorij " + Pathing.SysDir);
            }
            catch (IOException ioe)
            {
                ioe.ErrorMessageBox("Greška prilikom čitanja iz datoteke.");
            }

            return myString;
        }
    }
}
