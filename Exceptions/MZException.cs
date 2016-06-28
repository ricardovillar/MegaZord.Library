using System;
using System.Runtime.Serialization;

namespace MegaZord.Library.Exceptions
{
    [Serializable]
    public class MZException : Exception, ISerializable
    {
        public MZException(string message)
            : base(message)
        {
        }
    }
}
