using Telegrator.Hosting;
using Telegrator.Hosting.Web;

namespace Telegrator.ConsoleHost.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TelegramBotWebHostBuilder builder = TelegramBotWebHost.CreateBuilder(new TelegramBotWebOptions()
            {
                Args = args,
                WebhookUri = "https://telegrator-hooker.cloudpub.ru/bot",
                DescendDescriptorIndex = false,
                ExceptIntersectingCommandAliases = true,
            });

            builder.Handlers.CollectHandlersAssemblyWide();
            
            TelegramBotWebHost telegramBot = builder.Build();
            telegramBot.SetBotCommands();
            telegramBot.Run();
        }
    }
}
