using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramBotClientExtended;

namespace TelegramService
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);
            
            
            // Добавляем служебные сервисы в контейнер.
            builder.Services.AddControllers();
            builder.Services.ConfigureTelegramBotMvc();
            builder.Services.AddTelegramBotClient(builder.Configuration);
            
            var app = builder.Build();
            
            // Привязывает маршруты к контроллерам.
            app.MapControllers();
            
            app.UseRouting();
            
            
            // Run server.
            app.RunAsync();
        }
    }
}