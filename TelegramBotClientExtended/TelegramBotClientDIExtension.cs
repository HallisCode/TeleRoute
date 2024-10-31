using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace TelegramBotClientExtended
{
    public static class TelegramBotClientDIExtension
    {
        private const string _tokenKey = "TelegramBotToken";

        /// <summary>
        /// Добавляет в сервисы <see cref="TelegramBotClient"/> на основе токен ключа.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Токен не задан конфигурацией.</exception>
        public static IServiceCollection AddTelegramBotClient(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            string? token = configuration[_tokenKey];

            if (token is null)
            {
                throw new NullReferenceException($"--> {_tokenKey} is missing in configuration.");
            }

            TelegramBotClient bot = new TelegramBotClient(token);

            services.AddSingleton(serviceProvider => bot);

            return services;
        }
    }
}