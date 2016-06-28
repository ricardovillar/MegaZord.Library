using System;
using MegaZord.Library.Common;
using MegaZord.Library.Interfaces;

namespace MegaZord.Library.Command.Handler
{
    public abstract class MZDeleteGenericHandler<TEntity, TIReposity, TDeleteCommand> : IMZCommandHandler<TDeleteCommand>
        where TEntity : class, IMZEntity
        where TIReposity : IMZRepository<TEntity>
        where TDeleteCommand : class , IMZCommand
    {
        private readonly TIReposity _reposity;
        private readonly IMZBaseUnitOfWork _unitOfWork;

        protected MZDeleteGenericHandler(TIReposity reposity, IMZBaseUnitOfWork unitOfWork)
        {
            this._reposity = reposity;
            this._unitOfWork = unitOfWork;
        }

        public IMZCommandResult Execute(TDeleteCommand command)
        {
            var sucesso = true;
            MZServerError error = null;
            try
            {
                _unitOfWork.StartTrans();
                var entity = _reposity.GetById(command.ID);
                _reposity.Delete(entity);
                _unitOfWork.Commit();

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                sucesso = false;
                error = new MZServerError(ex);
            }
            return new MZCommandResult(sucesso, error);
        }
    }
}
