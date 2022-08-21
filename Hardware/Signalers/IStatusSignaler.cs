/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;

namespace GrowOne.Hardware.Signalers
{
    public interface IStatusSignaler : ISignaler
    {
        void SendSignal(Status status);
    }
}
