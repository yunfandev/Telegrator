# Getting Started with Telegrator

---

This guide will walk you through the core concepts and features of **Telegrator**.

- [1. Introduction](#1-introduction)
- [2. Key Concepts](#2-key-concepts)
- [3. Installation](#3-installation)
- [4. Your First Bot: A "Hello, World!" Example](#4-your-first-bot-a-hello-world-example)
- [5. Step-by-Step Tutorial](#5-step-by-step-tutorial)
- [6. Advanced Topics](#6-advanced-topics)
- [7. FAQ / Troubleshooting](#7-faq--troubleshooting)
- [8. Links](#8-links)

---

## 1. Introduction

Welcome to **Telegrator** — a modern, aspect-oriented, mediator-based framework for building powerful and maintainable Telegram bots in C#.

- **Why Telegrator?**
  - Decentralized logic: no more monolithic state machines.
  - Flexible filtering and handler composition.
  - Advanced state management and concurrency control.
  - Inspired by AOP, but tailored for practical bot development.

---

## 2. Key Concepts

At the core of **Telegrator** are a few fundamental concepts that enable its power and flexibility. Understanding them is key to building robust and scalable bots.

### Mediator: The Central Dispatcher
The framework is built around the **Mediator pattern**. Every incoming update from Telegram (like a message, a button click, or a user joining a chat) is first received by a central `UpdateRouter`. This router acts as a mediator, responsible for dispatching the update to the appropriate handlers. This decouples the update receiving logic from the processing logic, making the system clean and maintainable.

### Handlers: The Logic Processors
A **Handler** is a class that contains the logic for processing a specific type of update. For example, you might have a `WelcomeHandler` for new chat members, a `CommandHandler` for slash commands, or a `CallbackQueryHandler` for button clicks. Each handler is a small, focused, and reusable component.

*   **`MessageHandler`**: For processing text messages.
*   **`CallbackQueryHandler`**: For handling button clicks from `InlineKeyboardMarkup`.
*   **`AnyUpdateHandler`**: A catch-all handler for any type of update.

### Filters: The Gatekeepers
**Filters** are attributes that you apply to your handler classes to specify *when* a handler should be executed. They act as gatekeepers, ensuring that a handler only runs if the incoming update meets certain criteria.

- **`[Command("/start")]`**: Triggers the handler only for the `/start` command.
- **`[MessageText(Contains = "hello")]`**: Triggers when a message contains the word "hello".
- **`[MessageChat(Is = ChatType.Private)]`**: Triggers only for messages from private chats.
- **`[RepliedMessage]`**: Triggers only when the message is a reply to another message.

Filters can be combined with logical operators (`OrNext`, `Not`) to create complex and precise routing rules without writing messy `if/else` statements.

### State Management: The Memory
**State Management** in Telegrator is a powerful feature for creating multi-step conversations (wizards, forms, quizzes) without a database. It works by using special **filter attributes** that make handlers execute only when a user or chat is in a specific state.

The core idea is that you don't interact with a "StateKeeper" object directly. Instead:
1.  You define a state machine using a C# `enum` or simple `int` values.
2.  You use `[EnumState(YourEnum.SomeState)]` or `[NumericState(1)]` attributes to filter which handler should run at which step.
3.  Inside a handler, you use extension methods on the `IAbstractHandlerContainer<T>` (which I'll call `container` for simplicity) to change the user's state, e.g., `container.ForwardEnumState<YourEnum>()`.

#### Example: A Multi-Step Quiz

Let's build a simple two-question quiz.

**1. Define the Quiz States:**

```csharp
// Enums/QuizState.cs
public enum QuizState
{
    // We use the SpecialState enum to represent the "no state" condition.
    // This is the default state for all users.
    Start = SpecialState.NoState,
    ExpectingAnswer1,
    ExpectingAnswer2
}
```

**2. Build the Handlers:**

The `/quiz` command will start the process. It will only trigger for users who are not currently in a quiz (`QuizState.Start`).

```csharp
// Handlers/QuizHandler.cs
using Telegrator.Annotations.StateKeeping;

[MessageHandler]
[CommandAllias("quiz")]
// This handler only runs if the user's state for QuizState is the default (NoState).
[EnumState<QuizState>(QuizState.Start)]
public class StartQuizHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        // Create the state for the user and move it to the first question.
        container.ForwardNumericState<QuizState>(); // If state isnt created, creates default state and formards its value. QuizState.Start -> QuizState.ExpectingAnswer1
        await Reply("Quiz started! Question 1: What is the capital of France?");
    }
}
```

Now, create handlers for each expected answer. They will only trigger if the user is in the correct state.

```csharp
// Handlers/QuizAnswerHandlers.cs

[MessageHandler]
// This handler only runs if the user's state is ExpectingAnswer1.
[EnumState<QuizState>(QuizState.ExpectingAnswer1)]
public class Answer1Handler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input.Text.Trim().Equals("Paris", StringComparison.InvariantCultureIgnoreCase))
        {
            await Reply("Correct!");
        }
        else
        {
            await Reply("Incorrect. The answer is Paris.");
        }

        // Move to the next state.
        container.ForwardEnumState<QuizState>(); // Moves state to ExpectingAnswer2
        await Reply("Question 2: What is 2 + 2?");
    }
}

[MessageHandler]
// This handler only runs if the user's state is ExpectingAnswer2.
[EnumState<QuizState>(QuizState.ExpectingAnswer2)]
public class Answer2Handler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input.Text.Trim() == "4")
        {
            await Reply("Correct!");
        }
        else
        {
            await Reply("Incorrect. The answer is 4.");
        }

        // The quiz is over, so we delete the user's state.
        container.DeleteEnumState<QuizState>();
        await Reply("Quiz finished!");
    }
}
```

**How it Works:**
- **`[EnumState(QuizState.Start)]`**: This is a **filter**. It checks the user's current state for the `QuizState` enum. `SpecialState.NoState` is a conventional way to say "the user has no state set yet".
- **`container.CreateEnumState<QuizState>()`**: This initializes the state for the current user (or chat, depending on the key resolver) and sets it to the first actual value of the enum (`Start`).
- **`container.ForwardEnumState<QuizState>()`**: This moves the user's state to the next value in the enum sequence.
- **`container.DeleteEnumState<QuizState>()`**: This removes the state for the user, effectively resetting them to `SpecialState.NoState`.

This declarative, attribute-based approach keeps your handler logic clean and focused on a single task, while the framework manages the complexity of the state machine.

---

## 3. Installation

**Telegrator** is distributed as a NuGet package. You can install it using the .NET CLI, the NuGet Package Manager Console, or by managing NuGet packages in Visual Studio.

### Prerequisites
- .NET 6.0 SDK or later.
- A Telegram Bot Token from [@BotFather](https://t.me/BotFather).

### .NET CLI

```shell
dotnet add package Telegrator
```

### Package Manager Console

```shell
Install-Package Telegrator
```

The framework also has integrations for different hosting models, which can be installed separately:

- **`Telegrator.Hosting`**: For console applications and background services.
- **`Telegrator.Hosting.Web`**: For ASP.NET Core applications and Webhook support.

```shell
# For console apps
dotnet add package Telegrator.Hosting

# For ASP.NET Core apps
dotnet add package Telegrator.Hosting.Web
```

---

## 4. Your First Bot: A "Hello, World!" Example

Let's create a simple bot that replies with "Hello, {FirstName}!" when a user sends the `/start` command.

### 1. Create the Handler

First, create a new class that inherits from `MessageHandler`. This class will contain the logic for handling the message.

```csharp
// Handlers/StartHandler.cs
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegrator.Handlers;
using Telegrator.Annotations;

namespace Handlers;

// This attribute registers the class as a message handler.
[MessageHandler]
// This filter ensures that the message's text equals to "Hello".
[TextEquals("Hello")]
public class StartHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        // Get the user's first name from the incoming message.
        var firstName = Input.From?.FirstName ?? "User";
        
        // Reply to the user.
        await Reply($"Hello, {firstName}!", cancellationToken: cancellation);
    }
}
```

### 2. Set Up and Run the Bot

Next, in your application's entry point (e.g., `Program.cs`), create an instance of `ReactiveClient`, add your handler, and start listening for updates.

```csharp
// Program.cs
using System;
using System.Threading;
using Telegrator;
using Handlers; // Assuming your handler is in the "Handlers" namespace

class Program
{
    static void Main(string[] args)
    {
        // Replace "<YOUR_BOT_TOKEN>" with your actual bot token.
        var bot = new ReactiveClient("<YOUR_BOT_TOKEN>");

        // Automatically discover and add all **public** handlers from the current assembly.
        bot.Handlers.CollectHandlersDomainWide();
        
        // Or, add a specific handler manually:
        // bot.Handlers.AddHandler<StartHandler>();

        // Start receiving updates from Telegram.
        bot.StartReceiving();

        Console.WriteLine("Bot started. Press any key to exit.");
        Console.ReadKey();
    }
}
```

### What's Happening?
1.  **`[MessageHandler]`**: This attribute marks `StartHandler` as a handler for `Message` updates.
2.  **`[TextEquals("Hello")]`**: This is a **Filter**. It tells the `UpdateRouter` to only execute this handler if the message text is equals `Hello`.
3.  **`ReactiveClient`**: This is the main bot client. It manages the connection to Telegram and the update processing pipeline.
4.  **`bot.Handlers.CollectHandlersDomainWide()`**: This convenient extension method scans your project for all classes marked with handler attributes (like `[MessageHandler]`) and registers them automatically.
5.  **`bot.StartReceiving()`**: This method starts the long-polling loop to fetch updates from Telegram and passes them to the `UpdateRouter`.
6.  **`Reply(...)`**: This is a helper method from the base `MessageHandler` class that simplifies sending a reply to the original message.

---

## 5. Step-by-Step Tutorial

This tutorial will guide you through building a bot that demonstrates common features like command handling, filtering, and waiting for user input.

### 5.1 The Problem: An Overly Eager Echo Bot

Let's start with two simple handlers: one for the `/start` command and one to echo messages.

```csharp
// StartHandler.cs
[CommandHandler]
[CommandAllias("start")]
public class StartHandler : CommandHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Welcome! Send me any message and I will echo it back.");
    }
}

// EchoHandler.cs
[MessageHandler]
public class EchoHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply($"You said: {Input.Text}");
    }
}
```

If you run this bot and send `/start`, you'll get two replies!
1.  "Welcome! Send me any message and I will echo it back." (from `StartHandler`)
2.  "You said: /start" (from `EchoHandler`)

This happens because `EchoHandler` has no filters, so it triggers for *every* message, including the `/start` command.

### 5.2 The Traditional Fix: `if` Statements

The classic way to solve this is to add a check inside the `EchoHandler`.

```csharp
// EchoHandler.cs (with an if-statement)
[MessageHandler]
public class EchoHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        // Manually ignore commands.
        if (Input.Text.StartsWith("/"))
        {
            return;
        }

        await Reply($"You said: {Input.Text}");
    }
}
```
This works, but it clutters our handler with boilerplate logic. As you add more commands and conditions, this approach becomes messy.

### 5.3 The Reactive Fix: Declarative Filters

**Telegrator** lets you solve this cleanly using filter attributes. We can tell the `EchoHandler` to only trigger if the message is *not* a command.

```csharp
// EchoHandler.cs (the reactive way)
using Telegrator.Attributes; // For FilterModifier

[MessageHandler]
// This filter ensures the handler only runs for messages that are NOT the command.
[TextStartsWith("/", Modifiers = FilterModifier.Not)]
public class EchoHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply($"You said: {Input.Text}");
    }
}
```
Now, the `EchoHandler` will correctly ignore the any command without any `if` statements in the handler body. The routing logic is declared right where it belongs: on the class itself.

### 5.4 Waiting for Input with `AwaitingProvider`

What if you want to ask a question and wait for the user's *next* message? You could use the state management system, but for a simple one-off question, that's overkill. The `AwaitingProvider` is perfect for this.

Let's create a `/question` command that asks for the user's name and then greets them with the name they provide.

```csharp
// QuestionHandler.cs
[CommandHandler]
[CommandAllias("question")]
public class QuestionHandler : CommandHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("What is your name?");

        // Await the user's next message.
        // We apply a filter to ensure we only catch messages from the same user in the same chat.
        var nextMessage = await Container.AwaitMessage().BySenderId(cancellation);
        await Client.SendMessage(Input.Chat, $"Hello, {nextMessage.Text}!");
    }
}
```

**How it works:**
1.  The handler first sends the question "What is your name?".
2.  `AwaitMessage()` temporarily registers an internal handler that waits for the next `Message` sended by this user.

This pattern is extremely powerful for creating dynamic, interactive conversations without the complexity of a full state machine.

---

## 6. Advanced Topics

This section covers more advanced features of the framework.

### Handler Concurrency and Priority
By default, handlers are executed in the order they are added. However, you can control the execution order using the `Priority` property in the handler attribute. A Greater number means higher priority.

```csharp
[MessageHandler(Priority = 1)] // This will run before handlers with the default priority (0)
public class HighPriorityHandler : MessageHandler
{
    // ...
}
```

### Creating Custom Filters
You can create your own filters by inheriting from `UpdateFilterAttribute<T>`. This is useful for encapsulating complex or reusable filtering logic.

Let's create a filter that only allows messages from a specific user ID.

```csharp
// Filters/AdminOnlyFilter.cs
using Telegram.Bot.Types;
using Telegrator.Attributes;

public class AdminOnlyAttribute : UpdateFilterAttribute<Message>
{
    private readonly long _adminId;

    public AdminOnlyAttribute(long adminId)
    {
        _adminId = adminId;
    }

    public override Message? GetFilterringTarget(Update update) => update.Message;

    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        return context.Input.From?.Id == _adminId;
    }
}

// Usage in a handler:
[MessageHandler]
[AdminOnly(123456789)] // Replace with your actual admin ID
public class AdminCommandHandler : MessageHandler
{
    // ...
}
```


### Integration with Dependency Injection
**Telegrator** is designed to work seamlessly with dependency injection (DI) containers like the one built into ASP.NET Core.

When using the `Telegrator.Hosting` or `Telegrator.Hosting.Web` packages, handlers and their dependencies are automatically registered with the DI container. This means you can inject any registered service directly into your handler's constructor.

```csharp
[MessageHandler]
public class MyHandler : MessageHandler
{
    private readonly IMyService _myService;
    private readonly ILogger<MyHandler> _logger;

    // Dependencies are injected automatically.
    public MyHandler(IMyService myService, ILogger<MyHandler> logger)
    {
        _myService = myService;
        _logger = logger;
    }

    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        _logger.LogInformation("MyHandler executed!");
        var result = _myService.DoSomething();
        await Reply(result);
    }
}
```

### Concurrency Control
You can limit the number of concurrent executions for a specific handler by setting the `concurrency` parameter in the handler attribute. This is useful for preventing race conditions or for managing resource-intensive operations.

```csharp
// This handler will only allow one execution at a time for a given chat.
[MessageHandler(concurrency: 1)]
public class SlowHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Task.Delay(5000, cancellation); // Simulate a long-running operation
        await Reply("Done!");
    }
}
```

---

## 7. FAQ / Troubleshooting

### Q: My handler is not being triggered. What should I do?
- **Check Handler Registration**: Ensure you are calling `bot.Handlers.AddHandlers()` or `bot.Handlers.AddHandler<MyHandler>()`. If you are using DI, make sure the assembly containing your handlers is being scanned.
- **Check Filters**: Double-check your filter attributes. A common mistake is a typo in a command or text filter. Remember that filters are combined with a logical AND by default. If you have multiple filters, the update must pass all of them.
- **Check Update Type**: Make sure your handler is for the correct update type. A `MessageHandler` will not be triggered by a `CallbackQuery` update.
- **Enable Debug Logging**: You can subscribe to the `UpdateRouter.OnUpdate` and `UpdateRouter.OnHandlerEnter` events to see how updates are being processed in real-time.

### Q: How can I access the `ITelegramBotClient` or the original `Update` object inside a handler?
The base `AbstractUpdateHandler<T>` class provides access to these:
- **`Client`**: The `ITelegramBotClient` instance.
- **`Update`**: The raw `Update` object.
- **`Input`**: The specific update payload (e.g., `Message`, `CallbackQuery`).

### Q: How do I handle errors?
You can subscribe to the `UpdateRouter.OnError` event to receive notifications about exceptions that occur during update processing. This is a good place to log errors or send notifications to an administrator.

```csharp
bot.UpdateRouter.OnError += (sender, args) =>
{
    Console.WriteLine($"An error occurred: {args.Exception.Message}");
    return Task.CompletedTask;
};
```

### Q: How can I organize my code for a large bot?
- **Folders**: Organize your handlers, filters, and state keepers into separate folders (e.g., `Handlers/Commands`, `Handlers/Callbacks`, `StateKeepers`).
- **Feature Modules**: For very large bots, consider structuring your code into "feature modules", where each module is a separate class library containing all the related handlers, filters, and services for a specific feature.

---

## 8. Links

- [API Reference](./TelegramReactive_Api.md)
- [Main Repository](https://github.com/Rikitav/Telegrator)
- [Wiki & Examples](https://github.com/Rikitav/Telegrator/wiki/)

---

> **Feel free to contribute, ask questions, or open issues!** 