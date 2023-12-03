/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class StatusDescriptorDto
    {
        public int code { get; set; }

        public string? message { get; set; }

        public StatusDescriptorDto()
        {
        }

        public StatusDescriptorDto(int code, string? message)
        {
            this.code = code;
            this.message = message;
        }
    }
}
