using SocketLib.Model.BusinessModel;
using SocketLib.SQLDB.DAL.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SocketLib.Model.SAAModel.SAAModel;



namespace SocketLib.SQLDB.BLL.BusinessLogic
{
    class DirectiveManager
    {
        private static object objLocked = new object();

        DirectiveService dS = new DirectiveService();


        public oDirective GetDirective(string SAASocketType, ref ReturnInfo returnInfo)
        {
            oDirective GetoD = new oDirective();
            if (SAASocketType ==  SAASocketName.SAA_DB.ToString())
            {
                GetoD = this.dS.GetSAADirective(ref returnInfo);
            }
            //else if (SAASocketType == SAASocketName.Plasma_DB.ToString())
            //{
            //    //getDirective = this.dS.GetPlasmaDirective(ref returnInfo);
            //}


            return GetoD;
        }


        /// <summary>
        /// 檢查指令格式模組
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="returnInfo"></param>
        public void CheckDirective(string commandText, ref ReturnInfo returnInfo)
        {
            string[] sArray = commandText.Split(',');

            DataTable checkDirective = this.dS.CheckDirective(sArray[0].Replace("CMD_NO:", "").Trim().ToString(), ref returnInfo);

            if (checkDirective.Rows.Count > 0)
            {
                string CheckFlg = "";


                for (int i = 0; i < checkDirective.Rows.Count; i++)
                {

                    if (commandText.Contains(checkDirective.Rows[i]["DETAIL_CMD"].ToString()) == false)
                    {
                        CheckFlg += "無" + checkDirective.Rows[i]["DETAIL_CMD"].ToString() + "格式資訊;" + Environment.NewLine;
                    }
                }

                if (CheckFlg == "")
                {
                    returnInfo.Message = "OK";
                }
                else
                {
                    returnInfo.Message = sArray[1] + "指令格式異常" + Environment.NewLine + CheckFlg;
                }

            }
            else
            {
                returnInfo.Message = "查無" + commandText + "指令的格式模組";
            }

        }

        public int UpdateDirective(oDirective UpoD, ref ReturnInfo returnInfo)
        {

            this.dS.UpdateDirective(UpoD, ref returnInfo);

            if (returnInfo.Message == "OK")
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }

        public int InsertDirectiveASECommand(string commandText, ref ReturnInfo returnInfo)
        {

            string[] sArray = commandText.Split(',');


            int i = this.dS.InsertDirectiveASECommand(sArray[0].Replace("CMD_NO:", "").Trim().ToString(), commandText, ref returnInfo);

            return 0;
        }



    }
}
