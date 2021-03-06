﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02_webstat
{
    class Result
    {
        public string Url { get; set; }
        public int Byte { get; set; }
        public int MilliSec { get; set; }
        public double Speed { get { return Math.Round((double)Byte / MilliSec, 3); } }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Result[] T = new Result[] {
                new Result() { Url = "http://microsoft.com" },
                new Result() { Url = "http://bing.com" },
                new Result() { Url = "http://google.com" },
                new Result() { Url = "http://uni-obuda.hu" },
                new Result() { Url = "http://users.nik.uni-obuda.hu/siposm/" },
                new Result() { Url = "http://users.nik.uni-obuda.hu/prog3/" },
                new Result() { Url = "http://users.nik.uni-obuda.hu/gitstats/" },
                new Result() { Url = "http://users.nik.uni-obuda.hu/sztf2/" },
            };


            Thread[] threads = new Thread[T.Length];
            for (int i = 0; i < T.Length; i++)
            {
                threads[i] = new Thread(Measure);
                threads[i].Name = T[i].Url.Replace("http://", "");
                threads[i].Start(T[i]);
            }

            // info 
            for (int i = 0; i < T.Length; i++)
                WriteOutName(threads[i].Name);

            // sync
            for (int i = 0; i < T.Length; i++)
                threads[i].Join();

            Console.WriteLine("\n\n");
            // returned info
            foreach (var item in T.OrderBy(x => x.Speed))
                WriteOutResult(item.Url, item.Speed);
        }

        static void Measure(object o)
        {
            Result e = o as Result;
            Stopwatch sw = new Stopwatch();
            int avgLen = 0;
            int avgTim = 0;
            int iterations = 10;

            for (int i = 0; i < iterations; i++)
            {
                sw.Start();
                avgLen += (new System.Net.WebClient()).DownloadString(e.Url).Length;
                sw.Stop();
                avgTim += (int)sw.ElapsedMilliseconds;
                sw.Reset();

                Thread.Sleep(500); // DOS...
            }

            e.Byte = avgLen / iterations;
            e.MilliSec = avgTim / iterations;
            
        }

        static void WriteOutName(string input)
        {
            Console.Write($"> Waiting for ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{input}");
            Console.ResetColor();
            Console.Write($" test...\n");
        }

        static void WriteOutResult(string url, double speed)
        {
            Console.Write($"> RESULT:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{url} ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{speed} kB/s\n");
            Console.ResetColor();
        }
    }
}
