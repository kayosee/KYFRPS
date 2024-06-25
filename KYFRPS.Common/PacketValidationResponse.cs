using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketValidationResponse : PacketResponse
    {
        public PacketValidationResponse(int clientId, long requestId, ErrorType error) : base(PacketType.ValidationResponse, clientId, requestId, error)
        {
        }
        public PacketValidationResponse(ByteArrayStream stream) : base(stream)
        {
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            return stream;
        }
    }
}
