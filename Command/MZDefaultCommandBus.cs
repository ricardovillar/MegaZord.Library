using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using MegaZord.Library.Common;
using MegaZord.Library.Exceptions;
using MegaZord.Library.Interfaces;

namespace MegaZord.Library.Command
{
    public class MZDefaultCommandBus : IMZCommandBus
    {
        public IMZCommandResult Submit<TCommand>(TCommand command)
        {
            var handler = DependencyResolver.Current.GetService<IMZCommandHandler<TCommand>>();
            if (handler == null)
            {
                throw new MZCommandHandlerNotFoundException(typeof(TCommand));
            }
            return handler.Execute(command);

        }



        public IEnumerable<MZValidationResult> Validate<TCommand>(TCommand command)
        {
            var handler = DependencyResolver.Current.GetService<IMZValidationHandler<TCommand>>();
            if (handler == null)
            {
                throw new MZValidationHandlerNotFoundException(typeof(TCommand));
            }
            return handler.Validate(command);
        }



    }
}

