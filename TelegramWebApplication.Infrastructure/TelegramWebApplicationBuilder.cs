using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleNetFramework.Core;
using SimpleNetFramework.Core.Server;
using Telegram.Bot.Types;

namespace TelegramWebApplication.Infrastructure
{
    public class TelegramWebApplicationBuilder : IWebApplicationBuilder<TelegramWebApplication>
    {
        public IServiceCollection Services { get; protected set; } = new ServiceCollection();
        public IServer? Server { get; protected set; }


        public void SetServer(IServer server)
        {
            Server = server;
        }

        public TelegramWebApplication Build()
        {
            // Добавляем ServiceProvider
            IServiceProvider serviceProvider = Services.BuildServiceProvider();
            Services.AddSingleton<IServiceProvider>(serviceProvider);

            if (Server is null)
            {
                throw new Exception($"Не настроен {nameof(Server)}, необходимо вызвать {nameof(SetServer)}.");
            }

            TelegramWebApplication application = new TelegramWebApplication(Server, Services.BuildServiceProvider());

            Services.Clear();
            Server = null;

            return application;
        }
    }
}