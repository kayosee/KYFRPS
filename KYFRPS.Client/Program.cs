using System.Configuration;
using System.Net.Sockets;

namespace KYFRPS.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            var serverIp = ConfigurationManager.AppSettings[nameof(Client.ServerIp)];
            var serverPort = int.Parse(ConfigurationManager.AppSettings[nameof(Client.ServerPort)]);
            var password = ConfigurationManager.AppSettings[nameof(Client.Password)];
            var localPort = int.Parse(ConfigurationManager.AppSettings[nameof(Client.LocalPort)]);
            var remotePort = int.Parse(ConfigurationManager.AppSettings[nameof(Client.RemotePort)]);
            var protocolTypeStr = ConfigurationManager.AppSettings[nameof(Client.ProtocolType)];

            client.Connect(serverIp, serverPort, password, localPort,remotePort, (ProtocolType)Enum.Parse(typeof(ProtocolType), protocolTypeStr));
            Console.ReadKey();
        }
    }
}
