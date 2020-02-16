using EFS.Global.Exceptions;
using EFS.Global.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
        private readonly OnClientExpired _clientExpiredDelMethod;

        private List<IpTimerRecord> _clientIpRecords = new List<IpTimerRecord>();

        private readonly Thread _removeExpiredClientsThread;
        private bool _runThread = true;
        private readonly int _removeExpiredClientsPollDelay = 30000;
        private readonly int _removeExpiredClientsSecondsWithoutPollExpire = 30;
        private readonly Stopwatch _removeExpiredClientsPollStopwatch = new Stopwatch();

        public delegate void OnRecievedClientData(ClientInfo clientInfo);
        public delegate void OnClientExpired(string ipToRemove);

        public class State
        {
            public byte[] buffer = new byte[_bufSize];
        }

        public DiscoveryListener(string myIpAddress, int port, OnRecievedClientData onRecievedClientData, OnClientExpired onClientExpiredDel)
        {
            _myIpAddress = myIpAddress;

            // Note, I do not think the port or IP Addres of _listenEndPoint is relevant, any value can be used
            _listenEndPoint = new IPEndPoint(IPAddress.Any, port);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));

            _asyncCallback = ProcessRecievedPacket;
            _delegateMethod = onRecievedClientData;
            _clientExpiredDelMethod = onClientExpiredDel;

            _removeExpiredClientsThread = new Thread(new ThreadStart(this.ClearExpiredIPs));
        }

        public void StartListener()
        {
            _removeExpiredClientsThread.Start();
            _socket.BeginReceiveFrom(_state.buffer, 0, _bufSize, SocketFlags.None, ref _listenEndPoint, _asyncCallback, _state);
        }

        public void StopListener()
        {
            try
            {
                _runThread = false;
                _socket.Close();
                if (_removeExpiredClientsThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    _removeExpiredClientsThread.Join();
                }
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
                        if(_clientIpRecords.Any(a => a.IpAddress == ci.IpAddress) == false)
                        {
                            _clientIpRecords.Add(new IpTimerRecord() { IpAddress = ci.IpAddress, LastPollReceived = DateTime.Now });
                        }
                        else
                        {
                            _clientIpRecords.Single(a => a.IpAddress == ci.IpAddress).LastPollReceived = DateTime.Now;
                        }
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
        
        public void ClearExpiredIPs()
        {
            _removeExpiredClientsPollStopwatch.Start();
            // While loop is on 200 ms poll, but only Process the remove on a longer tick
            while (_runThread)
            {
                if (_removeExpiredClientsPollStopwatch.ElapsedMilliseconds >= _removeExpiredClientsPollDelay)
                {
                    foreach(var currentClient in _clientIpRecords.ToList())
                    {
                        if(currentClient.LastPollReceived.AddSeconds(_removeExpiredClientsSecondsWithoutPollExpire) < DateTime.Now)
                        {
                            _clientIpRecords.Remove(_clientIpRecords.Single(a => a.IpAddress == currentClient.IpAddress));
                            _clientExpiredDelMethod(currentClient.IpAddress);
                        }
                    }
                    _removeExpiredClientsPollStopwatch.Reset();
                    _removeExpiredClientsPollStopwatch.Start();
                }
                Thread.Sleep(200);
            }
        }

        class IpTimerRecord
        {
            public string IpAddress { get; set; }
            public DateTime LastPollReceived { get; set; }
        }
    }
}
