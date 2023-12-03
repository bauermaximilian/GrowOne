/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Resources;
using System;
using System.Net;

namespace GrowOne.Services.WebServer
{
    internal class FileRequestHandler : IRequestHandler
    {
        private readonly object lockObject = new();

        public FileRequestHandler()
        {
        }

        public bool CanHandleRequest(string rawUrl, string httpMethod)
        {
            string url = WebServerUtils.GetTrimmedLowercaseUrl(rawUrl);
            return httpMethod == "GET" &&
                (url.Length == 0 || WebResourceResolver.TryResolveResourceName(url, out _));
        }

        public void HandleRequest(HttpListenerContext context)
        {
            lock (lockObject)
            {
                string url = WebServerUtils.GetTrimmedLowercaseUrl(context);
                if (url.Length == 0)
                {
                    url = "index.html";
                }

                if (WebResourceResolver.TryResolveResourceName(url, out var resourceName))
                {
                    context.Response.ContentType = WebServerUtils.GetContentType(url);
                    context.Response.StatusCode = 200;
                    WebServerUtils.CopyResourceToStream(resourceName,
                        context.Response.OutputStream);
                    context.Response.Close();
                }
                else
                {
                    throw new InvalidOperationException("The request resource doesn't exist.");
                }
            }
        }
    }
}
