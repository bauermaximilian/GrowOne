/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Hardware.Sensors.Distance;
using GrowOne.Hardware.Sensors.TemperatureHumidity;
using System;
using System.Device.Adc;
using System.Device.Gpio;

namespace GrowOne.Hardware.Board
{
    public interface IBoard
    {
        bool TryGetChannelFromPin(int pin, out int channel);

        IDht22Wrapper OpenDht22(int pinEcho, int pinTrigger, GpioController? gpioController);

        IHcsr04SensorWrapper OpenHcsr04(int pinTrigger, int pinEcho);

        //GpioPin OpenGpioPin(int pin);

        //AdcChannel OpenAdcPin(int pin);

        void SetPwmPin(int pin);

        void Invoke(Action action);
    }
}
