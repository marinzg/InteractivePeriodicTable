using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using InteractivePeriodicTable.Models;

namespace InteractivePeriodicTable
{
    /// <summary>
    /// Interaction logic for SortElements.xaml
    /// </summary>
    public partial class SortElements : Window
    {
        List<Element> elements;
        List<ElementCategory> categories;
        List<ElementSubcategory> subcategories;
        List<Phase> phases;
        List<CrystalStructure> crystalStructures;

        public SortElements()
        {
            InitializeComponent();
            GetData();
        }

        private void DragAndDropMetali(object sender, RoutedEventArgs e)
        {
            Navigation.Content = new DragAndDrop_Metali(elements);
        }

        private void DragAndDropStanje(object sender, RoutedEventArgs e)
        {
            Navigation.Content = new DragAndDrop_Stanje(elements, phases);
        }

        private void DragAndDropStruktura(object sender, RoutedEventArgs e)
        {
            Navigation.Content = new DragAndDrop_Struktura(crystalStructures);
        }

        private void DragAndDropSkupine(object sender, RoutedEventArgs e)
        {
            Navigation.Content = new DragAndDrop_Skupine(elements, subcategories);
        }

        private void GetData()
        {
            PopulateElements();
            PopulateElementCategories();
            PopulateElementSubcategories();
            PopulatePhases();
            PopulateCrystalStructure();
        }

