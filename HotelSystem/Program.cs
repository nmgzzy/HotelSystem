using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelSystem
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin());
            if (Status.LoginSuccess)
            {
                Application.Run(new FormSystem());
            }
        }
    }
    
    static class Status
    {
        public static bool LoginSuccess = false;
        public static string manager;
        public enum Epage
        {
            people,
            room,
            admin,
            info
        }
        public static Epage page = Epage.people;
    }
}
