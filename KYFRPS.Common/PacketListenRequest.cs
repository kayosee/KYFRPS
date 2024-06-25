using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketListenRequest : PacketRequest
    {
        public ProtocolType ProtocolType { get; set; }
        public uint ClientPort { get; set; }
        public uint ServerPort { get; set; }
        public PacketListenRequest(int clentId, long requestId, ProtocolType protocolType, uint clientPort, uint serverPort) : base(PacketType.ListenRequest, clentId, requestId)
        {
            ProtocolType = protocolType;
            ClientPort = clientPort;
            ServerPort = serverPort;
        }
        public PacketListenRequest(ByteArrayStream stream) : base(stream)
        {
            ProtocolType = (ProtocolType)stream.ReadUShort();
            ClientPort = stream.ReadUInt();
            ServerPort = stream.ReadUInt();
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.Write((ushort)ProtocolType);
            stream.Write(ClientPort);
            stream.Write(ServerPort);
            return stream;
        }
    }
}
