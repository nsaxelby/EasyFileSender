using EFS.Global.Exceptions;
using EFS.Global.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EFS.Utilities.Discovery
{
    public class DiscoveryListener
    {
        // Credit: https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9

        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int _bufSize = 8 * 1024;
        private State _state = new State();
        private EndPoint _listenEndPoint;
        private readonly AsyncCallback _asyncCallback = null;
        private readonly string _myIpAddress;
        private readonly OnRecievedClientData _delegateMethod;

        public delegate void OnRecievedClientData(ClientInfo clientInfo);

        public class State
        {
            public byte[] buffer = new byte[_bufSize];
        }

        public DiscoveryListener(string myIpAddress, int port, OnRecievedClientData onRecievedClientData)
        {
            _myIpAddress = myIpAddress;

            // Note, I do not think the port or IP Addres of _listenEndPoint is relevant, any value can be used
            _listenEndPoint = new IPEndPoint(IPAddress.Any, port);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));

            _asyncCallback = ProcessRecievedPacket;
            _delegateMethod = onRecievedClientData;
        }

        public void StartListener()
        {
            _socket.BeginReceiveFrom(_state.buffer, 0, _bufSize, SocketFlags.None, ref _listenEndPoint, _asyncCallback, _state);
        }

        public void StopListener()
        {
            try
            {
                _socket.Close();
            }
            catch
            {
                Debug.WriteLine("Exception closing socket");
            }
        }

        private void ProcessRecievedPacket(IAsyncResult ar)
        {
            try
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref _listenEndPoint);
                _socket.BeginReceiveFrom(so.buffer, 0, _bufSize, SocketFlags.None, ref _listenEndPoint, _asyncCallback, so);
                string recievedStr = Encoding.ASCII.GetString(so.buffer, 0, bytes);
                try
                {
                    ClientInfo ci = DiscoveryUtils.GetClientInfoFromString(recievedStr);
                    if (string.Equals(ci.IpAddress, _myIpAddress) == false)
                    {
                        _delegateMethod(ci);
                    }
                }
                catch (MalformedUDPBroadcastException malformedExc)
                {
                    Debug.WriteLine("Malformed packet str: " + recievedStr + " ex: " + malformedExc.Message);
                }
            }
            catch (ObjectDisposedException)
            {
                // This is expected on close
            }
        }
    }
}
