using MessageHelper;
using SocketLib.Model.BusinessModel;
using SocketLib.SQLDB.BLL.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Threading;
using static MessageHelper.Model.MessageModel;
using static SocketLib.Model.SAAModel.SAAModel;
using SocketLib.Attributes;
using WinSocket;

namespace SocketLib.SocketCommand
{
    public class SAACommand
    {
        private bool _dbConnection = true;

        DirectiveManager dM = new DirectiveManager();

        public void Execute()
        {

            try
            {
                Thread t1 = new Thread(() =>
                {
                    while (true)
                    {
                        SAAJsonReport(); // WIP監控
                        Thread.Sleep(1000);
                    }
                })
                { IsBackground = true };
                t1.Start();
            }
            catch (Exception ex)
            {
                ShowMessage.LogMessage($"上報訊息時發生異常。Stack:{ex.StackTrace},Message:{ex.Message}", MessageLevel.Error);
            }

        }

        private void SAAJsonReport()
        {

           try
            {

                ReturnInfo returnInfo = new ReturnInfo();

                oDirective _DirectiveData = this.dM.GetDirective(SAASocketName.SAA_DB.ToString(), ref returnInfo);


                if (_DirectiveData  != null)
                {
                    Dictionary<string, string> dSendValue = new Dictionary<string, string>();

                    string[] aSendValue = _DirectiveData.CommandText.Split(new char[] { ',' });

                    foreach (string sStr in aSendValue)
                    {
                        if (!string.IsNullOrEmpty(sStr))
                        {
                            dSendValue.Add(sStr.Split(':')[0], sStr.Split(':')[1]);
                        }
                    }

                    if (_dbConnection == false)
                    {
                        ShowMessage.LogMessage("SQL Server連線異常...", MessageLevel.Error);
                        return;
                    }
                        

                    try
                    {
                       

                       SAASocketClientAb.SAASCA.CommunicationQueue.SendEventQueue.Enqueue(dSendValue);

                        ShowMessage.LogMessage("己傳送【" + _DirectiveData.CommandId + "】指令至Clien端", MessageLevel.Normal, SocketMessageType.Server);


                        if (this.dM.UpdateDirective(_DirectiveData, ref returnInfo) == 0)
                        {
                            ShowMessage.LogMessage("己完成【" + _DirectiveData.CommandText + "】指令至Clien端", MessageLevel.Normal, SocketMessageType.Server);
                        }
                        else
                        {
                            ShowMessage.LogMessage("傳送【" + _DirectiveData.CommandText + "】指令至Clien端發生異常，異常原因：" + returnInfo.Message, MessageLevel.Error, SocketMessageType.Server);
                        }
                    }
                    catch
                    {
                        ;
                    }
                }

                //傳送指令至Client端

                Thread.Sleep(1000);


            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("建立連接"))
                {
                    if (_dbConnection == false)
                        return;

                    _dbConnection = false;

                    ShowMessage.LogMessage("SQL Server連線異常...", MessageLevel.Error);

                    return;
                }

                ShowMessage.LogMessage(ex.StackTrace, MessageLevel.Error);
            }
        }

    }
}
