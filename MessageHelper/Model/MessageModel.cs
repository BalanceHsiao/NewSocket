using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageHelper.Model
{
    public class MessageModel
    {
        /// <summary>
        /// Message的級別
        /// </summary>
        public enum MessageLevel
        {
            /// <summary>
            /// 成功
            /// </summary>
            Normal = 0,

            /// <summary>
            /// 警告
            /// </summary>
            Warning = 1,

            /// <summary>
            /// 失敗
            /// </summary>
            Error = 2
        }


        /// <summary>
        /// 回傳何種Stocket訊息
        /// </summary>
        public enum SocketMessageType
        {
            /// <summary>
            /// 客戶端的訊息
            /// </summary>
            Client = 0,

            /// <summary>
            /// 服務端的訊息
            /// </summary>
            Server = 1,

            /// <summary>
            /// 頁面訊息
            /// </summary>
            From = 2,

            /// <summary>
            /// 系統訊息
            /// </summary>
            Systems = 99
        }



    }
}
