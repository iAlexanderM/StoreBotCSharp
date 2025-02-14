using StoreBotCSharp.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

namespace StoreBotCSharp.Services
{
    public static class BotService
    {
        public static ReplyKeyboardMarkup CreateKeyboard(params string[][] buttons)
        {
            var keyboardButtons = buttons.Select(row => row.Select(buttonText => new KeyboardButton(buttonText)).ToArray()).ToArray();

            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        public static async Task SendProductInfo(ITelegramBotClient botClient, long chatId, Product product, CancellationToken cancellationToken)
        {
            var caption = $"*Категория:* {product.Category.Name}\n" +
                          $"*Артикул:* {product.ProductId}\n" +
                          $"*Состав:* {product.Structure}\n" +
                          $"*Размер:* {product.Size}\n" +
                          $"*Цена:* {product.Price} руб.";

            if (product.ImageUrls.Length == 0)
            {
                await botClient.SendMessage(
                    chatId: chatId,
                    text: caption,
                    parseMode: ParseMode.Markdown,
                    disableNotification: true,
                    cancellationToken: cancellationToken
                );
                return;
            }

            var media = new List<IAlbumInputMedia>();
            var streams = new List<Stream>(); // Список открытых потоков

            try
            {
                foreach (var imageUrl in product.ImageUrls)
                {
                    if (!File.Exists(imageUrl))
                    {
                        Console.WriteLine($"Файл не найден: {imageUrl}");
                        continue;
                    }

                    var fileInfo = new FileInfo(imageUrl);
                    if (fileInfo.Length > 10 * 1024 * 1024) // 10 МБ
                    {
                        Console.WriteLine($"Файл слишком большой: {imageUrl}");
                        continue;
                    }

                    var stream = File.OpenRead(imageUrl); // Открываем поток вручную
                    streams.Add(stream); // Сохраняем ссылку на поток
                    var inputFile = InputFile.FromStream(stream, Path.GetFileName(imageUrl));
                    media.Add(new InputMediaPhoto(inputFile));
                }

                if (media.Any())
                {
                    if (media.First() is InputMediaPhoto firstPhoto)
                    {
                        firstPhoto.Caption = caption;
                        firstPhoto.ParseMode = ParseMode.Markdown;
                    }

                    await botClient.SendMediaGroup(
                        chatId: chatId,
                        media: media,
                        disableNotification: true,
                        cancellationToken: cancellationToken
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка HTTP-запроса: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неизвестная ошибка: {ex.Message}");
            }
            finally
            {
                // Закрываем все потоки после завершения операции
                foreach (var stream in streams)
                {
                    stream.Dispose();
                }
            }
        }
    }
}