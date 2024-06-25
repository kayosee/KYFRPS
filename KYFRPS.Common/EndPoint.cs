using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public class EndPoint : IDisposable
    {
        private bool _dispose;

        public int Id { get { return (int)Socket.Handle; } }
        public Socket Socket { get; }
        public Thread Thread { get; }
        public ManualResetEvent Flag { get; }
        public Action<Socket> Action { get; set; }
        public EndPoint(Socket socket, Action<Socket> action)
        {
            Socket = socket;
            Flag = new ManualResetEvent(false);
            Action = action;
            Thread = new Thread(Loop);
            Thread.Name = "outer-" + Id;
            Thread.IsBackground = true;
            Thread.Start();
        }
        public void Run()
        {
            Flag.Set();
        }
        public void Pause()
        {
            Flag.Reset();
        }
        private void Loop()
        {
            while (Flag.WaitOne())
            {
                if (_dispose)
                {
                    if (Socket.Connected)
                        Socket.Close();
                    break;
                }

                if (Socket.Connected)
                    Action(Socket);
            }
        }
        public void Dispose()
        {
            _dispose = true;
        }
    }
}
