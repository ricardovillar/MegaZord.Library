using System;

namespace MegaZord.Library.Exceptions
{
    [Serializable]
    public class MZValidationHandlerNotFoundException : MZException
    {
         public MZValidationHandlerNotFoundException(Type type)
            : base(string.Format("Validation handler not found for command type: {0}", type))
        {
        }
    }
}
