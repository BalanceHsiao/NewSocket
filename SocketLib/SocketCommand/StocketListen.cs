using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MessageHelper;
using NewSocketUI.Attributes;
using SocketLib.Attributes;
using WinSocket;
using log4net;
using static MessageHelper.Model.MessageModel;

namespace SocketLib.SocketCommand
{

    public class StocketListen
    {
        private SAACommand SaaCommand = new SAACommand();
        private ASECommand AseCommand = new ASECommand();

        public void SocketStart(SAASocketClient Sa)
        {
            try
            {
                SAASocketClientAb.SAASCA.ClientIP = IPAddress.Parse(Sa.SocketIP);
                SAASocketClientAb.SAASCA.ClientPortNum = int.Parse(Sa.SocketPort);
                SAASocketClientAb.SAASCA.Subject = Sa.SocketSubject;
                SAASocketClientAb.SAASCA.SendRequest = true;
                SAASocketClientAb.SAASCA.ThreadAbort = false;
                SAASocketClientAb.SAASCA.onReturnSystemMsg += SocketClient_onReturnSystemMsg;    //會將Socket元件本身的訊息拋回到主緒中執行
                SAASocketClientAb.SAASCA.Start();
                

                SaaCommand.Execute();
                AseCommand.Execute();


            }
            catch (Exception ex)
            {
                ShowMessage.LogMessage(ex.Message, MessageLevel.Error, SocketMessageType.Systems);
            }

        }

        private void SocketClient_onReturnSystemMsg(object sender, SystemMessageInformation e)
        {
            

            var isException = e.DataMessage;

            if (isException.Contains("遠端主機已強制關閉"))
            {
                //ErrorLog($"Client關閉程式{isException}");
                ShowMessage.LogMessage($"Client關閉程式{isException}", MessageLevel.Error, SocketMessageType.Systems);
                ShowMessage.LogMessage(string.Format("客戶端連接:{0}-服務端埠:{1}-Subject:{2}，己斷線並結束監聽", SAASocketClientAb.SAASCA.ClientIP, SAASocketClientAb.SAASCA.ClientPortNum, SAASocketClientAb.SAASCA.Subject), MessageLevel.Normal, SocketMessageType.From);
            }

            if (isException.IndexOf("Exception", StringComparison.Ordinal) != -1)
            {
                //ErrorLog(e.DataMessage);
                ShowMessage.LogMessage(e.DataMessage, MessageLevel.Error, SocketMessageType.Systems);
                ShowMessage.LogMessage(string.Format("客戶端連接:{0}-服務端埠:{1}-Subject:{2}，己斷線並結束監聽", SAASocketClientAb.SAASCA.ClientIP, SAASocketClientAb.SAASCA.ClientPortNum, SAASocketClientAb.SAASCA.Subject), MessageLevel.Normal, SocketMessageType.From);
            }
            else if (isException.IndexOf("接收到的字串錯誤, 檢查字元數超過正常範圍值", StringComparison.Ordinal) != -1)
            {
                //ErrorLog(e.DataMessage);
                ShowMessage.LogMessage(e.DataMessage, MessageLevel.Error, SocketMessageType.Systems);
                ShowMessage.LogMessage(string.Format("客戶端連接:{0}-服務端埠:{1}-Subject:{2}，己斷線並結束監聽", SAASocketClientAb.SAASCA.ClientIP, SAASocketClientAb.SAASCA.ClientPortNum, SAASocketClientAb.SAASCA.Subject), MessageLevel.Normal, SocketMessageType.From);
            }

            else if (isException.IndexOf("完成與Server端的連線建立!", StringComparison.Ordinal) >=0)
            {
                //ErrorLog(e.DataMessage);
                ShowMessage.LogMessage(e.DataMessage, MessageLevel.Normal, SocketMessageType.Systems);
                ShowMessage.LogMessage(string.Format("客戶端連接:{0}-服務端埠:{1}-Subject:{2}，己連線並開始監聽", SAASocketClientAb.SAASCA.ClientIP, SAASocketClientAb.SAASCA.ClientPortNum, SAASocketClientAb.SAASCA.Subject), MessageLevel.Normal, SocketMessageType.From);
            }
            else
            {
                ShowMessage.LogMessage(string.Format(string.Format("{0},{1}", e.LogSort, e.DataMessage), 1), MessageLevel.Normal, SocketMessageType.Systems);
                ShowMessage.LogMessage(string.Format("客戶端連接:{0}-服務端埠:{1}-Subject:{2}，連線中", SAASocketClientAb.SAASCA.ClientIP, SAASocketClientAb.SAASCA.ClientPortNum, SAASocketClientAb.SAASCA.Subject), MessageLevel.Normal, SocketMessageType.From);
            }
        }

        private void ErrorLog(string msg)
        {
            ShowMessage.LogMessage(msg, MessageLevel.Error, SocketMessageType.Systems);
        }



    }

}
