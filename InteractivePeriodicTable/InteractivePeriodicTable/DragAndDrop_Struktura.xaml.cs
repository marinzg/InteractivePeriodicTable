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
using System.Windows.Navigation;
using System.Windows.Shapes;
using InteractivePeriodicTable.Models;

namespace InteractivePeriodicTable
{
    /// <summary>
    /// Interaction logic for DragAndDrop_Struktura.xaml
    /// </summary>
    public partial class DragAndDrop_Struktura : Page
    {
        private List<CrystalStructure> crystalStructures;

        public DragAndDrop_Struktura(List<CrystalStructure> argCrystalStructures)
        {
            this.crystalStructures = argCrystalStructures;
            InitializeComponent();
        }
    }
}
