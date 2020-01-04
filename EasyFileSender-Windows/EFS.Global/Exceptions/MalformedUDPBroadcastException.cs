using System;

namespace EFS.Global.Exceptions
{
    public class MalformedUDPBroadcastException : Exception
    {
        public MalformedUDPBroadcastException(string message) : base(message)
        {
        }
    }
}
