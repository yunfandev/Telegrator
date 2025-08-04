using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Logging;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Represents a set of filters for a handler descriptor, including update and state keeper validators.
    /// </summary>
    public sealed class DescriptorFiltersSet
    {
        /// <summary>
        /// Validator for the update object.
        /// </summary>
        public IFilter<Update>? UpdateValidator { get; set; }

        /// <summary>
        /// Validator for the state keeper.
        /// </summary>
        public IFilter<Update>? StateKeeperValidator { get; set; }

        /// <summary>
        /// Array of update filters.
        /// </summary>
        public IFilter<Update>[]? UpdateFilters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorFiltersSet"/> class.
        /// </summary>
        /// <param name="updateValidator">Validator for the update object.</param>
        /// <param name="stateKeeperValidator">Validator for the state keeper.</param>
        /// <param name="updateFilters">Array of update filters.</param>
        public DescriptorFiltersSet(IFilter<Update>? updateValidator, IFilter<Update>? stateKeeperValidator, IFilter<Update>[]? updateFilters)
        {
            UpdateValidator = updateValidator;
            StateKeeperValidator = stateKeeperValidator;
            UpdateFilters = updateFilters;
        }

        /// <summary>
        /// Validates the filter context using all filters in the set.
        /// </summary>
        /// <param name="filterContext">The filter execution context.</param>
        /// <param name="failedFilter"></param>
        /// <param name="origin"></param>
        /// <returns>True if all filters pass; otherwise, false.</returns>
        public bool Validate(FilterExecutionContext<Update> filterContext, out IFilter<Update> failedFilter, out FilterOrigin origin)
        {
            if (UpdateValidator != null)
            {
                if (!UpdateValidator.CanPass(filterContext))
                {
                    Alligator.LogDebug("(E) UpdateValidator filter of {0} for Update ({1}) didnt pass!", filterContext.Data["handler_name"], filterContext.Update.Id);
                    failedFilter = UpdateValidator;
                    origin = FilterOrigin.Validator;
                    return false;
                }

                //LeveledDebug.FilterWriteLine("UpdateValidator of {0} for Update ({2}) passed", filterContext.Data["handler_name"]);
                filterContext.CompletedFilters.Add(UpdateValidator);
            }

            if (StateKeeperValidator != null)
            {
                if (!StateKeeperValidator.CanPass(filterContext))
                {
                    Alligator.LogDebug("(E) StateKeeperValidator filter of {0} for Update ({1}) didnt pass!", filterContext.Data["handler_name"], filterContext.Update.Id);
                    failedFilter = StateKeeperValidator;
                    origin = FilterOrigin.StateKeeper;
                    return false;
                }

                //LeveledDebug.FilterWriteLine("StateKeeperValidator of {0} for Update ({2}) passed", filterContext.Data["handler_name"]);
                filterContext.CompletedFilters.Add(StateKeeperValidator);
            }

            if (UpdateFilters != null)
            {
                foreach (IFilter<Update> filter in UpdateFilters)
                {
                    if (!filter.CanPass(filterContext))
                    {
                        if (filter is not AnonymousCompiledFilter && filter is not AnonymousTypeFilter)
                            Alligator.LogDebug("(E) {0} filter of {1} for Update ({2}) didnt pass!", filter.GetType().Name, filterContext.Data["handler_name"], filterContext.Update.Id);

                        failedFilter = filter;
                        origin = FilterOrigin.Regualr;
                        return false;
                    }

                    //LeveledDebug.FilterWriteLine("{0} filter of {1} for Update ({2}) passed", filter.GetType().Name, filterContext.Data["handler_name"]);
                    filterContext.CompletedFilters.Add(filter);
                }
            }

            failedFilter = null!;
            origin = FilterOrigin.None;
            return true;
        }
    }
}
