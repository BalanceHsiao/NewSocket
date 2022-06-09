using MessageHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MessageHelper.Model.MessageModel;

namespace SocketLib.SQLDB.BLL.Helper
{
    class ConvertHelper
    {
        private static object myObj = new object();

        public static Dictionary<string, string> ConvertToDictionary(string commandText)
        {
            lock (myObj)
            {

                try
                {
                    var myDictionary = new Dictionary<string, string>();

                    string[] afterConvert = commandText.Split(',');

                    foreach (var datas in afterConvert)
                    {
                        var data = datas.Split(':');
                        myDictionary.Add(data[0], data[1]);
                    }

                    return myDictionary;
                }
                catch (Exception ex)
                {
                    ShowMessage.LogMessage(ex.StackTrace, MessageLevel.Error, SocketMessageType.Server);
                    return null;
                }
            }
        }

        public static string ChangeCommandText(Dictionary<string, string> dicContent)
        {
            lock (myObj)
            {
                string commandText = string.Empty;
                foreach (KeyValuePair<string, string> keyValue in dicContent)
                {
                    if (keyValue.Key != "TEID")
                    {
                        commandText += $"{keyValue.Key}:{keyValue.Value},";
                    }
                }

                var dataLength = commandText.Length;
                return commandText.Substring(0, dataLength - 1);
            }
        }

        #region [===全域公用方法 -- 抓取任務時間含6位數的亂數===]
        /// <summary>
        /// 全域公用方法 -- 抓取任務時間含6位數的亂數
        /// </summary>
        /// <param name="bolDateTime">true(包含亂數)</param>
        /// <returns></returns>
        public static string GetTaskDateTimeIncludeRandom(bool bolDateTime)
        {
            var rdn = new Random();
            var int32Rdn = rdn.Next(100, 980); //刻意加避免撞到
            if (bolDateTime)
                return DateTime.Now.ToString("yyyyMMddHHmmssfff") + int32Rdn;
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        #endregion


    }
}
