using System;
using System.Collections.Generic;
using System.Text;
using Telegrator.Handlers;
using Telegrator.Handlers.Components;

namespace Telegrator.Aspects
{
    /// <summary>
    /// Interface for pre-execution processors that are executed before handler execution.
    /// Implement this interface to add cross-cutting concerns like validation, logging, or authorization.
    /// </summary>
    public interface IPreProcessor
    {
        /// <summary>
        /// Executes before the handler's main execution logic.
        /// </summary>
        /// <param name="container">The handler container containing the current update and context.</param>
        /// <returns>A <see cref="Result"/> indicating whether execution should continue or be stopped.</returns>
        public Task<Result> BeforeExecution(IHandlerContainer container);
    }
}
