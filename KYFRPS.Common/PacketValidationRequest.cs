using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class PacketValidationRequest : PacketRequest
    {
        public String Password { get; set; }
        public PacketValidationRequest(long requestId, string password) : base(PacketType.ValidationRequest, 0, requestId)
        {
            Password = password;
        }
        public PacketValidationRequest(ByteArrayStream stream) : base(stream)
        {
            Password = stream.ReadUTF8String();
        }
        protected override ByteArrayStream GetStream()
        {
            var stream = base.GetStream();
            stream.WriteUTF8string(Password);
            return stream;
        }

    }
}
