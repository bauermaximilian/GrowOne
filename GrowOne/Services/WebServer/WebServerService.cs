/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Services.DeviceManager;
using nanoFramework.WebServer;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GrowOne.Services.WebServer
{
    internal class WebServerService : BackgroundService
    {
        private readonly DeviceManagerService deviceManager;
        private readonly IRequestHandler[] requestHandlers;

        public TimeSpan WaitForNetworkTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public event EventHandler? ServerStarted;

        public WebServerService(DeviceManagerService deviceManager, params IRequestHandler[] requestHandlers)
        {
            this.deviceManager = deviceManager;
            this.requestHandlers = requestHandlers;
        }

        private static string? WaitForNetwork(CancellationToken token, TimeSpan timeout)
        {
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];
            DateTime timeoutTime = DateTime.UtcNow + timeout;
            while (!token.IsCancellationRequested && DateTime.UtcNow < timeoutTime)
            {
                if (networkInterface.IPv4Address != null &&
                    networkInterface.IPv4Address.Length > 0 &&
                    networkInterface.IPv4Address[0] != '0')
                {
                    return networkInterface.IPv4Address;
                }
                else
                {
                    token.WaitHandle.WaitOne(500, false);
                }
            }
            return null;
        }

        protected override void Run(CancellationToken token)
        {
            string? address = null;
            bool waitForNetwork = WaitForNetworkTimeout > TimeSpan.Zero;
            if (waitForNetwork)
            {
                Log.Debug("Waiting for Wifi network...");

                address = WaitForNetwork(token, WaitForNetworkTimeout);
            }

            if (address != null || !waitForNetwork)
            {
                if (address != null)
                {
                    Log.Debug(
                        $"Connection to Wifi network established with IP \"{address}\".");
                }

                Log.Debug("Starting webserver...");

                using var server = new nanoFramework.WebServer.WebServer(80, HttpProtocol.Http);
                server.CommandReceived += Server_CommandReceived;

                server.Start();

                // Wait for the server to start before testing the connection.
                Thread.Sleep(1000);

                if (TestWebServerConnection())
                {
                    Log.Debug("Web server started and available!");
                    ServerStarted?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Log.Error("Web server couldn't be started: self-test failed.");
                    Stop();
                }

                token.WaitHandle.WaitOne();

                server.CommandReceived -= Server_CommandReceived;
                server.Stop();
            }
            else
            {
                Log.Error("Web server couldn't be started: Wifi unavailable.");
            }
        }

        public static bool TestWebServerConnection()
        {
            try
            {
                using Socket socket = new(
                    AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    SendTimeout = 1000,
                    ReceiveTimeout = 1000
                };

                socket.Connect(new IPEndPoint(IPAddress.GetDefaultLocalAddress(), 80));
                socket.Send(Encoding.UTF8.GetBytes(
    @"HEAD / HTTP/1.1
Host: 127.0.0.1:80
User-Agent: test/1.0.0
Accept: */*

"));

                byte[] buffer = new byte[128];
                int totalReceived = 0;
                int received = 0;
                do
                {
                    received = socket.Receive(buffer);
                    totalReceived += received;
                } while (received > 0);
                socket.Close();

                return totalReceived > 0;
            }
            catch
            {
                return false;
            }
        }

        private void Server_CommandReceived(object obj, WebServerEventArgs e)
        {
            try
            {
                foreach (var requestHandler in requestHandlers)
                {
                    if (requestHandler.CanHandleRequest(e.Context.Request.RawUrl,
                        e.Context.Request.HttpMethod))
                    {
                        requestHandler.HandleRequest(e.Context);
                        return;
                    }
                }

                WebServerUtils.SendErrorResponse(e.Context.Response, 404,
                    "The requested resource wasn't found.");
            }
            catch (Exception exc)
            {
                Log.Warning("Error while processing request.", exc);
                try
                {
                    WebServerUtils.SendErrorResponse(e.Context.Response, 500,
                        "Internal server error.");
                }
                catch (Exception innerExc)
                {
                    Log.Warning("Error while sending error response.", innerExc);
                }
            }
        }
    }
}
