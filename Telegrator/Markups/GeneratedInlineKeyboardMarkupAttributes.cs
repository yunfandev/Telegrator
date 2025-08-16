using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegrator.Markups
{
    /// <inheritdoc cref="InlineKeyboardButton.WithCallbackData(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CallbackButtonAttribute(string name, string data) : Attribute
    {
        public string Name { get; } = name;
        public string Data { get; } = data;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithCallbackGame(string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class GameButtonAttribute(string name, string data) : Attribute
    {
        public string Name { get; } = name;
        public string Game { get; } = data;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithCopyText(string, CopyTextButton)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CopyTextButtonAttribute(string name, CopyTextButton copyText) : Attribute
    {
        public string Name { get; } = name;
        public CopyTextButton CopyText { get; } = copyText;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithPay(string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class PayRequestButtonAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithLoginUrl(string, LoginUrl)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class LoginButtonAttribute(string name, LoginUrl url) : Attribute
    {
        public string Name { get; } = name;
        public LoginUrl Url { get; } = url;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithUrl(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class UrlRedirectButtonAttribute(string name, string url) : Attribute
    {
        public string Name { get; } = name;
        public string Url { get; } = url;
    }
     
    /// <inheritdoc cref="InlineKeyboardButton.WithWebApp(string, WebAppInfo)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class WebAppButtonAttribute(string name, WebAppInfo webApp) : Attribute
    {
        public string Name { get; } = name;
        public WebAppInfo AppInfo { get; } = webApp;
    }
     
    /// <inheritdoc cref="InlineKeyboardButton.WithSwitchInlineQuery(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class SwitchQueryButtonAttribute(string name, string switchInlineQuery = "") : Attribute
    {
        public string Name { get; } = name;
        public string Query { get; } = switchInlineQuery;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class QueryCurrentButtonAttribute(string name, string switchInlineQueryCurrentChat = "") : Attribute
    {
        public string Name { get; } = name;
        public string Query { get; } = switchInlineQueryCurrentChat;
    }
}
