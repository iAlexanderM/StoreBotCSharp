using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;

public static class AboutHandler
{
    public static async Task HandleAbout(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "*Тамбовчанка Кашира*\r\nРоссийский бренд качественной женской одежды с более чем полувековой историей." +
            "\r\n\r\nСтильный дизайн, величайшее разнообразие моделей, широкий размерный ряд и доступные цены изделий от Тамбовчанки™ " +
            "уже оценили жители 50 регионов России! Тамбовчанка™ - любимый и узнаваемый российский бренд!" +
            "\r\n\r\nВ нашем магазине вы можете приобрести нашу продукцию по весьма привлекательным ценам. Обновление ассортимента происходит каждые две недели! " +
            "Следите за обновлениями в нашей группе VK и Telegram канале",
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken
        );

        await botClient.SendVenue(
            chatId: message.Chat.Id,
            latitude: 54.836897,
            longitude: 38.246380,
            title: "Тамбовчанка Кашира",
            address: "г. Кашира, ул. Садовая д.32, ТЦ Звезда, 3 этаж",
            cancellationToken: cancellationToken
        );
    }
}
