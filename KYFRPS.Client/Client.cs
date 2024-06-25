using KYFRPS.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Client
{
    public class Client
    {
        public int ClientId { get; set; }
        public bool IsServerConnected { get; set; }
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public string Password { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public int LocalPort { get; set; }
        public int RemotePort { get; set; }

        private Forwarder _forwarder;
        public Client()
        {
        }
        public void Connect(string serverIp, int serverPort, string password, int localPort, int remotePort, ProtocolType protocolType)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
            Password = password;
            LocalPort = localPort;
            RemotePort = remotePort;
            ProtocolType = protocolType;

            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ServerIp, serverPort);

            var validation = new PacketValidationRequest(DateTime.Now.Ticks, password);
            socket.Send(validation.GetBytes());

            _forwarder = new Forwarder();
            _forwarder.StartForward(socket);
            _forwarder.OnConnectRequest += OnConnectRequest;
            _forwarder.OnValidationResponse += OnValidationResponse;
            _forwarder.OnListenResponse += OnListenResponse;
            _forwarder.OnDisconnectRequest += OnDisconnectRequest;
        }
        private void OnDisconnectRequest(PacketDisconnectRequest packet)
        {
            Console.WriteLine("收到断开请求");
            _forwarder.RemoveEndPoint(packet.SocketId);
            var response = new PacketDisconnectResponse(packet.ClientId, packet.RequestId, ErrorType.NoError);
            _forwarder.Center.SendAsync(response.GetBytes());
        }

        private void OnConnectRequest(PacketConnectRequest packet)
        {
            var response = new PacketConnectResponse(packet.ClientId, packet.RequestId, 0, packet.SocketId, ErrorType.NoError);
            try
            {
                var socket = new Socket(SocketType.Stream, ProtocolType);
                socket.Connect(IPAddress.Parse("127.0.0.1"), LocalPort);
                _forwarder.AddEndPoint(packet.SocketId, socket).Run();
                response.ClientSocketId = (int)socket.Handle;
            }
            catch (Exception ex)
            {
                response.Error = ErrorType.ConnectError;
                Console.WriteLine(ex.ToString());
            }
            _forwarder.Center.SendAsync(response.GetBytes());
        }
        private void OnValidationResponse(PacketValidationResponse packet)
        {
            if (packet.Success)
            {
                ClientId = packet.ClientId;
                IsServerConnected = true;
                Console.WriteLine("登录成功");

                var request = new PacketListenRequest(ClientId, DateTime.Now.Ticks, ProtocolType, (uint)LocalPort, (uint)RemotePort);
                _forwarder.Center.SendAsync(request.GetBytes());
            }
            else
            {
                _forwarder.Center.Close();
                Console.WriteLine("登录失败：" + packet.Error);
            }
        }
        private void OnListenResponse(PacketListenResponse packet)
        {
            if (packet.Success)
            {
                try
                {
                    Console.WriteLine("远程端口建立成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                Console.WriteLine("连接建立失败，原因：" + packet.Error);
            }
        }
    }
}
