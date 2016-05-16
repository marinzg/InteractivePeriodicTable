using System;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;

namespace InteractivePeriodicTable.ExtensionMethods
{
    public static class ErrorHandle
    {
        /// <summary>
        ///     Metoda proširenja za centralizirano upravljanje pogreškama.
        ///     Dojavljuje poruku o iznimci.
        ///     Gasi program.
        /// </summary>
        /// <param name="exception">
        ///     Iznimka koja se zapisuje u log.txt
        /// </param>
        /// <param name="message">
        ///     Poruka korisniku.
        /// </param>
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

            System.Windows.Forms.MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Environment.Exit(0);

            return;
        }
    }

    public static class Styling
    {
        /// <summary>
        ///     Metoda proširenja za stiliziranje Button kontrola.
        /// </summary>
        public static void styleButton(this System.Windows.Controls.Button button)
        {
            button.Width = 250;
            button.Height = 35;
            button.Margin = new Thickness(0, 5, 0, 5);
            button.Background = Brushes.DarkTurquoise;
            button.Foreground = Brushes.MidnightBlue;
            button.FontWeight = FontWeights.Bold;

            return;
        }

        /// <summary>
        ///     Metoda proširenja za stiliziranje Label kontrola.
        /// </summary>
        public static void styleLabel(this System.Windows.Controls.Label label)
        {
            label.Foreground = Brushes.DarkTurquoise;
            label.FontSize = 18;
            label.FontWeight = FontWeights.SemiBold;

            return;
        }

        /// <summary>
        ///     Metoda proširenja za stiliziranje TextBlock kontrola.
        /// </summary>
        public static void styleTextBlock(this System.Windows.Controls.TextBlock textBlock)
        {
            textBlock.Foreground = Brushes.MidnightBlue;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.TextWrapping = TextWrapping.Wrap;

            return;
        }

        /// <summary>
        ///     Metoda proširenja za stiliziranje TextBox kontrola.
        /// </summary>
        /// <param name="textBox"></param>
        public static void styleTextBox(this System.Windows.Controls.TextBox textBox)
        {
            textBox.TextAlignment = TextAlignment.Center;
            textBox.Width = 120;
            textBox.Margin = new Thickness(0, 5, 0, 5);

            return;
        }
    }

    public static class Dialog
    {
        /// <summary>
        ///     Metoda proširenja za centraliziranu dojavu obavijesti korisniku.
        /// </summary>
        /// <param name="text">
        ///     Poruka korisniku.
        /// </param>
        public static void Notify(this string text)
        {
            if(text != null)
            {
                System.Windows.Forms.MessageBox.Show(text, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return;
        }

        /// <summary>
        ///     Metoda proširenja za centraliziranu dojavu korisniku.
        /// </summary>
        /// <param name="text">
        ///     Poruka korisniku.
        /// </param>
        public static void Alert(this string text)
        {
            if (text != null)
            {
                System.Windows.Forms.MessageBox.Show(text, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return;
        }
    }
}
