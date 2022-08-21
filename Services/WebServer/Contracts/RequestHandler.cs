/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using System;
using System.Collections;
using System.Net;
using System.Reflection;

namespace GrowOne.Services.WebServer.Contracts
{
    internal delegate void RouteHandler(HttpListenerContext context);

    internal class RequestRouteAttribute : Attribute
    {
        public string Prefix { get; }

        public string HttpMethod { get; }

        public RequestRouteAttribute(string prefix, string httpMethod)
        {
            Prefix = prefix;
            HttpMethod = httpMethod;
        }
    }

    internal abstract class RequestHandler : IRequestHandler
    {
        private readonly ArrayList requestHandlers = new();

        protected RequestHandler()
        {
            MethodInfo[] methods = GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods)
            {
                ParameterInfo[] methodParameters = method.GetParameters();
                if (method.ReturnType != typeof(void) ||
                    methodParameters.Length != 1 ||
                    methodParameters[0].ParameterType != typeof(HttpListenerContext))
                    continue;

                RequestRouteAttribute? requestHandlerRoute = null;
                var attributes = method.GetCustomAttributes(true);
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i] is RequestRouteAttribute requestHandlerRouteCandidate)
                    {
                        requestHandlerRoute = requestHandlerRouteCandidate;
                        break;
                    }
                }
                if (requestHandlerRoute == null) continue;

                requestHandlers.Add(
                    new RequestRouteHandler(requestHandlerRoute, method, this));
            }
        }

        public virtual bool CanHandleRequest(string rawUrl, string httpMethod)
        {
            string url = WebServerUtils.GetTrimmedLowercaseUrl(rawUrl);
            return TryGetRouteHandler(url, httpMethod, out _);
        }

        public virtual void HandleRequest(HttpListenerContext context)
        {
            string url = WebServerUtils.GetTrimmedLowercaseUrl(context);
            if (TryGetRouteHandler(url, context.Request.HttpMethod, out var handler))
            {
                handler!.Invoke(context);
            }
            else
            {
                throw new InvalidOperationException("The requested route couldn't be found.");
            }
        }

        protected bool TryGetRouteHandler(string? url, string? httpMethod,
            out RouteHandler? handler)
        {
            RequestRouteHandler? routeHandler = null;

            if (url != null)
            {
                foreach (RequestRouteHandler handlerCandidate in requestHandlers)
                {
                    if (url.StartsWith(handlerCandidate.Route.Prefix) &&
                        (httpMethod == null || handlerCandidate.Route.HttpMethod == httpMethod))
                    {
                        if (routeHandler == null ||
                            routeHandler.Route.Prefix.Length < handlerCandidate.Route.Prefix.Length)
                        {
                            routeHandler = handlerCandidate;
                        }
                    }
                }
            }

            handler = routeHandler != null ? routeHandler.Invoke : null;
            return handler != null;
        }

        private class RequestRouteHandler
        {
            public RequestRouteAttribute Route { get; set; }
            public MethodInfo HandlerMethod { get; set; }
            public RequestHandler Self { get; set; }

            public RequestRouteHandler(RequestRouteAttribute route, MethodInfo method,
                RequestHandler self)
            {
                Route = route;
                HandlerMethod = method;
                Self = self;
            }

            public void Invoke(HttpListenerContext context)
            {
                HandlerMethod.Invoke(Self, new object[] { context });
            }
        }
    }
}
