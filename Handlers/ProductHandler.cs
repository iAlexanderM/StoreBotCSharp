using Telegram.Bot;
using Telegram.Bot.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StoreBotCSharp.Models;
using StoreBotCSharp.Services;

namespace StoreBotCSharp.Handlers
{
    public static class ProductsHandler
    {
        public static async Task HandleProducts(ITelegramBotClient botClient, Message message, List<Category> categories, CancellationToken cancellationToken)
        {
            var messageText = "Выберите категорию:\n";

            for (int i = 0; i < categories.Count; i++)
            {
                messageText += $"{i + 1}. {categories[i].Name}\n";
            }

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: messageText,
                replyMarkup: BotService.CreateKeyboard(new[] { "Товары", "Помощь", "О магазине" }),
                cancellationToken: cancellationToken
            );
        }
    }
}