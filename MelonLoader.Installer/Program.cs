using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MelonLoader.Installer
{
    static class Program
    {
        internal static string Title = (BuildInfo.Name + " Installer for v" + BuildInfo.Version + " Open-Beta");
        internal static MainForm mainForm = new MainForm();

        [STAThread]
        static void Main()
        {
            new Thread(() => { mainForm.ShowDialog(); }).Start();
            Thread.Sleep(1000);
            string filePath = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Unity Game (*.exe)|*.exe";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    filePath = openFileDialog.FileName;
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                string dirpath = Path.GetDirectoryName(filePath);
                PackageManager.Run(dirpath, File.Exists(Path.Combine(dirpath, "GameAssembly.dll")));
                mainForm.Close();
                DialogResult dlg = MessageBox.Show("Installation Successful!", Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            Application.Exit();
        }
    }
}
