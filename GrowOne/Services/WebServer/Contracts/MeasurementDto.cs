/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class MeasurementDto
    {
        public string? label { get; set; }

        public string? currentValue { get; set; }

        public string? statistics { get; set; }
    }
}
