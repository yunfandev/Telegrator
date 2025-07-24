using Telegrator.StateKeeping;
using Telegrator.Attributes;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Annotations.StateKeeping
{
    /// <summary>
    /// Attribute for associating a handler or method with a string-based state keeper.
    /// Provides various constructors for flexible state and key resolver configuration.
    /// </summary>
    public class StringStateAttribute : StateKeeperAttribute<long, string, StringStateKeeper>
    {
        /// <summary>
        /// Initializes the attribute with a special state and a custom key resolver.
        /// </summary>
        /// <param name="specialState">The special state to associate</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        public StringStateAttribute(SpecialState specialState, IStateKeyResolver<long> keyResolver)
            : base(specialState, keyResolver) { }

        /// <summary>
        /// Initializes the attribute with a specific state and a custom key resolver.
        /// </summary>
        /// <param name="myState">The string state to associate</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        public StringStateAttribute(string myState, IStateKeyResolver<long> keyResolver)
            : base(myState, keyResolver) { }

        /// <summary>
        /// Initializes the attribute with a special state and the default sender ID resolver.
        /// </summary>
        /// <param name="specialState">The special state to associate</param>
        public StringStateAttribute(SpecialState specialState)
            : base(specialState, new SenderIdResolver()) { }

        /// <summary>
        /// Initializes the attribute with a specific state and the default sender ID resolver.
        /// </summary>
        /// <param name="myState">The string state to associate</param>
        public StringStateAttribute(string myState)
            : base(myState, new SenderIdResolver()) { }

        /// <summary>
        /// Initializes the attribute with a specific state, a custom key resolver, and a set of possible states.
        /// </summary>
        /// <param name="myState">The string state to associate</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        /// <param name="states">The set of possible string states</param>
        public StringStateAttribute(string myState, IStateKeyResolver<long> keyResolver, params string[] states)
            : base(new StringStateKeeper(states), myState, keyResolver) { }

        /// <summary>
        /// Initializes the attribute with a special state, a custom key resolver, and a set of possible states.
        /// </summary>
        /// <param name="specialState">The special state to associate</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        /// <param name="states">The set of possible string states</param>
        public StringStateAttribute(SpecialState specialState, IStateKeyResolver<long> keyResolver, params string[] states)
            : base(new StringStateKeeper(states), specialState, keyResolver) { }

        /// <summary>
        /// Initializes the attribute with a specific state, the default sender ID resolver, and a set of possible states.
        /// </summary>
        /// <param name="myState">The string state to associate</param>
        /// <param name="states">The set of possible string states</param>
        public StringStateAttribute(string myState, params string[] states)
            : base(new StringStateKeeper(states), myState, new SenderIdResolver()) { }

        /// <summary>
        /// Initializes the attribute with a special state, the default sender ID resolver, and a set of possible states.
        /// </summary>
        /// <param name="specialState">The special state to associate</param>
        /// <param name="states">The set of possible string states</param>
        public StringStateAttribute(SpecialState specialState, params string[] states)
            : base(new StringStateKeeper(states), specialState, new SenderIdResolver()) { }
    }
}
