using MegaZord.Library.Common;
using MegaZord.Library.Interfaces;

namespace MegaZord.Library.Command
{
    public class MZCommandResult : IMZCommandResult
    {
        public MZCommandResult(bool success, MZServerError error)
        {
            this.Success = success;
            this.Error = error;
        }

        public MZServerError Error { get; protected set; }
        public bool Success { get; protected set; }
    }
}

