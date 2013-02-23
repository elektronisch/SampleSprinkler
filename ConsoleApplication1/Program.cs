using System;
using System.Threading;
using OpenSprinkler.Manager;

namespace OpenSprinkler.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new SprinklerDriver(5);
            app.Start(new[] { 0, 1, 2, 3, 4 });

            //Thread.Sleep(10000);

            //app.Stop(); // wait 10 seconds and shut em all off

            Console.ReadLine();
        }
    }
}
