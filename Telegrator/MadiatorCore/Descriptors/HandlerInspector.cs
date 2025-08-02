using System.ComponentModel;
using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Aspects;
using Telegrator.Attributes.Components;
using Telegrator.Filters.Components;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Provides methods for inspecting handler types and retrieving their attributes and filters.
    /// </summary>
    public static class HandlerInspector
    {
        /// <summary>
        /// Gets handler's display name
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public static string? GetDisplayName(MemberInfo handlerType)
        {
            return handlerType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        }

        /// <summary>
        /// Gets the handler attribute from the specified member info.
        /// </summary>
        /// <param name="handlerType">The member info representing the handler type.</param>
        /// <returns>The handler attribute.</returns>
        public static UpdateHandlerAttributeBase GetHandlerAttribute(MemberInfo handlerType)
        {
            // Getting polling handler attribute
            IEnumerable<UpdateHandlerAttributeBase> handlerAttrs = handlerType.GetCustomAttributes<UpdateHandlerAttributeBase>();

            //
            return handlerAttrs.Single();
        }

        /// <summary>
        /// Gets the state keeper attribute from the specified member info, if present.
        /// </summary>
        /// <param name="handlerType">The member info representing the handler type.</param>
        /// <returns>The state keeper attribute, or null if not present.</returns>
        public static StateKeeperAttributeBase? GetStateKeeperAttribute(MemberInfo handlerType)
        {
            // Getting polling handler attribute
            IEnumerable<StateKeeperAttributeBase> handlerAttrs = handlerType.GetCustomAttributes<StateKeeperAttributeBase>();

            //
            return handlerAttrs.Any() ? handlerAttrs.Single() : null;
        }

        /// <summary>
        /// Gets all filter attributes for the specified handler type and update type.
        /// </summary>
        /// <param name="handlerType">The member info representing the handler type.</param>
        /// <param name="validUpdType">The valid update type.</param>
        /// <returns>An enumerable of filter attributes.</returns>
        public static IEnumerable<IFilter<Update>> GetFilterAttributes(MemberInfo handlerType, UpdateType validUpdType)
        {
            //
            IEnumerable<UpdateFilterAttributeBase> filters = handlerType.GetCustomAttributes<UpdateFilterAttributeBase>();

            //
            if (filters.Any(filterAttr => !filterAttr.AllowedTypes.Contains(validUpdType)))
                throw new InvalidOperationException();

            UpdateFilterAttributeBase? lastFilterAttribute = null;
            foreach (UpdateFilterAttributeBase filterAttribute in filters)
            {
                if (!filterAttribute.ProcessModifiers(lastFilterAttribute))
                {
                    lastFilterAttribute = null;
                    yield return filterAttribute.AnonymousFilter;
                }
                else
                {
                    lastFilterAttribute = filterAttribute;
                    continue;
                }
            }
        }

        public static DescriptorAspectsSet GetAspects(Type handlerType)
        {
            bool selfPre = handlerType.GetInterface(nameof(IPreProcessor)) != null;
            bool selfPost = handlerType.GetInterface(nameof(IPostProcessor)) != null;
            Type? typedPre = null;
            Type? typedPost = null;

            if (!selfPre)
            {
                Attribute? preAttr = handlerType.GetCustomAttribute(typeof(BeforeExecutionAttribute<>));
                if (preAttr != null)
                    typedPre = preAttr.GetType().GetGenericArguments()[0];
            }

            if (!selfPost)
            {
                Attribute? postAttr = handlerType.GetCustomAttribute(typeof(AfterExecutionAttribute<>));
                if (postAttr != null)
                    typedPre = postAttr.GetType().GetGenericArguments()[0];
            }

            return new DescriptorAspectsSet(selfPre, typedPre, selfPost, typedPost);
        }
    }
}