        private void PopulateElements()
        {
            elements = new List<Element>()
            {
                new Element(){ atomicNumber = 1, name = "Hydrogen", symbol = "H", elementCategory = 3, elementSubcategory = 7, group = 1, block = "s", period = 1, phase = 2, crystalStructure = 1 },
                new Element(){ atomicNumber = 2, name = "Helium", symbol = "He", elementCategory = 3, elementSubcategory = 9, group = 18, block = "s", period = 1, phase = 2, crystalStructure = 1 },
                new Element(){ atomicNumber = 3, name = "Lithium", symbol = "Li", elementCategory = 1, elementSubcategory = 1, group = 1, block = "s", period = 2, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 4, name = "Beryllium", symbol = "Be", elementCategory = 1, elementSubcategory = 2, group = 2, block = "s", period = 2, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 5, name = "Boron", symbol = "B", elementCategory = 2, elementSubcategory = 0, group = 13, block = "p", period = 2, phase = 1, crystalStructure = 3 },
                new Element(){ atomicNumber = 6, name = "Carbon", symbol = "C", elementCategory = 3, elementSubcategory = 7, group = 14, block = "p", period = 2, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 7, name = "Nitrogen", symbol = "N", elementCategory = 3, elementSubcategory = 7, group = 15, block = "p", period = 2, phase = 2, crystalStructure = 1 },
                new Element(){ atomicNumber = 8, name = "Oxygen", symbol = "O", elementCategory = 3, elementSubcategory = 7, group = 16, block = "p", period = 2, phase = 2, crystalStructure = 2 },
                new Element(){ atomicNumber = 9, name = "Fluorine", symbol = "F", elementCategory = 3, elementSubcategory = 8, group = 17, block = "p", period = 2, phase = 2, crystalStructure = 2 },
                new Element(){ atomicNumber = 10, name = "Neon", symbol = "Ne", elementCategory = 3, elementSubcategory = 9, group = 18, block = "p", period = 2, phase = 2, crystalStructure = 2 },
                new Element(){ atomicNumber = 11, name = "Sodium", symbol = "Na", elementCategory = 1, elementSubcategory = 1, group = 1, block = "s", period = 3, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 12, name = "Magnesium", symbol = "Mg", elementCategory = 1, elementSubcategory = 2, group = 2, block = "s", period = 3, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 13, name = "Aluminium", symbol = "Al", elementCategory = 1, elementSubcategory = 6, group = 13, block = "p", period = 3, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 14, name = "Silicon", symbol = "Si", elementCategory = 2, elementSubcategory = 0, group = 14, block = "p", period = 3, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 15, name = "Phosphorus", symbol = "P", elementCategory = 3, elementSubcategory = 7, group = 15, block = "p", period = 3, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 16, name = "Sulfur", symbol = "S", elementCategory = 3, elementSubcategory = 7, group = 16, block = "p", period = 3, phase = 1, crystalStructure = 4 },
                new Element(){ atomicNumber = 17, name = "Chlorine", symbol = "Cl", elementCategory = 3, elementSubcategory = 8, group = 17, block = "p", period = 3, phase = 2, crystalStructure = 4 },
                new Element(){ atomicNumber = 18, name = "Argon", symbol = "Ar", elementCategory = 3, elementSubcategory = 9, group = 18, block = "p", period = 3, phase = 2, crystalStructure = 2 },
                new Element(){ atomicNumber = 19, name = "Potassium", symbol = "K", elementCategory = 1, elementSubcategory = 1, group = 1, block = "s", period = 4, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 20, name = "Calcium", symbol = "Ca", elementCategory = 1, elementSubcategory = 2, group = 2, block = "s", period = 4, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 21, name = "Scandium", symbol = "Sc", elementCategory = 1, elementSubcategory = 5, group = 3, block = "d", period = 4, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 22, name = "Titanium", symbol = "Ti", elementCategory = 1, elementSubcategory = 5, group = 4, block = "d", period = 4, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 23, name = "Vanadium", symbol = "V", elementCategory = 1, elementSubcategory = 5, group = 5, block = "d", period = 4, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 24, name = "Chromium", symbol = "Cr", elementCategory = 1, elementSubcategory = 5, group = 6, block = "d", period = 4, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 25, name = "Manganese", symbol = "Mn", elementCategory = 1, elementSubcategory = 5, group = 7, block = "d", period = 4, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 26, name = "Iron", symbol = "Fe", elementCategory = 1, elementSubcategory = 5, group = 8, block = "d", period = 4, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 27, name = "Cobalt", symbol = "Co", elementCategory = 1, elementSubcategory = 5, group = 9, block = "d", period = 4, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 28, name = "Nickel", symbol = "Ni", elementCategory = 1, elementSubcategory = 5, group = 10, block = "d", period = 4, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 29, name = "Copper", symbol = "Cu", elementCategory = 1, elementSubcategory = 5, group = 11, block = "d", period = 4, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 30, name = "Zinc", symbol = "Zn", elementCategory = 1, elementSubcategory = 5, group = 12, block = "d", period = 4, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 31, name = "Gallium", symbol = "Ga", elementCategory = 1, elementSubcategory = 6, group = 13, block = "p", period = 4, phase = 1, crystalStructure = 4 },
                new Element(){ atomicNumber = 32, name = "Germanium", symbol = "Ge", elementCategory = 2, elementSubcategory = 0, group = 14, block = "p", period = 4, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 33, name = "Arsenic", symbol = "As", elementCategory = 2, elementSubcategory = 0, group = 15, block = "p", period = 4, phase = 1, crystalStructure = 3 },
                new Element(){ atomicNumber = 34, name = "Selenium", symbol = "Se", elementCategory = 3, elementSubcategory = 7, group = 16, block = "p", period = 4, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 35, name = "Bromine", symbol = "Br", elementCategory = 3, elementSubcategory = 8, group = 17, block = "p", period = 4, phase = 3, crystalStructure = 4 },
                new Element(){ atomicNumber = 36, name = "Krypton", symbol = "Kr", elementCategory = 3, elementSubcategory = 9, group = 18, block = "p", period = 4, phase = 2, crystalStructure = 2 },
                new Element(){ atomicNumber = 37, name = "Rubidium", symbol = "Rb", elementCategory = 1, elementSubcategory = 1, group = 1, block = "s", period = 5, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 38, name = "Strontium", symbol = "Sr", elementCategory = 1, elementSubcategory = 2, group = 2, block = "s", period = 5, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 39, name = "Yttrium", symbol = "Y", elementCategory = 1, elementSubcategory = 5, group = 3, block = "d", period = 5, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 40, name = "Zirconium", symbol = "Zr", elementCategory = 1, elementSubcategory = 5, group = 4, block = "d", period = 5, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 41, name = "Niobium", symbol = "Nb", elementCategory = 1, elementSubcategory = 5, group = 5, block = "d", period = 5, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 42, name = "Molybdenum", symbol = "Mo", elementCategory = 1, elementSubcategory = 5, group = 6, block = "d", period = 5, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 43, name = "Technetium", symbol = "Tc", elementCategory = 1, elementSubcategory = 5, group = 7, block = "d", period = 5, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 44, name = "Ruthenium", symbol = "Ru", elementCategory = 1, elementSubcategory = 5, group = 8, block = "d", period = 5, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 45, name = "Rhodium", symbol = "Rh", elementCategory = 1, elementSubcategory = 5, group = 9, block = "d", period = 5, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 46, name = "Palladium", symbol = "Pd", elementCategory = 1, elementSubcategory = 5, group = 10, block = "d", period = 5, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 47, name = "Sliver", symbol = "Ag", elementCategory = 1, elementSubcategory = 5, group = 11, block = "d", period = 5, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 48, name = "Cadmium", symbol = "Cd", elementCategory = 1, elementSubcategory = 5, group = 12, block = "d", period = 5, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 49, name = "Indium", symbol = "In", elementCategory = 1, elementSubcategory = 6, group = 13, block = "p", period = 5, phase = 1, crystalStructure = 5 },
                new Element(){ atomicNumber = 50, name = "Tin", symbol = "Sn", elementCategory = 1, elementSubcategory = 6, group = 14, block = "p", period = 5, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 51, name = "Antimony", symbol = "Sb", elementCategory = 2, elementSubcategory = 0, group = 15, block = "p", period = 5, phase = 1, crystalStructure = 3 },
                new Element(){ atomicNumber = 52, name = "Tellurium", symbol = "Te", elementCategory = 2, elementSubcategory = 0, group = 16, block = "p", period = 5, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 53, name = "Iodine", symbol = "I", elementCategory = 3, elementSubcategory = 8, group = 17, block = "p", period = 5, phase = 1, crystalStructure = 4 },
                new Element(){ atomicNumber = 54, name = "Xenon", symbol = "Xe", elementCategory = 3, elementSubcategory = 9, group = 18, block = "p", period = 5, phase = 2, crystalStructure = 2 },
                new Element(){ atomicNumber = 55, name = "Caesium", symbol = "Cs", elementCategory = 1, elementSubcategory = 1, group = 1, block = "s", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 56, name = "Barium", symbol = "Ba", elementCategory = 1, elementSubcategory = 2, group = 2, block = "s", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 57, name = "Lanthanum", symbol = "La", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 58, name = "Cerium", symbol = "Ce", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 59, name = "Praseodymium", symbol = "Pr", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 60, name = "Neodymium", symbol = "Nd", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 61, name = "Promethium", symbol = "Pm", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 62, name = "Samarium", symbol = "Sm", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 3 },
                new Element(){ atomicNumber = 63, name = "Europium", symbol = "Eu", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 64, name = "Gadolinium", symbol = "Gd", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 65, name = "Terbium", symbol = "Tb", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 66, name = "Dysprosium", symbol = "Dy", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 67, name = "Holmium", symbol = "Ho", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 68, name = "Erbium", symbol = "Er", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 69, name = "Thulium", symbol = "Tm", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 70, name = "Ytterbium", symbol = "Yb", elementCategory = 1, elementSubcategory = 3, group = 0, block = "f", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 71, name = "Lutetium", symbol = "Lu", elementCategory = 1, elementSubcategory = 3, group = 0, block = "d", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 72, name = "Hafnium", symbol = "Hf", elementCategory = 1, elementSubcategory = 5, group = 4, block = "d", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 73, name = "Tantalum", symbol = "Ta", elementCategory = 1, elementSubcategory = 5, group = 5, block = "d", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 74, name = "Tungsten", symbol = "W", elementCategory = 1, elementSubcategory = 5, group = 6, block = "d", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 75, name = "Rhenium", symbol = "Re", elementCategory = 1, elementSubcategory = 5, group = 7, block = "d", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 76, name = "Osmium", symbol = "Os", elementCategory = 1, elementSubcategory = 5, group = 8, block = "d", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 77, name = "Iridium", symbol = "Ir", elementCategory = 1, elementSubcategory = 5, group = 9, block = "d", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 78, name = "Platinum", symbol = "Pt", elementCategory = 1, elementSubcategory = 5, group = 10, block = "d", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 79, name = "Gold", symbol = "Au", elementCategory = 1, elementSubcategory = 5, group = 11, block = "d", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 80, name = "Mercury", symbol = "Hg", elementCategory = 1, elementSubcategory = 5, group = 12, block = "d", period = 6, phase = 3, crystalStructure = 3 },
                new Element(){ atomicNumber = 81, name = "Thallium", symbol = "Tl", elementCategory = 1, elementSubcategory = 6, group = 13, block = "p", period = 6, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 82, name = "Lead", symbol = "Pb", elementCategory = 1, elementSubcategory = 6, group = 14, block = "p", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 83, name = "Bismuth", symbol = "Bi", elementCategory = 1, elementSubcategory = 6, group = 15, block = "p", period = 6, phase = 1, crystalStructure = 3 },
                new Element(){ atomicNumber = 84, name = "Polonium", symbol = "Po", elementCategory = 2, elementSubcategory = 0, group = 16, block = "p", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 85, name = "Astatine", symbol = "At", elementCategory = 3, elementSubcategory = 8, group = 17, block = "p", period = 6, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 86, name = "Radon", symbol = "Rn", elementCategory = 3, elementSubcategory = 9, group = 18, block = "p", period = 6, phase = 2, crystalStructure = 2 },
                new Element(){ atomicNumber = 87, name = "Francium", symbol = "Fr", elementCategory = 1, elementSubcategory = 1, group = 1, block = "s", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 88, name = "Radium", symbol = "Ra", elementCategory = 1, elementSubcategory = 2, group = 2, block = "s", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 89, name = "Actinium", symbol = "Ac", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 90, name = "Thorium", symbol = "Th", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 91, name = "Protactinium", symbol = "Pa", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 5 },
                new Element(){ atomicNumber = 92, name = "Uranium", symbol = "U", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 4 },
                new Element(){ atomicNumber = 93, name = "Neptunium", symbol = "Np", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 4 },
                new Element(){ atomicNumber = 94, name = "Plutonium", symbol = "Pu", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 6 },
                new Element(){ atomicNumber = 95, name = "Americium", symbol = "Am", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 96, name = "Curium", symbol = "Cm", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 97, name = "Berkelium", symbol = "Bk", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 98, name = "Californium", symbol = "Cf", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 99, name = "Einsteinium", symbol = "Es", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 100, name = "Fermium", symbol = "Fm", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 0 },
                new Element(){ atomicNumber = 101, name = "Mendelevium", symbol = "Md", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 0 },
                new Element(){ atomicNumber = 102, name = "Nobelium", symbol = "No", elementCategory = 1, elementSubcategory = 4, group = 0, block = "f", period = 7, phase = 1, crystalStructure = 0 },
                new Element(){ atomicNumber = 103, name = "Lawrencium", symbol = "Lr", elementCategory = 1, elementSubcategory = 4, group = 0, block = "d", period = 7, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 104, name = "Rutherfordium", symbol = "Rf", elementCategory = 1, elementSubcategory = 5, group = 4, block = "d", period = 7, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 105, name = "Dubnium", symbol = "Db", elementCategory = 1, elementSubcategory = 5, group = 5, block = "d", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 106, name = "Seaborgium", symbol = "Sg", elementCategory = 1, elementSubcategory = 5, group = 6, block = "d", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 107, name = "Bohrium", symbol = "Bh", elementCategory = 1, elementSubcategory = 5, group = 7, block = "d", period = 7, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 108, name = "Hassium", symbol = "Hs", elementCategory = 1, elementSubcategory = 5, group = 8, block = "d", period = 7, phase = 1, crystalStructure = 1 },
                new Element(){ atomicNumber = 109, name = "Meitnerium", symbol = "Mt", elementCategory = 1, elementSubcategory = 5, group = 9, block = "d", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 110, name = "Darmstadtium", symbol = "Ds", elementCategory = 1, elementSubcategory = 5, group = 10, block = "d", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 111, name = "Roentgenium", symbol = "Rg", elementCategory = 1, elementSubcategory = 5, group = 11, block = "d", period = 7, phase = 1, crystalStructure = 2 },
                new Element(){ atomicNumber = 112, name = "Copernicium", symbol = "Cn", elementCategory = 1, elementSubcategory = 5, group = 12, block = "d", period = 7, phase = 0, crystalStructure = 1 },
                new Element(){ atomicNumber = 113, name = "Ununtrium", symbol = "Uut", elementCategory = 2, elementSubcategory = 0, group = 13, block = "p", period = 7, phase = 1, crystalStructure = 0 },
                new Element(){ atomicNumber = 114, name = "Flerovium", symbol = "Fl", elementCategory = 2, elementSubcategory = 0, group = 14, block = "p", period = 7, phase = 1, crystalStructure = 0 },
                new Element(){ atomicNumber = 115, name = "Ununpentium", symbol = "Uup", elementCategory = 2, elementSubcategory = 0, group = 15, block = "p", period = 7, phase = 1, crystalStructure = 0 },
                new Element(){ atomicNumber = 116, name = "Livermorium", symbol = "Lv", elementCategory = 2, elementSubcategory = 0, group = 16, block = "p", period = 7, phase = 1, crystalStructure = 0 },
                new Element(){ atomicNumber = 117, name = "Ununseptium", symbol = "Uus", elementCategory = 3, elementSubcategory = 8, group = 17, block = "p", period = 7, phase = 1, crystalStructure = 0 },
                new Element(){ atomicNumber = 118, name = "Ununoctium", symbol = "Uuo", elementCategory = 3, elementSubcategory = 8, group = 18, block = "p", period = 7, phase = 1, crystalStructure = 0 }
            };
        }

