/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Resources;
using nanoFramework.Json;
using nanoFramework.Runtime.Native;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace GrowOne.Services.WebServer
{
    internal static class WebServerUtils
    {
        public const string HttpMethodGet = "GET";
        public const string HttpMethodPost = "POST";
        public const string HttpMethodPut = "PUT";
        public const string HttpMethodDelete = "DELETE";
        public const string MimeTypeHtml = "text/html";
        public const string MimeTypeJson = "application/json";

        public static string GetTrimmedLowercaseUrl(HttpListenerContext context) =>
            GetTrimmedLowercaseUrl(context.Request.RawUrl);

        public static string GetTrimmedLowercaseUrl(string rawUrl) => 
            rawUrl.ToLower().TrimStart('/', ' ').TrimEnd();

        public static void SendErrorResponse(HttpListenerResponse targetResponse, int statusCode,
            string description)
        {
            SendStringResponse(targetResponse, statusCode,
                BuildErrorPage("Error", "&#xFF1E;&#xFE4F;&#xFF1C;", description));
        }

        public static void SendJsonResponse(HttpListenerResponse targetResponse, 
            int status, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            SendStringResponse(targetResponse, status, json, MimeTypeJson);
        }

        public static void SendEmptyResponse(HttpListenerResponse targetResponse, 
            int statusCode = 204)
        {
            targetResponse.StatusCode = statusCode;
            targetResponse.Close();
        }

        public static void SendStringResponse(HttpListenerResponse targetResponse, int statusCode,
            string content, string contentType = MimeTypeHtml, bool closeAfterwards = true)
        {
            targetResponse.ContentType = contentType;
            targetResponse.StatusCode = statusCode;
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            targetResponse.ContentLength64 = contentBytes.Length;
            targetResponse.OutputStream.Write(contentBytes, 0, contentBytes.Length);

            if (closeAfterwards)
            {
                targetResponse.Close();
            }
        }

        public static string GetContentType(string uri)
        {
            int fileExtensionSeparatorIndex = uri.LastIndexOf('.') + 1;
            if (fileExtensionSeparatorIndex > 0)
            {
                string fileExtension = uri.Substring(fileExtensionSeparatorIndex).ToLower();
                return fileExtension switch
                {
                    "txt" => "text/plain",
                    "css" => "text/css",
                    "js" => "text/javascript",
                    "htm" => MimeTypeHtml,
                    "html" => MimeTypeHtml,
                    "ico" => "image/vnd.microsoft.icon",
                    "svg" => "image/svg+xml",
                    "json" => MimeTypeJson,
                    "png" => "image/png",
                    _ => "application/octet-stream",
                };
            }
            else return "application/octet-stream";
        }

        public static string BuildErrorPage(string title, string heading, string subtext)
        {
            return $"<!DOCTYPE html><html><head><title>{title}</title><meta name='viewport' " +
                "content='width=device-width,initial-scale=1,maximum-scale=1," +
                "user-scalable=no'></head><body style=\"background-color:#11191F; " +
                "color: #EDF0F3; margin: 0; height: 100vh; display: flex; flex-direction: " +
                "column; justify-content: center; text-align: center; font-family: system-ui," +
                "-apple-system,'Segoe UI','Roboto','Ubuntu','Cantarell','Noto Sans'," +
                $"sans-serif\"><h1 style='font-size: 5em'>{heading}</h1>" +
                $"<sub>{subtext}</sub></body></html>";
        }

        public static void CopyResourceToStream(WebResources.BinaryResources resourceName,
            Stream targetStream)
        {
            byte[] buffer;
            int offset = 0;
            const int count = 1024;
            try
            {
                do
                {
                    buffer = (byte[])ResourceUtility.GetObject(
                        WebResources.ResourceManager, resourceName, offset, count);
                    targetStream.Write(buffer, 0, buffer.Length);
                    offset += buffer.Length;
                } while (buffer.Length == count);
            }
            catch (Exception)
            {
                // If the size of the requested resource is divisible by the specified count,
                // an exception will be thrown after getting the last chunk (as no way to retrieve
                // the resource file size was found while writing this).
                // So, only if the offset is 0 (no data has been transferred), treat an exception
                // as an error.
                if (offset == 0) throw;
            }
        }
    }
}
