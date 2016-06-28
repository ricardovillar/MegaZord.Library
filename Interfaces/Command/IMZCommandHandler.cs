namespace MegaZord.Library.Interfaces
{
    public interface IMZCommandHandler<in TCommand>
    {
        IMZCommandResult Execute(TCommand command);
    }
}