        private void PopulateElementCategories()
        {
            categories = new List<ElementCategory>()
            {
                new ElementCategory() {id = 1, name = "metal" },
                new ElementCategory() {id = 2, name = "metalloid" },
                new ElementCategory() {id = 3, name = "nonmetal" }
            };
        }
        private void PopulateElementSubcategories()
        {
            subcategories = new List<ElementSubcategory>()
            {
                new ElementSubcategory() {id = 0, name = "none" },
                new ElementSubcategory() {id = 1, name = "alkali metal" },
                new ElementSubcategory() {id = 2, name = "alkaline earth metal" },
                new ElementSubcategory() {id = 3, name = "lanthanoid" },
                new ElementSubcategory() {id = 4, name = "actinoid" },
                new ElementSubcategory() {id = 5, name = "transtition metal" },
                new ElementSubcategory() {id = 6, name = "posttransition metal" },
                new ElementSubcategory() {id = 7, name = "other nonmetal" },
                new ElementSubcategory() {id = 8, name = "halogen" },
                new ElementSubcategory() {id = 9, name = "neble gas" }
            };
        }
        private void PopulatePhases()
        {
            phases = new List<Phase>()
            {
                new Phase() {id = 0, name = "unknown" },
                new Phase() {id = 1, name = "solid" },
                new Phase() {id = 2, name = "gas" },
                new Phase() {id = 3, name = "liquid" }
            };
        }
        private void PopulateCrystalStructure()
        {
            crystalStructures = new List<CrystalStructure>()
            {
                new CrystalStructure() {id = 0, name = "unknown" },
                new CrystalStructure() {id = 1, name = "hexagonal" },
                new CrystalStructure() {id = 2, name = "cubic" },
                new CrystalStructure() {id = 3, name = "rhombohedral" },
                new CrystalStructure() {id = 4, name = "orthothombic" },
                new CrystalStructure() {id = 5, name = "tetragonal" },
                new CrystalStructure() {id = 6, name = "monoclinic" }
            };
        }
    }
}
