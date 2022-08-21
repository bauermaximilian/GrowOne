/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;

namespace GrowOne.Services
{
    public interface IHostedService : IService
    {
        bool IsRunning { get; }

        event EventHandler StateChanged;

        void Start();

        void Stop();
    }
}
