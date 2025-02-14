using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StoreBotCSharp.Services
{
    public class StartService
    {
        private readonly ITelegramBotClient botClient;

        public StartService(ITelegramBotClient botClient)
        {
            this.botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        }

        public async Task HandleStartAsync(Message message, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var keyboard = CreateMainMenuKeyboard();
            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "Добро пожаловать! Выберите следующее действие:",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );
        }

        private static ReplyKeyboardMarkup CreateMainMenuKeyboard()
        {
            // Создаем клавиатуру с настройками через свойства
            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[] { "Товары", "Помощь" },
                    new KeyboardButton[] { "О магазине" }
                }
            )
            {
                ResizeKeyboard = true
            };
        }
    }
}