using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    /// <summary>
    /// Sample code for the mono open sprinkler implementation
    /// </summary>
    public class SprinklerApplication
    {
        private Task stationTask;
        private readonly CancellationTokenSource tokenSource;

        public SprinklerApplication()
        {
            tokenSource = new CancellationTokenSource();
        }

        public void Start(int numberOfStations)
        {
            if (IsRunning())
                return;

            // don't block main thread so spin up thread for job
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        for (var i = 0; i < numberOfStations; i++)
                        {
                            int station_id = i; // closure

                            stationTask = Task.Factory.StartNew(() =>
                                {
                                    Console.WriteLine("Station {0} turned on.", station_id);
                                    ScheduledJob(DateTime.Now.AddSeconds(1), station_id);
                                }, tokenSource.Token);


                            stationTask.Wait(); // operate stations in parallel
                        }
                    }
                    catch (AggregateException)
                    {
                        Console.WriteLine("Application stopped.");
                    }
                });
        }
        
        /// <summary>
        /// Returns true if there is a sprinkler job running
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return stationTask != null && stationTask.Status == TaskStatus.Running;
        }

        /// <summary>
        /// Terminates any running job
        /// </summary>
        public bool Stop()
        {
            if (!IsRunning())
                return false;

            tokenSource.Cancel();
            return true;
        }

        /// <summary>
        /// Blocking loop that shuts off station on complete
        /// </summary>
        /// <param name="runTime"></param>
        private static void ScheduledJob(DateTime? runTime, int stationId)
        {
            // if runtime is null, default to the optimal 7 minutes
            runTime = runTime ?? DateTime.Now.AddMinutes(7);

            while (true)
            {
                if (DateTime.Now > runTime)
                {
                    Console.WriteLine("\nSprinkle job for station {0} completed", stationId);
                    // todo: turn off station and continue

                    break;
                }

                Console.Write(".");
                Thread.Sleep(100);
            }
        }
    }
}