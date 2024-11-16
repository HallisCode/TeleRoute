using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleRoute.Core.Routing.Filters
{
    public class FilterContext
    {
        public Update Update { get; init; }
        
        public ITelegramBotClient TelegramBot { get; init; }

        public FilterContext(Update update, ITelegramBotClient telegramBot)
        {
            Update = update;
            TelegramBot = telegramBot;
        }
    }
}