using KYFRPS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Server
{
    public class Server
    {
        public int LocalPort { get; set; }
        private Socket _socket;
        private Thread _thread;
        private List<Session> _peers;
        public void Start(int port)
        {
            LocalPort = port;
            _peers = new List<Session>();
            _socket = new Socket(0,SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen();
            _thread = new Thread((ThreadStart) =>
            {
                while (true)
                {
                    var client = (Socket)_socket.Accept();
                    _peers.Add(new Session(client));
                }
            });
            _thread.Start();
            _thread.IsBackground = true;
        }
    }
}
