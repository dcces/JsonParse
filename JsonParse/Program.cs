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
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string connstr = "server=127.0.0.1;user id=sa;password=1234;database=BK";

            FileStream fs = new FileStream(@"C:\Users\ZZJ\Desktop\data\banks.json", FileMode.Open, FileAccess.Read);
            //FileStream fs = new FileStream(@"C:\Users\ZZJ\Desktop\data\areas.json", FileMode.Open, FileAccess.Read);
            //FileStream fs = new FileStream(@"C:\Users\ZZJ\Desktop\data\streets.json", FileMode.Open, FileAccess.Read);
            //FileStream fs = new FileStream(@"C:\Users\ZZJ\Desktop\data\provinces.json", FileMode.Open, FileAccess.Read);
            //FileStream fs = new FileStream(@"C:\Users\ZZJ\Desktop\Cards.json", FileMode.Open, FileAccess.Read);
            #region MyRegion
            //StreamReader sr = new StreamReader(fs);
            //string body = sr.ReadToEnd();
            ////var newbody = body.Replace("name", "nam");
            //var newbody = body.Replace("city_id", "city_i");
            //var sum = (body.Length - newbody.Length);
            //Console.WriteLine(sum);
            //Console.WriteLine();
            //Console.WriteLine();
            //fs.Seek(0, SeekOrigin.Begin); 
            #endregion
            //Parse p = new Parse(fs, connstr, "pro", 40960, Encoding.UTF8);
            //Parse p = new Parse(fs, connstr, "st", 40960, Encoding.UTF8);

            //Parse p = new Parse(fs, connstr, "cards", 40960, Encoding.UTF8);
            //Parse p = new Parse(fs, connstr, "Area", 512, Encoding.UTF8);
            Parse p = new Parse(fs, connstr, "Banks", 40961, Encoding.UTF8);
            //Parse p = new Parse(@"C:\Users\ZZJ\Desktop\data\areas.txt", "Area");
            p.Start();
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine("完成");
            Console.Read();





        }


    }
}
