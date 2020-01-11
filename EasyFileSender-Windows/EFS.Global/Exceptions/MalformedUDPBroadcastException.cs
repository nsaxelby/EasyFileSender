using System;
using System.Runtime.Serialization;

namespace EFS.Global.Exceptions
{
    public class MalformedUDPBroadcastException : Exception
    {
        public MalformedUDPBroadcastException()
        {
        }

        public MalformedUDPBroadcastException(string message) : base(message)
        {
        }

        public MalformedUDPBroadcastException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MalformedUDPBroadcastException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
