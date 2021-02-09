using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleOutPut
{
    class Program
    {
        static void Main(string[] args)
        {
            TextWriter u = Console.Out;
            FileStream fs = new FileStream(Environment.CurrentDirectory+"\\out.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Console.SetOut(new StreamWriter(fs));
            Thread task = new Thread(async ()=> {
                Stopwatch stopWatch = Stopwatch.StartNew();

                do
                {
                    Thread.Sleep(400);
                    Console.WriteLine(stopWatch.Elapsed);
                    await fs.FlushAsync();
                } while (true);
            });
            task.Start();

        }
    }
}
