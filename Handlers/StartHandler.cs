namespace StoreBotCSharp.Handlers;

using StoreBotCSharp.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public static class StartHandler
{
    public static async Task HandleStart(ITelegramBotClient botClient, Message message)
    {
        var keyboard = BotService.CreateKeyboard(
            new[] { "Товары", "Помощь" },
            new[] { "О магазине" }
        );

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "Выберите следующее действие:",
            replyMarkup: keyboard
        );
    }
}