using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketDisconnectResponse : PacketResponse
    {
        public PacketDisconnectResponse(int clientId, long requestId, ErrorType error) : base(PacketType.DisconnectResponse, clientId, requestId, error)
        {
        }
        public PacketDisconnectResponse(ByteArrayStream stream) : base(stream)
        {
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            return stream;
        }
    }
}
