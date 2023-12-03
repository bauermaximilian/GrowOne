/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Core
{
    public static class ToneExtensions
    {
        public static double ToFrequency(this Tone tone)
        {
            return (int)tone * 0.01;
        }
    }
}
