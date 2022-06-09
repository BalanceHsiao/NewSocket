using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SocketLib.SQLDB.DAL.DataAccess
{
    class ConnStrHelper
    {
        /// <summary>
        /// 取得連線Sql Server 連線字串
        /// </summary>
        /// <returns></returns>
        public static string GetConnStr()
        {
            string GetConnStr = "";
            try
            {
                GetConnStr = ConfigurationManager.ConnectionStrings["ConnStr"].ToString();
            }
            catch
            {
                GetConnStr = "";
            }

            return GetConnStr;
        }
    }
}
