using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotClientExtended.Types
{
    public static class MessageExtension
    {
        public static bool IsCommand(this Message? message)
        {
            if (message is null ||
                message.Type != MessageType.Text || !message.Text.StartsWith('/')
               )
                return false;

            return true;
        }

        public static string? GetCommand(this Message? message, out string[]? args)
        {
            if (!message.IsCommand())
            {
                args = null;

                return null;
            }

            string[] query = message.Text.Split(' ');

            args = query[1..];

            return query[0];
        }
    }
}