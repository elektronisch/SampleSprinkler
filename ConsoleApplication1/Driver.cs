using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSprinkler.Driver
{
    public class SprinklerDriver
    {
        private Task _stationTask;
        private readonly CancellationTokenSource _tokenSource;
        
        private volatile Boolean _isStarted = false;
        private readonly Boolean[] _stationBits;


        public SprinklerDriver(int numberOfStations)
        {
            this._tokenSource = new CancellationTokenSource();
            _stationBits = new Boolean[numberOfStations];
        }


        public void Start(IEnumerable<int> stations)
        {
            if (IsRunning() || _isStarted)
                return;

            _isStarted = true;

            ShiftStations(false);

            // don't block main thread so spin up thread for job
            Task.Factory.StartNew(() =>
            {
                try
                {
                    for (var i = 0; i < _stationBits.Length; i++)
                    {
                        int station_id = i; // closure

                        _stationTask = Task.Factory.StartNew(() =>
                            {
                                Console.WriteLine("Station {0} turned on.", station_id);
                                ScheduledJob(DateTime.Now.AddSeconds(2));
                            },
                            _tokenSource.Token);


                        _stationTask.Wait(); // operate stations in parallel

                        if (!_tokenSource.IsCancellationRequested)
                        {
                            Console.WriteLine("Waiting for the sprinkler value to rotate (5s)");

                            // optional: 8 second delay to allow the sprinkler valve to rotate
                            Thread.Sleep(5000);
                        }
                    }
                }
                catch (AggregateException)
                {
                    Console.WriteLine("Sprinkler job canceled.. shutting them all off.");
                }
                finally
                {
                    _isStarted = false;
                    Console.WriteLine("All completed.");
                    
                    ShiftStations(false);
                }
            });
        }


        /// <summary>
        /// Terminates any running job
        /// </summary>
        public void Stop()
        {
            if (!IsRunning())
                return;

            _tokenSource.Cancel();
        }


        /// <summary>
        /// Returns true if there is a sprinkler job running
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return _stationTask != null && _stationTask.Status == TaskStatus.Running;
        }


        /// <summary>
        /// Blocking loop that shuts off station on complete
        /// </summary>
        /// <param name="runTime"></param>
        private void ScheduledJob(DateTime? runTime)
        {
            // if runtime is null, default to the optimal 7 minutes
            runTime = runTime ?? DateTime.Now.AddMinutes(7);

            while (true)
            {
                if (DateTime.Now >= runTime)
                {
                    Console.WriteLine("\nSprinkle job completed");
                    
                    // todo: turn off station and continue

                    break;
                }

                if (_tokenSource.IsCancellationRequested)
                    break;

                Console.Write(".");

                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Shifts all of the bits from 0/1
        /// </summary>
        /// <param name="position"></param>
        private void ShiftStations(bool position = false)
        {
            for (var i = 0; i < _stationBits.Length; i++)
            {
                _stationBits[i] = position;
            }
        }
    }
}