using Telegram.Bot;
using Telegram.Bot.Types;

public static class HelpHandler
{
    public static async Task HandleHelp(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "По вопросам бронирования, наличия в магазине, доставки, примерки : 8(910)486-12-13",
            cancellationToken: cancellationToken
        );
    }
}
