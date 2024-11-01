using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing;
using TelegramBotClientExtended.Types;

namespace TelegramService.Controllers
{
    [ApiController]
    [Route("/telegram/webhook")]
    public class TelegramController
    {
        private readonly TelegramBotClient _bot;
        private readonly ITelegramRouteTree _telegramRouteTree;
        private readonly IServiceProvider _serviceProvider;

        public TelegramController(
            TelegramBotClient bot, 
            ITelegramRouteTree telegramRouteTree,
            IServiceProvider serviceProvider)
        {
            _bot = bot;
            _telegramRouteTree = telegramRouteTree;
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public async Task HandleUpdate([FromBody] Update update)
        {
           
        }
    }
}