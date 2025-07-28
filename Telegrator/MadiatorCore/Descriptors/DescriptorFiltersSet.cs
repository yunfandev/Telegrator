using Telegram.Bot.Types;
using Telegrator.Filters.Components;

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
        /// <returns>True if all filters pass; otherwise, false.</returns>
        public bool Validate(FilterExecutionContext<Update> filterContext)
        {
            if (UpdateValidator != null)
            {
                if (!UpdateValidator.CanPass(filterContext))
                {
                    LeveledDebug.FilterWriteLine("(E) UpdateValidator filter of {0} for Update ({1}) didnt pass!", filterContext.Data["handler_name"], filterContext.Update.Id);
                    return false;
                }

                //LeveledDebug.FilterWriteLine("UpdateValidator of {0} for Update ({2}) passed", filterContext.Data["handler_name"]);
                filterContext.CompletedFilters.Add(UpdateValidator);
            }

            if (StateKeeperValidator != null)
            {
                if (!StateKeeperValidator.CanPass(filterContext))
                {
                    LeveledDebug.FilterWriteLine("(E) StateKeeperValidator filter of {0} for Update ({1}) didnt pass!", filterContext.Data["handler_name"], filterContext.Update.Id);
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
                            LeveledDebug.FilterWriteLine("(E) {0} filter of {1} for Update ({2}) didnt pass!", filter.GetType().Name, filterContext.Data["handler_name"], filterContext.Update.Id);

                        return false;
                    }

                    //LeveledDebug.FilterWriteLine("{0} filter of {1} for Update ({2}) passed", filter.GetType().Name, filterContext.Data["handler_name"]);
                    filterContext.CompletedFilters.Add(filter);
                }
            }

            return true;
        }
    }
}
