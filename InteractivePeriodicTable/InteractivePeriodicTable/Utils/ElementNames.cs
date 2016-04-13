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
        public static string allElements = getAllElements(Pathing.localDir);

        private static string getAllElements(string path)
        {
            StreamReader myFile = new StreamReader(Pathing.resourcesDir + "\\Materijali o elementima\\imena_elemenata.txt");

            string myString = myFile.ReadToEnd();
            myFile.Close();

            return myString;
        }
    }
}
