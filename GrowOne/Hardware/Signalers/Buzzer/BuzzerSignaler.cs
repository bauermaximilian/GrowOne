/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Hardware.Board;
using System.Device.Pwm;
using System.Threading;

namespace GrowOne.Hardware.Signalers.Buzzer
{
    internal class BuzzerSignaler : IStatusSignaler
    {
        private readonly PwmChannel buzzerChannel;

        public BuzzerSignaler(IBoard board, int pin)
        {
            board.SetPwmPin(pin);
            buzzerChannel = PwmChannel.CreateFromPin(pin);
            buzzerChannel.Stop();
        }

        public void SendSignal(Status status)
        {
            switch (status)
            {
                case Status.Default:
                    PlayTone(Tone.C4, 200, 50, 2);
                    break;
                case Status.Ping:
                    PlayTone(Tone.C3, 200, 50, 1);
                    break;
                case Status.Disabled:
                    PlayTone(Tone.C4, 100, 50);
                    PlayTone(Tone.C1, 700, 50);
                    break;
                case Status.Enabled:
                    PlayTone(Tone.C1, 100, 50);
                    PlayTone(Tone.C4, 700, 50);
                    break;
                case Status.Failure:
                    PlayTone(Tone.A4, 100, 25);
                    PlayTone(Tone.A3, 100, 25);
                    PlayTone(Tone.A2, 100, 25);
                    PlayTone(Tone.A1, 700, 50);
                    break;
                case Status.Success:
                    PlayTone(Tone.C1, 100, 25);
                    PlayTone(Tone.C2, 100, 25);
                    PlayTone(Tone.C3, 100, 25);
                    PlayTone(Tone.C4, 700, 50);
                    break;
            }
        }

        public void SendSignal()
        {
            SendSignal(Status.Default);
        }

        private void PlayTone(Tone tone, int durationMs, int pauseMs = 0, 
            int repetitions = 1)
        {
            for (int i = 0; i < repetitions; i++)
            {
                buzzerChannel.Frequency = (int)tone.ToFrequency();
                buzzerChannel.Start();
                Thread.Sleep(durationMs);
                buzzerChannel.Stop();
                if (pauseMs > 0) Thread.Sleep(pauseMs);
            }
        }

        public void Dispose()
        {
            buzzerChannel.Dispose();
        }
    }
}
