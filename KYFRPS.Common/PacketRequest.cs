using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public abstract class PacketRequest : PacketSession
    {
        public long RequestId { get; set; }
        public PacketRequest(ByteArrayStream stream) : base(stream)
        {
            RequestId = stream.ReadLong();
        }
        public PacketRequest(PacketType packetType, int clientId, long requestId) : base(packetType, clientId)
        {
            RequestId = requestId;
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.Write(RequestId);
            return stream;
        }
    }
}
