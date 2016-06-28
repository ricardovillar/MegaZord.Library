using MegaZord.Library.Common;

namespace MegaZord.Library.Interfaces
{
    public interface IMZCommandResult
    {
        bool Success { get; }
        MZServerError Error { get;  }
    }
}

