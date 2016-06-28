using System;

namespace MegaZord.Library.Exceptions
{
    [Serializable]
    public class MZCommandHandlerNotFoundException : MZException
    {
        public MZCommandHandlerNotFoundException(Type type)
            : base(string.Format("Command handler not found for command type: {0}", type))
        {
        }
    }
}

