using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Types;

namespace TelegramService.Controllers
{
    [ApiController]
    [Route("/telegram/webhook")]
    public class TelegramController
    {
        private readonly TelegramBotClient _bot;

        public TelegramController(TelegramBotClient bot)
        {
            _bot = bot;
        }

        [HttpPost]
        public async Task HandleUpdate([FromBody] Update update)
        {
            if (update.Message.IsCommand())
            {
                string? command = update.Message.GetCommand(out string[]? commandArgs);

                switch (command)
                {
                    case "/help":
                        await _bot.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Список команд :\n\n" +
                                  "/achievement [command]\n" +
                                  "     * create <name> <count_message> добавляет достижение за количество сообщений\n" +
                                  "     * list список созданных достижений\n" +
                                  "     * delete удалить достижение"
                        );

                        break;

                    case "/achievement":

                        break;

                    default:
                        await _bot.SendTextMessageAsync(
                            chatId: update.Message.Chat.Id,
                            text: "Неизвестная команда, воспользуйтеьс /help.",
                            replyParameters: new ReplyParameters() { ChatId = update.Message.Chat.Id }
                        );

                        break;
                }

                return;
            }
        }
    }
}