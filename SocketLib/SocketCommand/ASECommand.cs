using MessageHelper;
using SocketLib.Attributes;
using SocketLib.Model.BusinessModel;
using SocketLib.SQLDB.BLL.BusinessLogic;
using System;
using WinSocket;
using static MessageHelper.Model.MessageModel;

namespace SocketLib.SocketCommand
{
    class ASECommand
    {
        private string SendCommand = string.Empty;
        DirectiveManager dM = new DirectiveManager();

        public void Execute()
        {
            SAASocketClientAb.SAASCA.onReturnReceivedMsg += SocketClient_onReturnReceivedMsg;
        }

        private void SocketClient_onReturnReceivedMsg(object sender, ReceivedMessageInformation e)
        {
            try
            {
                ReturnInfo returnInfo = new ReturnInfo();


                SendCommand = SAASocketClientAb.GetSedStackerCommands(e.ReceiveMessage);

                int i = this.dM.InsertDirectiveASECommand(SendCommand, ref returnInfo);

                if (SendCommand.Substring(0,9).ToString()== "ReplyTEID")
                {
                    ShowMessage.LogMessage( SendCommand , MessageLevel.Normal, SocketMessageType.Client);
                }
                else
                {
                    ShowMessage.LogMessage("己接收Server端【" + SendCommand + "】指令", MessageLevel.Normal, SocketMessageType.Client);
                }

                
            }
            catch (Exception ex)
            {
                ShowMessage.LogMessage($"接收訊息時發生異常。Stack:{ex.StackTrace},Message:{ex.Message}", MessageLevel.Error, SocketMessageType.Server);
            }

        }
    }
}
