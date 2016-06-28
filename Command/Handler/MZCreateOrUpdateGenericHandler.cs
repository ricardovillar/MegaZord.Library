using System;
using AutoMapper;
using MegaZord.Library.Common;
using MegaZord.Library.Interfaces;

namespace MegaZord.Library.Command.Handler
{

    public abstract class MZCreateOrUpdateGenericHandler<TEntity, TIReposity, TCreateUpdateCommand> :
        IMZCommandHandler<TCreateUpdateCommand>
        where TEntity : class, IMZEntity
        where TIReposity : IMZRepository<TEntity>
        where TCreateUpdateCommand : class, IMZCommand
    {

        private readonly TIReposity _reposity;
        private readonly IMZBaseUnitOfWork _unitOfWork;

        protected MZCreateOrUpdateGenericHandler(TIReposity reposity, IMZBaseUnitOfWork unitOfWork)
        {
            this._reposity = reposity;
            this._unitOfWork = unitOfWork;
        }

        public IMZCommandResult Execute(TCreateUpdateCommand command)
        {
            var sucesso = true;
            MZServerError error = null;
            try
            {
                _unitOfWork.StartTrans();
                var entity = GetEntityToSave(command);
                _reposity.Save(entity, true);
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

        protected virtual TEntity GetEntityToSave(TCreateUpdateCommand command)
        {
            return Mapper.Map<TCreateUpdateCommand, TEntity>(command);
        }


    }
}
