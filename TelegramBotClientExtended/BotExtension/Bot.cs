using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotClientExtended.Bot
{
    public static class BotExtension
    {
        public static Task<Message> ReplyAsync(this ITelegramBotClient botClient, Update update)
        {
            throw new NotImplementedException();
        }
    }
}