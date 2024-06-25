using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketForwardResponse : PacketResponse
    {
        public PacketForwardResponse(int clientId, long requestId, ErrorType error) : base(PacketType.ForwardResponse, clientId, requestId, error)
        {
        }
        public PacketForwardResponse(ByteArrayStream stream) : base(stream)
        {
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            return stream;
        }
    }
}
