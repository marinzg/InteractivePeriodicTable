﻿using System;
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
            System.IO.StreamReader myFile =
   new StreamReader(PathingHelper.resourcesDir + "\\imena_elemenata.txt");

            string myString = myFile.ReadToEnd();
            myFile.Close();

            return myString;
        }
    }
}
