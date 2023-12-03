/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System.Net;

namespace GrowOne.Services.WebServer
{
    internal interface IRequestHandler
    {
        bool CanHandleRequest(string rawUrl, string httpMethod);

        void HandleRequest(HttpListenerContext context);
    }
}
