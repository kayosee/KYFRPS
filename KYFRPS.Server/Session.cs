using KYFRPS.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KYFRPS.Server
{
    public class Session
    {
        private Socket _listenSocket;
        private Thread _listenThread;
        private Forwarder _forwarder;
        public Session(Socket client)
        {
            _forwarder = new Forwarder();
            _forwarder.StartForward(client);
            _forwarder.OnConnectResponse += OnConnectResponse;
            _forwarder.OnListenRequest += OnListenRequest;
            _forwarder.OnValidationRequest += OnValidationRequest;
            _forwarder.OnDisconnectRequest += OnDisconnectRequest;
        }
        private void OnDisconnectRequest(PacketDisconnectRequest packet)
        {
            Console.WriteLine("收到断开请求");
            _forwarder.RemoveEndPoint(packet.SocketId);
            var response = new PacketDisconnectResponse(packet.ClientId, packet.RequestId, ErrorType.NoError);
            _forwarder.Center.SendAsync(response.GetBytes());
        }

        private void OnConnectResponse(PacketConnectResponse packet)
        {
            if (packet.Success)
            {
                _forwarder.MapEndPoints(packet.ClientSocketId, packet.ServerSocketId);
                _forwarder.StartEndPoint(packet.ServerSocketId);
                Console.WriteLine("客户端转发连接成功");
            }
            else
                Console.WriteLine("客户端转发连接失败");
        }
        private void OnListenRequest(PacketListenRequest packet)
        {
            _listenSocket = new Socket(SocketType.Stream, packet.ProtocolType);
            _listenSocket.Bind(new IPEndPoint(IPAddress.Any, (int)packet.ServerPort));
            _listenSocket.Listen();
            _listenThread = new Thread(() =>
            {
                while (true)
                {
                    var socket = (Socket)_listenSocket.Accept();
                    _forwarder.AddEndPoint(socket);
                    var request = new PacketConnectRequest(packet.ClientId, DateTime.Now.Ticks, (int)socket.Handle);
                    _forwarder.Center.Send(request.GetBytes());
                }
            });
            _listenThread.Name = "listen";
            _listenThread.IsBackground = true;
            _listenThread.Start();

            Console.WriteLine($"在服务端监听：{packet.ServerPort}");
        }

        private void OnValidationRequest(PacketValidationRequest packet)
        {
            var response = new PacketValidationResponse(packet.ClientId, packet.RequestId, ErrorType.NoError);
            var password = ConfigurationManager.AppSettings["Password"];
            if (packet.Password != password)
            {
                response.Error = ErrorType.PasswordInvalid;
            }
            _forwarder.Center.SendAsync(response.GetBytes());
        }

    }
}
