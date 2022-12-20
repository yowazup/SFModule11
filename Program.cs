using Microsoft.Extensions.Hosting;
using MyFirstTelegramBot.Controllers;
using Telegram.Bot;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using MyFirstTelegramBot.Services;
using MyFirstTelegramBot.Configuration;

namespace MyFirstTelegramBot
{
    public class Program
    {
        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.Unicode;

            // Объект, отвечающий за постоянный жизненный цикл приложения
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
                .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
                .Build(); // Собираем

            Console.WriteLine("Сервис запущен");
            // Запускаем сервис
            await host.RunAsync();
            Console.WriteLine("Сервис остановлен");
        }

        static void ConfigureServices(IServiceCollection services)
        {
            //Добавляем инициализацию конфигурации
            AppSettings appSettings = BuildAppSettings();
            services.AddSingleton(BuildAppSettings());
            services.AddSingleton<IFileHandler, AudioFileHandler>();

            //Добавляем хранилище сессий в контейнер зависимостей
            services.AddSingleton<IStorage, MemoryStorage>();

            // Подключаем контроллеры сообщений и кнопок
            services.AddTransient<DefaultMessageController>();
            services.AddTransient<VoiceMessageController>();
            services.AddTransient<TextMessageController>();
            services.AddTransient<InlineKeyboardController>();

            // Регистрируем объект TelegramBotClient c токеном подключения
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(appSettings.BotToken));

            // Регистрируем постоянно активный сервис бота
            services.AddHostedService<Bot>();
        }
        static AppSettings BuildAppSettings()
        {
            return new AppSettings()
            {
                DownloadsFolder = "D:/",
                BotToken = "5810036243:AAHuApfTmd-IGNkXcUVSzz9G6Buq8VNl4qM",
                AudioFileName = "audio",
                InputAudioFormat = "ogg",
                OutputAudioFormat = "wav",
                InputAudioBitrate = 48000,
            };
        }
    }
}