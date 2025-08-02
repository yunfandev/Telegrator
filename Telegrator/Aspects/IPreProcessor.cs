using System;
using System.Collections.Generic;
using System.Text;
using Telegrator.Handlers;
using Telegrator.Handlers.Components;

namespace Telegrator.Aspects
{
    public interface IPreProcessor
    {
        public Task<Result> BeforeExecution(IHandlerContainer container);
    }
}
