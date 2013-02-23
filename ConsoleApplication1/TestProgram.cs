using System;
using System.Threading;

namespace OpenSprinkler.Driver
{
    class TestProgram
    {
        static void Main(string[] args)
        {
            var app = new SprinklerDriver(5);
            app.Start(new int[] { 0, 1, 2, 3, 4 });

            Thread.Sleep(500);

            app.Stop(); // wait 3 seconds and shut em all off

            Console.ReadLine();
        }
    }
}
