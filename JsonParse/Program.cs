using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParse
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = new FileStream("../../areas.json", FileMode.Open, FileAccess.Read);
            
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string connstr = "server=127.0.0.1;user id=sa;password=1234;database=BK";
            Parse p = new Parse(fs, connstr, "Area", 1024*10, Encoding.UTF8);
            p.Start();
            sw.Stop();
            Console.WriteLine("完成，耗时{0}", sw.Elapsed);
            Console.Read();





        }


    }
}
