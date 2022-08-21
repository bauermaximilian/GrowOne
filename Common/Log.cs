/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;

namespace GrowOne.Common
{
    public delegate void LogEventHandler(DateTime timestamp, string level, string message);

    public static class Log
    {
        public static event LogEventHandler? MessageLogged;

        public static void Debug(string message)
        {
            LogMessage("DBG", message);
        }

        public static void Info(string message)
        {
            LogMessage("INF", message);
        }

        public static void Warning(string message)
        {
            LogMessage("WRN", message);
        }

        public static void Warning(string message, Exception exception)
        {
            LogMessage("WRN", $"{message}\n{exception}");
        }

        public static void Error(string message)
        {
            LogMessage("ERR", $"{message}");
        }

        public static void Error(string message, Exception exception)
        {
            LogMessage("ERR", $"{message}\n{exception}");
        }

        private static void LogMessage(string level, string message)
        {
            MessageLogged?.Invoke(DateTime.UtcNow, level, message);
        }
    }
}
