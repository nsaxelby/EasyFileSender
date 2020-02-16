using EFS.Global.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EFS.Utilities.Discovery
{
    public class DiscoveryAdvertiser
    {
        private readonly int _port;
        private readonly string _sendText;

        private readonly Thread _broadcastThread;
        private bool _runThread = true;

        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private readonly IPEndPoint _sendEndpoint;

        private readonly int _delayMs;
        private readonly Stopwatch _pollStopwatch = new Stopwatch();

        public DiscoveryAdvertiser(string broadcastAddress, int port, ClientInfo clientInfo, int delayMs = 5000)
        {
            _delayMs = delayMs;
            _port = port;

            _sendText = DiscoveryUtils.GetDiscoveryPacketStr(clientInfo);

            _broadcastThread = new Thread(new ThreadStart(this.DoBroadcastEvent));

            IPAddress broadcastAdd = IPAddress.Parse(broadcastAddress);
            _sendEndpoint = new IPEndPoint(broadcastAdd, port);
        }

        public bool StartDiscoveryPolling()
        {
            _runThread = true;
            _broadcastThread.Start();
            return true;
        }

        public bool StopDiscoveryPolling()
        {
            _runThread = false;
            if (_broadcastThread.ThreadState == System.Threading.ThreadState.Running)
            {
                _broadcastThread.Join();
            }
            return true;
        }

        public void DoBroadcastEvent()
        {
            // Upon first fire, before starting, do a broadcast so we get an immediate poll on startup
            byte[] send_buffer = Encoding.ASCII.GetBytes(_sendText);
            _socket.SendTo(send_buffer, _sendEndpoint);

            _pollStopwatch.Start();
            // While loop is on 200 ms poll, but only send broadcast on bigger tick
            while (_runThread)
            {
                if (_pollStopwatch.ElapsedMilliseconds >= _delayMs)
                {
                    send_buffer = Encoding.ASCII.GetBytes(_sendText);
                    _socket.SendTo(send_buffer, _sendEndpoint);
                    _pollStopwatch.Reset();
                    _pollStopwatch.Start();
                }
                Thread.Sleep(200);
            }

            try
            {
                _socket.Close();
            }
            catch
            {

            }
        }
    }
}
