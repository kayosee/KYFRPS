using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketConnectResponse : PacketResponse
    {
        public int ClientSocketId { get; set; }
        public int ServerSocketId { get; set; }
        public PacketConnectResponse(int clientId, long requestId, int clientSocketId, int serverSocketId, ErrorType error) : base(PacketType.ConnectResponse, clientId, requestId, error)
        {
            ClientSocketId = clientSocketId;
            ServerSocketId = serverSocketId;
        }
        public PacketConnectResponse(ByteArrayStream stream) : base(stream)
        {
            ClientSocketId = stream.ReadInt();
            ServerSocketId = stream.ReadInt();
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.Write(ClientSocketId);
            stream.Write(ServerSocketId);
            return stream;
        }
    }
}
