using System;
using System.IO;
using System.Text;

namespace HotelSystem
{
    public static class Log
    {
        static private FileStream fs = new FileStream("log.log", FileMode.Create, FileAccess.Write);
        static private StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
        static public void print(string s)
        {
            try
            {
                DateTime dt = DateTime.Now;
                sw.WriteLine(dt.ToString() + "  " + s);
                sw.Flush();
            }
            catch (Exception)
            {

            }
            
        }
    }
}
