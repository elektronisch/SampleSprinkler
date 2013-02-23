using System;
using System.Threading;
using OpenSprinkler.Manager;

namespace OpenSprinkler.Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new SprinklerDriver(5);
            app.Start(new int[] { 0, 1, 2, 3, 4 });

            app.Start(new int[] { 0, 1, 2, 3, 4 });

            Thread.Sleep(30000);

            app.Stop(); // wait 10 seconds and shut em all off

            Console.ReadLine();
        }
    }
}
