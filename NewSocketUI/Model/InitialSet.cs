using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSocketUI.Model
{
    /// <summary>
    /// 初始頁面
    /// </summary>
    public class FromSet
    {
        /// <summary>
        /// 頁面說明
        /// </summary>
        public string FromIllustrate { get; set; }

        /// <summary>
        /// ServerLog說明
        /// </summary>
        public string ServerLog { get; set; }

        /// <summary>
        /// ClientLog說明
        /// </summary>
        public string ClientLog { get; set; }

        /// <summary>
        /// SystemLog說明
        /// </summary>
        public string SystemLog { get; set; }

        /// <summary>
        /// 模擬指令作業
        /// </summary>
        public string SimulationCommandArea { get; set; }

        /// <summary>
        /// Socket設定作業
        /// </summary>
        public string SocketSetArea { get; set; }

    }


    /// <summary>
    /// 左側按鈕
    /// </summary>
    public class LeftButtonSet
    {
        /// <summary>
        /// 所有Log訊息按鈕顯示名稱
        /// </summary>
        public string AllLogButton { get; set; }


        /// <summary>
        /// Socket Log訊息按鈕顯示名稱
        /// </summary>
        public string SocketAllLogButton { get; set; }


        /// <summary>
        /// Server Log訊息按鈕顯示名稱
        /// </summary>
        public string ServerLogButton { get; set; }

        /// <summary>
        /// Clinet Log訊息按鈕顯示名稱
        /// </summary>
        public string ClientLogButton { get; set; }


        /// <summary>
        /// System Log訊息按鈕顯示名稱
        /// </summary>
        public string SystemLogButton { get; set; }

        /// <summary>
        /// 模擬指令按鈕顯示名稱
        /// </summary>
        public string SimulationCommand { get; set; }

        /// <summary>
        /// 系統流程按鈕顯示名稱
        /// </summary>
        public string SocketSet { get; set; }

    }
}
