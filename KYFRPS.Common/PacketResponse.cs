using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public abstract class PacketResponse : PacketSession
    {
        public long RequestId { get; set; }
        public ErrorType Error { get; set; }
        public bool Success { get { return Error == 0; } }
        protected PacketResponse(PacketType packetType, int clientId, long requestId, ErrorType error) : base(packetType, clientId)
        {
            RequestId = requestId;
            Error = error;
        }
        public PacketResponse(ByteArrayStream stream) : base(stream)
        {
            RequestId = stream.ReadLong();
            Error = (ErrorType)stream.ReadInt();
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.Write(RequestId);
            stream.Write((int)Error);
            return stream;
        }
    }
}
