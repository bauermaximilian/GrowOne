/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using GrowOne.Services;
using nanoFramework.Runtime.Native;
using System;
using System.Diagnostics;
using System.Threading;

namespace GrowOne
{
    public class Program
    {
        /// <summary>
        /// Defines whether the application settings, stored as "GrowOne.cfg" on the internal 
        /// memory, should be reset at every start of the application (<c>true</c>) or not and the
        /// settings should be loaded normally (<c>false</c>, default).
        /// Can be used to reset settings that somehow got corrupted and prevent the application 
        /// from starting normally.
        /// </summary>
        public const bool ResetSettingsOnInitialisation = false;

        /// <summary>
        /// Defines whether setting a relais actuator to <c>true</c> should result into the
        /// associated pin to be set to <see cref="System.Device.Gpio.PinValue.Low"/> (<c>true</c>)
        /// or to <see cref="System.Device.Gpio.PinValue.High"/> (<c>false</c>).
        /// Depending on the component used for the relais actuator, this value may have to be 
        /// adjusted so that the actuator doesn't behave the opposite from what it should.
        /// </summary>
        public const bool InvertRelaisOutput = true;

        public static void Main()
        {
            Log.MessageLogged += Log_MessageLogged;
            Log.Info("Program execution started.");

            AutoResetEvent applicationStoppedEvent = new(false);
            ApplicationService applicationService = new ApplicationService();
            applicationService.StateChanged += delegate (object s, EventArgs e)
            {
                if (!applicationService.IsRunning) applicationStoppedEvent.Set();
            };
            applicationService.ResetSettingsOnInitialisation = ResetSettingsOnInitialisation;
            applicationService.Start();
            applicationStoppedEvent.WaitOne();

            Log.Info("Program execution ended.");
            Log.MessageLogged -= Log_MessageLogged;
            Power.RebootDevice(1000);
        }

        private static void Log_MessageLogged(DateTime timestamp, string level, string message)
        {
#if DEBUG
            Debug.WriteLine($"{timestamp.ToString("HH:mm:ss")} {level}: {message}");
#endif
        }
    }
}
