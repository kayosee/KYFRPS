using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketListenResponse : PacketResponse
    {
        public int SocketId { get; set; }
        public uint ClientPort { get; set; }
        public uint ServerPort { get; set; }
        public PacketListenResponse(int clientId, long requestId, ErrorType error, int socketId, uint clientPort,uint serverPort) : base(PacketType.ListenResponse, clientId, requestId, error)
        {
            SocketId = socketId;
            ClientPort = clientPort;
            ServerPort = serverPort;
        }
        public PacketListenResponse(ByteArrayStream stream) : base(stream)
        {
            SocketId = stream.ReadInt();
            ClientPort = stream.ReadUInt();
            ServerPort = stream.ReadUInt();
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.Write(SocketId);
            stream.Write(ClientPort);
            stream.Write(ServerPort);
            return stream;
        }
    }
}
