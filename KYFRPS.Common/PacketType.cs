using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public enum PacketType:byte
    {
        ValidationRequest = 1,
        ListenRequest = 2,
        ListenResponse = 3,
        ValidationResponse = 4,
        ForwardRequest,
        ForwardResponse,
        ConnectRequest,
        ConnectResponse,
        DisconnectRequest,
        DisconnectResponse,
        ConnectFailRequest,
        ConnectFailResponse,
    }
}
