using Telegram.Bot.Types.Enums;

namespace Telegrator.Attributes
{
    /// <summary>
    /// Attribute that says if this handler cn await some of await types, that is not listed by its handler base
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MightAwaitAttribute : Attribute
    {
        private readonly UpdateType[] _updateTypes;

        /// <summary>
        /// Update types that may be awaited
        /// </summary>
        public UpdateType[] UpdateTypes => _updateTypes;

        /// <summary>
        /// main ctor of <see cref="MightAwaitAttribute"/>
        /// </summary>
        /// <param name="updateTypes"></param>
        public MightAwaitAttribute(params UpdateType[] updateTypes)
            => _updateTypes = updateTypes;
    }
}
