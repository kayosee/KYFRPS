using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketDisconnectRequest : PacketRequest
    {
        public int SocketId { get; set; }
        public PacketDisconnectRequest(int clientId, long requestId, int socketId) : base(PacketType.DisconnectRequest, clientId, requestId)
        {
            SocketId = socketId;
        }
        public PacketDisconnectRequest(ByteArrayStream stream) : base(stream)
        {
            SocketId = stream.ReadInt();
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.Write(SocketId);
            return stream;
        }
    }
}
