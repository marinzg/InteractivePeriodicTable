using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using InteractivePeriodicTable.Utils;
using InteractivePeriodicTable.ExtensionMethods;

namespace InteractivePeriodicTable
{
    public partial class PopupWebpage : Window
    {
        public PopupWebpage(string elementName)
        {
            InitializeComponent();

            this.Title = elementName;
            browser1.LoadCompleted += browser1_LoadCompleted;
            string path = Pathing.ResourcesDir + "\\Web_pages\\" + elementName + " - Wikipedia, the free encyclopedia.mht";

            if(File.Exists(path) == true)
            {
                try
                {
                    browser1.Navigate(new Uri(path, UriKind.Absolute));
                }
                catch (InvalidOperationException ex)
                {
                    ex.ErrorMessageBox("Error while reading a file on hard drive!");
                }
                catch(System.Security.SecurityException ex)
                {
                    ex.ErrorMessageBox("Please check your security settings!");
                }
            }
            else
            {
                if (InternetConnection.IsConnected() == true)
                {
                    string uri = "https://en.wikipedia.org/wiki/" + elementName;
                    try
                    {
                        browser1.Navigate(uri);
                    }
                    catch (Exception ex)
                    {
                        ex.ErrorMessageBox("Error trying to open wikipedia page!");
                    }
                }
                else
                {
                    "You are not connected to internet!".Alert();
                    this.Close();
                    return;
                }
            }
        }

        //Prikaži kada se load complete-a
        private void browser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            browser1.Visibility = Visibility.Visible;
            browser1.Navigating += browser1_Navigating;

            return;
        }

        //Onemogućuje klikanje linkova
        private void browser1_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = true;

            return;
        }
    }
}
