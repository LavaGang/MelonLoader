using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MelonLoader.Installer
{
    static class Program
    {
        internal static string Title = (BuildInfo.Name + " Installer for v" + BuildInfo.Version + " Open-Beta");
        private static MainForm mainForm = new MainForm();

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

                bool IsIl2CppGame = File.Exists(Path.Combine(dirpath, "GameAssembly.dll"));\
            }

            mainForm.Close();
        }
    }
}
