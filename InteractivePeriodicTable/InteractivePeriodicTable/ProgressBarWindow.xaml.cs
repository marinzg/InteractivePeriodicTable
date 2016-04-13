using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace InteractivePeriodicTable
{

    public partial class ProgressBarWindow : Window
    {
        public bool loadingFinished = false;

        public ProgressBarWindow()
        {
            InitializeComponent();
            Window_ContentRendered();
        }

        private void Window_ContentRendered()
        {
            public BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();

        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                if (loadingFinished == true)
                {
                    i = 99;
                    (sender as BackgroundWorker).ReportProgress(100);
                }
                else (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(10);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (loadingFinished == true)
            {
                Dispatcher.Invoke(() =>
                {
                    Thread.Sleep(100);
                    this.Close();                
                });
            }
            Dispatcher.Invoke(() => {
                    pbStatus.Value = e.ProgressPercentage;
            });
        }

    }
}
