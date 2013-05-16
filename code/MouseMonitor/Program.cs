using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace MouseMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception e)
            {
                string error = e.Message;
                string logFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mouseMonitor";
                using (FileStream stream = new FileStream(logFolder + "error.log", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    byte[] errorbytes = Encoding.UTF8.GetBytes(error);
                    stream.Write(errorbytes, 0, errorbytes.Length);
                }
                throw e;
            }
        }
    }
}
