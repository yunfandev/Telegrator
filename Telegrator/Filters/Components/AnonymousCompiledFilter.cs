using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegrator.Filters.Components
{
    /// <summary>
    /// Represents a compiled filter that applies a set of filters to an anonymous target type.
    /// </summary>
    public sealed class AnonymousCompiledFilter : AnonymousTypeFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCompiledFilter"/> class.
        /// </summary>
        /// <param name="filterAction">The filter action delegate.</param>
        /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
        private AnonymousCompiledFilter(Func<FilterExecutionContext<Update>, object, bool> filterAction, Func<Update, object?> getFilterringTarget)
            : base(filterAction, getFilterringTarget) { }

        /// <summary>
        /// Compiles a set of filters into an <see cref="AnonymousCompiledFilter"/> for a specific target type.
        /// </summary>
        /// <typeparam name="T">The type of the filtering target.</typeparam>
        /// <param name="filters">The list of filters to compile.</param>
        /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
        /// <returns>The compiled filter.</returns>
        public static AnonymousCompiledFilter Compile<T>(IList<IFilter<T>> filters, Func<Update, object?> getFilterringTarget) where T : class
        {
            return new AnonymousCompiledFilter(
                (context, filterringTarget) => CanPassInternal(filters, context, filterringTarget),
                getFilterringTarget);
        }

        /// <summary>
        /// Determines whether all filters can pass for the given context and filtering target.
        /// </summary>
        /// <typeparam name="T">The type of the filtering target.</typeparam>
        /// <param name="filters">The list of filters.</param>
        /// <param name="updateContext">The filter execution context.</param>
        /// <param name="filterringTarget">The filtering target.</param>
        /// <returns>True if all filters pass; otherwise, false.</returns>
        private static bool CanPassInternal<T>(IList<IFilter<T>> filters, FilterExecutionContext<Update> updateContext, object filterringTarget) where T : class
        {
            FilterExecutionContext<T> context = updateContext.CreateChild((T)filterringTarget);

            foreach (IFilter<T> filter in filters)
            {
                if (!filter.CanPass(context))
                {
                    if (filter is not AnonymousCompiledFilter && filter is not AnonymousTypeFilter)
                        Alligator.FilterWriteLine("(E) {0} filter of {1} didnt pass!", filter.GetType().Name, context.Data["handler_name"]);
                
                    return false;
                }

                context.CompletedFilters.Add(filter);
            }

            return true;
        }
    }
}
