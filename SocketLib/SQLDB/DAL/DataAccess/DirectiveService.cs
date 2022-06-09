using Dapper;
using SocketLib.Model.BusinessModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketLib.SQLDB.BLL.Helper;

namespace SocketLib.SQLDB.DAL.DataAccess
{
    class DirectiveService
    {
        /// <summary>
        /// 檢查指令格式模組
        /// </summary>
        /// <param name="CMD_NO"></param>
        /// <param name="returnInfo"></param>
        /// <returns></returns>
        public DataTable CheckDirective(string CMD_NO, ref ReturnInfo returnInfo)
        {

            string connStr = ConnStrHelper.GetConnStr(); //

            #region SQL 語法
            string sSQL = @"";

            sSQL += " SELECT CSM.Communication,CSD.DETAIL_CMD,CSD.CMD_DESCRIPTION " + Environment.NewLine;
            sSQL += " FROM sCmdSetMaster CSM LEFT JOIN sCmdSetDetail CSD " + Environment.NewLine;
            sSQL += " ON CSM.MID = CSD.MID " + Environment.NewLine;
            sSQL += " WHERE CMD_ID = @CMD_ID " + Environment.NewLine;
            sSQL += " ORDER BY CSD.DETAIL_CMD" + Environment.NewLine;
            #endregion SQL 語法

            DynamicParameters paras = new DynamicParameters();

            paras.Add("CMD_ID", CMD_NO);

            DataTable result = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    var reader = conn.ExecuteReader(sSQL, paras);
                    result = new DataTable();
                    result.Load(reader);
                    result.TableName = "CMD_DETAIL";
                    returnInfo.Message = "OK";
                }
                catch (Exception ex)
                {
                    returnInfo.Message = ex.ToString();
                    returnInfo.Exception = ex;
                }
            }

            return result;
        }

        /// <summary>
        /// 取得SAA指令訊息
        /// </summary>
        /// <param name="returnInfo"></param>
        /// <returns></returns>
        public oDirective GetSAADirective(ref ReturnInfo returnInfo)
        {

            string connStr = ConnStrHelper.GetConnStr(); //

            #region SQL 語法
            string sSQL = @"";

            sSQL += " SELECT  TaskDateTime,CommandId,CommandText FROM oDirective WHERE  Sender ='SAA' AND  SendFlag is NULL" + Environment.NewLine;
            sSQL += " order by TaskDateTime" + Environment.NewLine;

            #endregion SQL 語法

            oDirective results = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {

                    results = conn.Query<oDirective>(sSQL).First();
                    returnInfo.Message = "OK";

                }
                catch (Exception ex)
                {
                    returnInfo.Message = ex.ToString();
                    returnInfo.Exception = ex;
                    
                }
            }
            return results;

        }


        /// <summary>
        /// 更新指令資料
        /// </summary>
        public int UpdateDirective(oDirective UpoD, ref ReturnInfo returnInfo)
        {
            string connStr = ConnStrHelper.GetConnStr(); //
            #region sqlcontentlotId
            string sql = $@"update oDirective set SendFlag = 'Y' where TaskDateTime = @TaskDateTime And CommandId=@CommandId";
            #endregion
            int _intResult = 0;
            DynamicParameters paras = new DynamicParameters();
            paras.Add("TaskDateTime", UpoD.TaskDateTime);
            paras.Add("CommandId", UpoD.CommandId);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    _intResult = conn.Execute(sql, paras, trans);
                    trans.Commit();
                    returnInfo.Message = "OK";
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnInfo.Message = ex.Message;
                    returnInfo.Exception = ex;
                }
            }
            return _intResult;
        }


        /// <summary>
        /// 新增ASECommand
        /// </summary>
        public int InsertDirectiveASECommand(string commandId, string commandText, ref ReturnInfo returnInfo)
        {

            string connStr = ConnStrHelper.GetConnStr(); //
            #region sqlcontentlotId
            string sql = $@"";

            sql += " insert into oDirective(TaskDateTime, CommandId, CommandText, Sender)" + Environment.NewLine;
            sql += " values (@TaskDateTime, @CommandId, @CommandText, @Sender)";


            #endregion
            int _intResult = 0;
            DynamicParameters paras = new DynamicParameters();
            paras.Add("TaskDateTime", ConvertHelper.GetTaskDateTimeIncludeRandom(true));
            paras.Add("CommandId", commandId);
            paras.Add("commandText", commandText);
            paras.Add("Sender", "ASE");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    _intResult = conn.Execute(sql, paras, trans);
                    trans.Commit();
                    returnInfo.Message = "OK";
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    returnInfo.Message = ex.Message;
                    returnInfo.Exception = ex;
                }
            }
            return _intResult;
        }


    }
}
