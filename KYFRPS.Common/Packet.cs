using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class Packet
    {
        public PacketType Type { get; set; }
        public int Length { get; set; }
        public Packet(PacketType packetType)
        {
            Type = packetType;
        }
        public Packet(ByteArrayStream stream)
        {
            Type = (PacketType)stream.ReadByte();
            Length = stream.ReadInt();
        }
        protected virtual ByteArrayStream GetStream()
        {
            return new ByteArrayStream();
        }
        public byte[] GetBytes()
        {
            var body = GetStream().GetBuffer();
            var stream = new ByteArrayStream();
            stream.Write((byte)Type);
            stream.Write(body.Length);
            stream.Write(body);
            return stream.GetBuffer();
        }
        public static Packet? FromSocket(Socket socket)
        {
            using (var stream = new ByteArrayStream())
            {
                var type = socket.ReadExactly(1)[0];
                if (!Enum.IsDefined(typeof(PacketType), type))
                    throw new Exception();

                stream.Write(type);

                var length = BitConverter.ToInt32(socket.ReadExactly(sizeof(int)));
                stream.Write(length);

                stream.Write(socket.ReadExactly(length));

                return BuildPacket(stream, type);
            }
        }
        private static Packet? BuildPacket(ByteArrayStream stream, byte type)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = Enum.GetName(typeof(PacketType), type);
            var constructor = assembly.GetTypes().First(f => f.Name == "Packet" + name).GetConstructors().First(f => f.GetParameters().Any(s => s.ParameterType == typeof(ByteArrayStream)));
            if (constructor != null)
            {
                return (Packet)constructor.Invoke(new object[] { stream });
            }
            return null;
        }
    }

    internal record NewRecord(ByteArrayStream Stream);
}
