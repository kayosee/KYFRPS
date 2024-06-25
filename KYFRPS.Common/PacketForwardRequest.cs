using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketForwardRequest : PacketRequest
    {
        public int SocketId { get; set; }
        public int DataLength { get { return DataArray.Length; } }
        public byte[] DataArray { get; set; }
        public PacketForwardRequest(int clientId, long requestId, int socketId, byte[] dataArray) : base(PacketType.ForwardRequest, clientId, requestId)
        {
            SocketId = socketId;
            DataArray = dataArray;
        }
        public PacketForwardRequest(ByteArrayStream stream) : base(stream)
        {
            SocketId = stream.ReadInt();
            var dataLength = stream.ReadInt();
            DataArray = new byte[dataLength];
            stream.Read(DataArray, 0, dataLength);
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.Write(SocketId);
            stream.Write(DataArray.Length);
            stream.Write(DataArray);
            return stream;
        }
    }
}
