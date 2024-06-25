using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketConnectFailResponse : PacketResponse
    {
        public int SocketId { get; set; }
        public PacketConnectFailResponse(int clientId, long requestId, int socketId, ErrorType error) : base(PacketType.ConnectFailResponse, clientId, requestId, error)
        {
            SocketId = socketId;
        }
        public PacketConnectFailResponse(ByteArrayStream stream) : base(stream)
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
