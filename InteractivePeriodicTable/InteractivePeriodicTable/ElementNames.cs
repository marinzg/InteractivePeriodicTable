using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InteractivePeriodicTable
{
    public static class ElementNames
    {
        public static string allElements = getAllElements(Directory.GetCurrentDirectory());



        private static string getAllElements(string path)
        {
            path = path.Remove(path.IndexOf("\\bin\\"));
            path = path + "\\Notepad_resursi+ErazDB\\imena_elemenata.txt";

            System.IO.StreamReader myFile =
   new StreamReader(path);
            string myString = myFile.ReadToEnd();
            myFile.Close();

            return myString;
        }
    }
}
