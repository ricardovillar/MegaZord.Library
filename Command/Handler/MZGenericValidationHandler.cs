using System.Collections.Generic;
using MegaZord.Library.Common;
using MegaZord.Library.Interfaces;

namespace MegaZord.Library.Command.Handler
{
    public abstract class MZGenericValidationHandler<TEntity, TIReposity, TCreateUpdateCommand> :
        IMZValidationHandler<TCreateUpdateCommand>
        where TEntity : class, IMZEntity
        where TIReposity : IMZRepository<TEntity>
        where TCreateUpdateCommand : class , IMZCommand
    {
        protected TIReposity CurrentRepository { get; private set; }

        
        protected MZGenericValidationHandler(TIReposity reposity)
        {
            this.CurrentRepository = reposity;
        }

        protected virtual IEnumerable<MZValidationResult> ExecuteValidate(TCreateUpdateCommand command)
        {
            return null;
        }


        public IEnumerable<MZValidationResult> Validate(TCreateUpdateCommand command)
        {
            return ExecuteValidate(command);
        }
    }
}
