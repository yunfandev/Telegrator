using Telegram.Bot.Types;

namespace Telegrator.Filters.Components
{
    /// <summary>
    /// Represents a filter that applies a filter action to an anonymous target type extracted from an update.
    /// </summary>
    public class AnonymousTypeFilter : Filter<Update>
    {
        private readonly Func<FilterExecutionContext<Update>, object, bool> FilterAction;
        private readonly Func<Update, object?> GetFilterringTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousTypeFilter"/> class.
        /// </summary>
        /// <param name="filterAction">The filter action delegate.</param>
        /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
        protected AnonymousTypeFilter(Func<FilterExecutionContext<Update>, object, bool> filterAction, Func<Update, object?> getFilterringTarget)
        {
            FilterAction = filterAction;
            GetFilterringTarget = getFilterringTarget;
        }

        /// <summary>
        /// Compiles a filter for a specific target type.
        /// </summary>
        /// <typeparam name="T">The type of the filtering target.</typeparam>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
        /// <returns>The compiled filter.</returns>
        public static AnonymousTypeFilter Compile<T>(IFilter<T> filter, Func<Update, T?> getFilterringTarget) where T : class
        {
            return new AnonymousTypeFilter(
                (context, filterringTarget) => CanPassInternal(context, filter, filterringTarget),
                getFilterringTarget);
        }

        /// <summary>
        /// Determines whether the filter can pass for the given context and filtering target.
        /// </summary>
        /// <typeparam name="T">The type of the filtering target.</typeparam>
        /// <param name="updateContext">The filter execution context.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="filterringTarget">The filtering target.</param>
        /// <returns>True if the filter passes; otherwise, false.</returns>
        private static bool CanPassInternal<T>(FilterExecutionContext<Update> updateContext, IFilter<T> filter, object filterringTarget) where T : class
        {
            FilterExecutionContext<T> context = updateContext.CreateChild((T)filterringTarget);
            if (!filter.CanPass(context))
            {
                if (filter is not AnonymousCompiledFilter && filter is not AnonymousTypeFilter)
                    Alligator.FilterWriteLine("(E) {0} filter of {1} didnt pass!", filter.GetType().Name, context.Data["handler_name"]);

                return false;
            }

            context.CompletedFilters.Add(filter);
            return true;
        }

        /// <summary>
        /// Determines whether the filter can pass for the given context by extracting the filtering target and applying the filter action.
        /// </summary>
        /// <param name="context">The filter execution context.</param>
        /// <returns>True if the filter passes; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Update> context)
        {
            try
            {
                object? filterringTarget = GetFilterringTarget.Invoke(context.Input);
                if (filterringTarget == null)
                    return false;

                return FilterAction.Invoke(context, filterringTarget);
            }
            catch
            {
                return false;
            }
        }
    }
}
