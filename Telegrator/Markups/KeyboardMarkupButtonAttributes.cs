using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegrator.Markups
{
    /// <inheritdoc cref="InlineKeyboardButton.WithCallbackData(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CallbackButtonAttribute(string name, string data) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Data that will be send to bot
        /// </summary>
        public string Data { get; } = data;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithCallbackGame(string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class GameButtonAttribute(string name, string data) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Game to open
        /// </summary>
        public string Game { get; } = data;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithCopyText(string, CopyTextButton)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CopyTextButtonAttribute(string name, CopyTextButton copyText) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Text to copy
        /// </summary>
        public CopyTextButton CopyText { get; } = copyText;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithPay(string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class PayRequestButtonAttribute(string name) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithLoginUrl(string, LoginUrl)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class LoginButtonAttribute(string name, LoginUrl url) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Url of app to login to
        /// </summary>
        public LoginUrl Url { get; } = url;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithUrl(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class UrlRedirectButtonAttribute(string name, string url) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Url to redirect user
        /// </summary>
        public string Url { get; } = url;
    }
     
    /// <inheritdoc cref="InlineKeyboardButton.WithWebApp(string, WebAppInfo)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class WebAppButtonAttribute(string name, WebAppInfo webApp) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Info about mini app to open
        /// </summary>
        public WebAppInfo AppInfo { get; } = webApp;
    }
     
    /// <inheritdoc cref="InlineKeyboardButton.WithSwitchInlineQuery(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class SwitchQueryButtonAttribute(string name, string switchInlineQuery = "") : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;
        
        /// <summary>
        /// Query
        /// </summary>
        public string Query { get; } = switchInlineQuery;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class QueryCurrentButtonAttribute(string name, string switchInlineQueryCurrentChat = "") : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Query
        /// </summary>
        public string Query { get; } = switchInlineQueryCurrentChat;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestChatButtonAttribute(string name, int requestId, bool chatIsChannel) : Attribute
    {
        public string Name { get; } = name;

        public int RequestId { get; } = requestId;

        public bool ChatIsChannel { get; } = chatIsChannel;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestContactButtonAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestLocationButtonAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestPoolButtonAttribute(string name, KeyboardButtonPollType requestPoll) : Attribute
    {
        public string Name { get; } = name;

        public KeyboardButtonPollType PollType { get; } = requestPoll;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestUsersButtonAttribute(string name, int requestId, int? maxQuantity = null) : Attribute
    {
        public string Name { get; } = name;

        public int RequestId { get; } = requestId;

        public int? MaxQuantity { get; } = maxQuantity;
    }
}
