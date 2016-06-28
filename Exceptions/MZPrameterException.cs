using System;

namespace MegaZord.Library.Exceptions
{
    [Serializable]
    public class MZManyPrameterException : MZException
    {
        public MZManyPrameterException(string parameterName)
            : base(string.Format("Foi encontrado mais de um parâmetro com a chave '{0}.'", parameterName))
        {
        }
    }

    [Serializable]
    public class MZZeroPrameterException : MZException
    {
        public MZZeroPrameterException(string parameterName)
            : base(string.Format("Não Foi encontrado um parâmetro com a chave '{0}.'", parameterName))
        {
        }
    }
}



