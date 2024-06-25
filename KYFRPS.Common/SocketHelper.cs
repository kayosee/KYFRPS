using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KYFRPS.Common
{
    public static class SocketHelper
    {
        public static byte[] ReadExactly(this Socket socket,int count)
        {
            int nret = 0;
            var buffer = new byte[count];
            while (nret < count)
            {
                int size = socket.Receive(buffer, nret, count - nret, SocketFlags.None);
                nret += size;
            }
            return buffer;
        }
        public static int ReadExactly(this Socket socket, byte[] buffer, int count)
        {
            int nret = 0;
            while (nret < count)
            {
                int size = socket.Receive(buffer, nret, count - nret, SocketFlags.None);
                nret += size;
            }
            return nret;
        }

    }
}
