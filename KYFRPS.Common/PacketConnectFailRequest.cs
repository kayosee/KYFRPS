using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketConnectFailRequest : PacketRequest
    {
        public int SocketId { get; set; }
        public SocketError SocketError { get; set; }
        public PacketConnectFailRequest(int clientId, long requestId, int socketId, SocketError socketError) : base(PacketType.ConnectFailRequest, clientId, requestId)
        {
            SocketId = socketId;
            SocketError = socketError;
        }
        public PacketConnectFailRequest(ByteArrayStream stream) : base(stream)
        {
            SocketId = stream.ReadInt();
            SocketError = (SocketError)stream.ReadInt();
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.Write(SocketId);
            stream.Write((int)SocketError);
            return stream;
        }
    }
}
