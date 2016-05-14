using System.Windows;
using InteractivePeriodicTable.Utils;
using System.Threading;
using System.ComponentModel;
using System.Data.SqlClient;

using InteractivePeriodicTable.ExtensionMethods;
using System;

namespace InteractivePeriodicTable
{
    public partial class App : Application
    {
        //ProgressBarWindow progBar;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //try
            //{
            //    BackgroundWorker workerUpdateQuiz = new BackgroundWorker();
            //    workerUpdateQuiz.WorkerReportsProgress = true;
            //    workerUpdateQuiz.WorkerSupportsCancellation = true;
            //    workerUpdateQuiz.DoWork += worker_DoWork;
            //    workerUpdateQuiz.ProgressChanged += worker_ProgressChanged;

            //    workerUpdateQuiz.RunWorkerAsync();


            MainWindow mainView = new MainWindow();
            mainView.Show();
            //    progBar = new ProgressBarWindow();


            //    progBar.ShowDialog();
            //    //TODO @ petar: dovršiti logiku iza ovoga
            //    Thread.Sleep(700);
            //    progBar.worker.CancelAsync();
            //    workerUpdateQuiz.CancelAsync();
            //    mainView.Show();
            //}
            //catch(Exception ex)
            //{
            //    ex.ErrorMessageBox("Pogreška prilikom paljenja aplikacije !");
            //}
        }

        //private void worker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    (sender as BackgroundWorker).ReportProgress(0);
        //    Update up = new Update();
        //    try {
        //        (sender as BackgroundWorker).ReportProgress(25);
        //        up.updateFacts();
        //        (sender as BackgroundWorker).ReportProgress(50);
        //        up.updateQuiz();
        //        (sender as BackgroundWorker).ReportProgress(100);
        //    }
        //    catch(SqlException)
        //    {
        //        MessageBox.Show("Error connecting to database");
        //        App.Current.Shutdown();
        //    }
        //}
        //private void worker_ProgressChanged (object sender, ProgressChangedEventArgs e)
        //{
        //    if (e.ProgressPercentage == 100)
        //    {
        //        Dispatcher.Invoke(() => {
        //            progBar.textBlock.Text = "Completed!";
        //            progBar.loadingFinished = true;
        //        });
        //    }
        //    else
        //    {
        //        Dispatcher.Invoke(() => {
        //            progBar.textBlock.Text += ".";
        //        });
        //    }
        //    Thread.Sleep(100);
        //}
    }
}
