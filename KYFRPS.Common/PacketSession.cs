using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public abstract class PacketSession : Packet
    {
        public int ClientId { get; set; }
        public PacketSession(PacketType packetType, int clientId) : base(packetType)
        {
            ClientId = clientId;
        }

        public PacketSession(ByteArrayStream stream) : base(stream)
        {
            ClientId = stream.ReadInt();
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.Write(ClientId);
            return stream;
        }
    }
}
