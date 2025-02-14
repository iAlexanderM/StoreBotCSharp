using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreBotCSharp.Models;
using StoreBotCSharp.Services;

namespace StoreBotCSharp.Handlers
{
    public static class CategoryHandler
    {
        public static async Task HandleCategorySelection(ITelegramBotClient botClient, Message message, List<Product> products, CancellationToken cancellationToken)
        {
            var selectedCategory = message.Text;
            var filteredProducts = products.Where(p => p.Category.Name == selectedCategory).ToList();

            if (!filteredProducts.Any())
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "В данной категории товаров нет.",
                    cancellationToken: cancellationToken
                );
                return;
            }

            foreach (var product in filteredProducts)
            {
                await BotService.SendProductInfo(botClient, message.Chat.Id, product, cancellationToken);
            }
        }
    }
}