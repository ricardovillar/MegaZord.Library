using System.Collections.Generic;
using MegaZord.Library.Common;

namespace MegaZord.Library.Interfaces
{
    public interface IMZCommandBus
    {
        IMZCommandResult Submit<TCommand>(TCommand command);
        IEnumerable<MZValidationResult> Validate<TCommand>(TCommand command);


    }
}

