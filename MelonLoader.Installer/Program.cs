using System;
using System.Windows.Forms;
using System.Net;

namespace MelonLoader.Installer
{
    static class Program
    {
        internal static string Title = "MelonLoader Installer";

        [STAThread]
        static void Main()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = new MainForm();
            Application.EnableVisualStyles();

            try
            {
                //mainForm.comboBox1.Items.Clear();

                // Get Version List from GitHub

                if (mainForm.comboBox1.Items.Count <= 0)
                    throw new Exception("Version List is Empty!");
                mainForm.Show();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Get Version List; copy this dialog (press Control+C) to #melonloader-support on discord\n" + ex, Title);
                Application.Exit();
            }
        }
    }
}