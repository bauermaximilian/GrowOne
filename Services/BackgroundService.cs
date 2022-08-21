/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;
using System.Diagnostics;
using System.Threading;

namespace GrowOne.Services
{
    public abstract class BackgroundService : IHostedService
    {
        public bool IsRunning
        {
            get => isRunning;
            private set
            {
                isRunning = value;
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool isRunning;

        public event EventHandler? StateChanged;

        private CancellationTokenSource? cts;
        private Thread? serviceThread;

        public void Start()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            AutoResetEvent threadStartedEvent = new(false);
            serviceThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    IsRunning = true;
                    threadStartedEvent.Set();
                    Run(cts.Token);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine($"The service \"{GetType().Name}\" failed unexpectedly: " +
                        $"{exc.Message}");
                }
                finally
                {
                    IsRunning = false;
                }
            }));
            serviceThread.Start();
            threadStartedEvent.WaitOne();
        }

        protected abstract void Run(CancellationToken token);

        public void Stop()
        {
            cts?.Cancel();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
