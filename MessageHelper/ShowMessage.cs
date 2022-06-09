using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static MessageHelper.Model.MessageModel;

namespace MessageHelper
{
    public static class ShowMessage
    {
        public delegate void DInMessage(string sMessage, MessageLevel sMessageLevel, SocketMessageType sSocketType);

        public static event DInMessage OnMessage;
        private static string DateFormat;

        

        public static void LogMessage(string sMessage, MessageLevel sMessageLevel = MessageLevel.Normal, SocketMessageType sSocketType = SocketMessageType.Systems)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                //string saaMessage = $"【{GetDatetime()}】 {sMessage}";
                string saaMessage = $"{sMessage}";
                if (sMessageLevel == MessageLevel.Normal)
                {
                    logger.Info(saaMessage);
                    OnMessage?.Invoke(saaMessage, MessageLevel.Normal, sSocketType);
                }
                else if (sMessageLevel == MessageLevel.Error)
                {
                    logger.Error(saaMessage);
                    OnMessage?.Invoke($"{saaMessage}", MessageLevel.Error, sSocketType);
                }
                else
                {
                    logger.Warn(saaMessage);
                    OnMessage?.Invoke($"{saaMessage}", MessageLevel.Warning, sSocketType);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
                OnMessage?.Invoke($"EXCEPTION-{e.StackTrace}", MessageLevel.Error, sSocketType);
            }
        }

        public static void LogMessage(Dictionary<string, string> message, bool indent, MessageLevel sMessageLevel = MessageLevel.Normal, SocketMessageType sSocketType = SocketMessageType.Server)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                var msg = "";
                if (indent)
                {
                    msg = JsonConvert.SerializeObject(message, Formatting.Indented);
                }
                else
                {
                    msg = JsonConvert.SerializeObject(message);
                }

                string saaMessage = $"【{GetDatetime()}】 {msg}";

                if (sMessageLevel == MessageLevel.Normal)
                {
                    logger.Info(saaMessage);
                    OnMessage?.Invoke(saaMessage, MessageLevel.Normal, sSocketType);
                }
                else if (sMessageLevel == MessageLevel.Error)
                {
                    logger.Error(saaMessage);
                    OnMessage?.Invoke($"{saaMessage}", MessageLevel.Error, sSocketType);
                }
                else
                {
                    logger.Warn(saaMessage);
                    OnMessage?.Invoke($"{saaMessage}", MessageLevel.Warning, sSocketType);
                }
            }
            catch (Exception e)
            {
                //OnMessage?.Invoke($"({saaEquipmentImp}, {sSocketType})EXCEPTION-{e.StackTrace}", MessageLevel.Error, sSocketType);
                logger.Error(e.StackTrace);
                OnMessage?.Invoke($"EXCEPTION-{e.StackTrace}", MessageLevel.Error, sSocketType);
            }
        }

        private static string GetDatetime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        #region [Log記錄]
        private static void CreateLogger()
        {
            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget
            {
                FileName = "${basedir}/logs/${shortdate}.log",
                Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} [${uppercase:${level}}] ${message}",
            };
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
            LogManager.Configuration = config;

        }
        #endregion [Log記錄]

    }

}
