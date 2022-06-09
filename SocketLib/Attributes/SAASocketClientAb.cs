using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSocket;

namespace SocketLib.Attributes
{
    public class SAASocketClientAb
    {
        public static WinSocketClient SAASCA = new WinSocketClient();
        private static string _sendCommand = string.Empty;

        public static string GetSedStackerCommands(Dictionary<string, string> DictionASE)
        {

            _sendCommand = string.Empty;
            foreach (KeyValuePair<string, string> Single in DictionASE)
            {
                _sendCommand += string.Format("{0}:{1},", Single.Key, Single.Value);
            }

            _sendCommand = _sendCommand.Substring(0, _sendCommand.Length - 1);

            return _sendCommand;
        }



    }
}
