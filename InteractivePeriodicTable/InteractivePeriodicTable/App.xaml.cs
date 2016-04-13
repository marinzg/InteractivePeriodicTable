using System.Windows;
using InteractivePeriodicTable.Utils;

namespace InteractivePeriodicTable
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Update up = new Update();
            up.updateFacts();
            up.updateQuiz();

            MainWindow mainView = new MainWindow();
            mainView.Show();
        }
    }
}
