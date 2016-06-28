using System.Collections.Generic;
using MegaZord.Library.Common;

namespace MegaZord.Library.Interfaces
{
    public interface IMZValidationHandler<TCommand>
    {
        IEnumerable<MZValidationResult>  Validate(TCommand command);
    }
}
