using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSocketUI.Attributes
{
    public class SAASocketClient
    {
                /// <summary>
        /// Server IP
        /// </summary>
        public string SocketIP { get; set; }

        /// <summary>
        /// Server Port 
        /// </summary>
        public string SocketPort { get; set; }

        /// <summary>
        /// Server Subject
        /// </summary>
        public string SocketSubject { get; set; }



        public string SAA_DatabaseName { get; set; }
        public string SAA_DatabaseIP { get; set; }

        public string Plasma_DatabaseName { get; set; }
        public string Plasma_DatabaseIP { get; set; }


    }
}
