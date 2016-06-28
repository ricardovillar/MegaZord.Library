namespace MegaZord.Library.Interfaces
{
    public interface IMZCommandResults
    {
        IMZCommandResult[] Results { get; }

        bool Success { get; }
    }
}

