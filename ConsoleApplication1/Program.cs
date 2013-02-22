using System;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new SprinklerApplication();
            app.Start(5);

            Thread.Sleep(1000);
            app.Stop(); // wait 1 seconds and shut em all off

            Console.WriteLine("Sprinkler application complete");
            Console.ReadLine();
        }
    }
}
