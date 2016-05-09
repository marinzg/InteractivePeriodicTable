using System;
using System.Windows;
using System.Windows.Navigation;
using InteractivePeriodicTable.Utils;

namespace InteractivePeriodicTable
{
    /// <summary>
    /// Interaction logic for Popup_webpage.xaml
    /// </summary>
    public partial class PopupWebpage : Window
    {

        public PopupWebpage(string elementName)
        {
            InitializeComponent();
            this.Title = elementName;
            browser1.LoadCompleted += browser1_LoadCompleted;
            string uri = Pathing.ResourcesDir + "\\Web_pages\\" + elementName + " - Wikipedia, the free encyclopedia.mht";


            //Navigiranje na stranicu
            browser1.Navigate(new Uri(uri, UriKind.Absolute));
        }


        //Prikaži kada se load complete-a
        void browser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            browser1.Visibility = Visibility.Visible;
            browser1.Navigating += browser1_Navigating;

        }


        //Onemogućuje klikanje linkova
        void browser1_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = true;
        }



    }
}
