using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class Forwarder
    {
        private Thread _thread;
        private ConcurrentDictionary<int, EndPoint> _endPoints;
        private Socket _center;
        public Socket Center { get => _center; }
        public delegate void PackHandler<in T>(T packet) where T : Packet;
        public event PackHandler<PacketValidationRequest>? OnValidationRequest;
        public event PackHandler<PacketListenRequest>? OnListenRequest;
        public event PackHandler<PacketListenResponse>? OnListenResponse;
        public event PackHandler<PacketValidationResponse>? OnValidationResponse;
        public event PackHandler<PacketForwardRequest>? OnForwardRequest;
        public event PackHandler<PacketForwardResponse>? OnForwardResponse;
        public event PackHandler<PacketConnectRequest>? OnConnectRequest;
        public event PackHandler<PacketConnectResponse>? OnConnectResponse;
        public event PackHandler<PacketDisconnectRequest>? OnDisconnectRequest;
        public event PackHandler<PacketDisconnectResponse>? OnDisconnectResponse;
        public event PackHandler<PacketConnectFailRequest>? OnConnectFailRequest;
        public event PackHandler<PacketConnectFailResponse>? OnConnectFailResponse;
        public event Action<Socket,SocketException>? OnCenterSocketError;
        public event Action<Socket, SocketException>? OnEndPointSocketError;
        private readonly Type _type = typeof(Forwarder);
        public Forwarder()
        {
            _endPoints = new ConcurrentDictionary<int, EndPoint>();
            OnForwardRequest += OnForward;
        }

        private void Invoke(Packet packet)
        {
            var name = "On" + Enum.GetName(packet.Type);
            FieldInfo? fi = _type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (fi != null)
            {
                Delegate? @delegate = fi.GetValue(this) as Delegate;
                var list = @delegate?.GetInvocationList();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        item.DynamicInvoke(packet);
                    }
                }
            }
        }
        public void StartForward(Socket socket)
        {
            _center = socket;
            _thread = new Thread(() =>
            {
                while (_center.Connected)
                {
                    try
                    {
                        var packet = Packet.FromSocket(_center);
                        if (packet != null)
                        {
                            Invoke(packet);
                        }

                    }
                    catch (SocketException ex)
                    {
                        OnCenterSocketError?.Invoke(socket, ex);
                        Console.WriteLine(ex.ToString());
                    }
                }
            });
            _thread.Name = "inner";
            _thread.IsBackground = true;
            _thread.Start();
        }
        private void OnForward(PacketForwardRequest packet)
        {
            _endPoints[packet.SocketId].Socket.SendAsync(packet.DataArray);
        }

        public EndPoint AddEndPoint(Socket socket)
        {
            var id = (int)socket.Handle;
            var task = new EndPoint(socket, f => Read(f));
            _endPoints.TryAdd(id, task);
            return task;
        }
        public EndPoint AddEndPoint(int otherId, Socket socket)
        {
            var task = new EndPoint(socket, f => Read(f));
            _endPoints.TryAdd(task.Id, task);
            _endPoints.TryAdd(otherId, task);
            return task;
        }
        private void Read(Socket socket)
        {
            try
            {
                var buffer = new byte[4096];
                int nret = socket.Receive(buffer);
                var packet = new PacketForwardRequest(0, DateTime.Now.Ticks, (int)socket.Handle, buffer.Take(nret).ToArray());
                _center.SendAsync(packet.GetBytes());
            }
            catch (SocketException e)
            {
                OnEndPointSocketError?.Invoke(socket, e);
                Console.WriteLine("连接被关闭:" + e.Message + e.StackTrace);
                RemoveEndPoint((int)socket.Handle);
                var request = new PacketDisconnectRequest(0, DateTime.Now.Ticks, (int)socket.Handle);
                _center.SendAsync(request.GetBytes());
            }
        }
        public bool RemoveEndPoint(int id)
        {
            var ok = _endPoints.TryRemove(id, out var task);
            if (ok)
            {
                task.Dispose();
                CleanDispose();
            }
            return ok;
        }

        private void CleanDispose()
        {
            var closed = _endPoints.Where(f => !f.Value.Socket.Connected).ToList();
            foreach (var f in closed)
            {
                _endPoints.Remove(f.Key, out var _);
            }
        }

        public void MapEndPoints(int clientId, int serverId)
        {
            _endPoints.TryAdd(clientId, _endPoints[serverId]);
        }
        public void StartEndPoint(int id)
        {
            _endPoints[id].Run();
        }
        public void PauseEndPoint(int id)
        {
            _endPoints[id].Pause();
        }
    }
}
