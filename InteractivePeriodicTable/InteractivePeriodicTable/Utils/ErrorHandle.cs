using System;
using System.IO;
using System.Windows.Forms;

namespace InteractivePeriodicTable.Utils
{
    public static class ErrorHandle
    {
        public static void ErrorMessageBox(this Exception exception, string message)
        {
            if (exception != null)
            {
                using (StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\log.txt", true))
                {
                    writer.WriteLine("Pogreška: " + exception.Message + Environment.NewLine +
                                     "Datum:    " + DateTime.Now.ToString());

                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }

            MessageBox.Show( message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error );

            return;
        }
    }
}
