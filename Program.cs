using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using StoreBotCSharp.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StoreBotCSharp.Models;
using Telegram.Bot.Types.ReplyMarkups;
using DotNetEnv;

class Program
{
	private static ITelegramBotClient _botClient;
	private static CategoryService _categoryService;
	private static StartService _startService;

	static async Task Main(string[] args)
	{
		Env.Load();

		var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
		var dataFilePath = Environment.GetEnvironmentVariable("DATAFILEPATH");

		if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(dataFilePath))
		{
			Console.WriteLine("Ошибка: Отсутствует токен бота или путь к файлу данных.");
			return;
		}

		_botClient = new TelegramBotClient(botToken);

		var jsonDataService = new JsonDataService(dataFilePath);

		jsonDataService.LoadData();

		_categoryService = new CategoryService(jsonDataService.Categories, jsonDataService.Products);
		_startService = new StartService(_botClient);

		var cancellationTokenSource = new CancellationTokenSource();
		_botClient.StartReceiving(
			HandleUpdateAsync,
			HandlePollingErrorAsync,
			default,
			cancellationTokenSource.Token
		);

		Console.WriteLine("Бот запущен. Нажмите Enter для выхода.");
		Console.ReadLine();

		cancellationTokenSource.Cancel();
	}

	private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
	{
		if (update.Message != null && update.Message.Text != null)
		{
			var message = update.Message;
			var text = message.Text.Trim();

			switch (text.ToLower())
			{
				case "/start":
					await _startService.HandleStartAsync(message, cancellationToken);
					break;

				case "товары":
					await HandleProductsAsync(botClient, message, _categoryService.GetCategories(), cancellationToken);
					break;

				case "помощь":
					await HelpHandler.HandleHelp(botClient, message, cancellationToken);
					break;

				case "о магазине":
					await AboutHandler.HandleAbout(botClient, message, cancellationToken);
					break;

				default:
					await botClient.SendMessage(
						chatId: message.Chat.Id,
						text: "Неизвестная команда. Пожалуйста, выберите действие из меню.",
						cancellationToken: cancellationToken
					);
					break;
			}
		}
		else if (update.CallbackQuery != null)
		{
			var callbackQuery = update.CallbackQuery;
			var data = callbackQuery.Data;

			if (data == "back_to_categories")
			{
				await SendCategoryListAsync(botClient, callbackQuery.Message.Chat.Id, _categoryService.GetCategories(), cancellationToken);
			}
			else
			{
				var selectedCategory = _categoryService.GetCategories().FirstOrDefault(c => c.Name.Equals(data, StringComparison.OrdinalIgnoreCase));
				if (selectedCategory != null)
				{
					var filteredProducts = _categoryService.GetProductsByCategory(selectedCategory.Name);

					if (filteredProducts.Any())
					{
						foreach (var product in filteredProducts)
						{
							await BotService.SendProductInfo(botClient, callbackQuery.Message.Chat.Id, product, cancellationToken);
						}

						// Добавляем кнопку "Назад" после товаров
						var backButton = new InlineKeyboardMarkup(
							InlineKeyboardButton.WithCallbackData("Назад", "back_to_categories")
						);

						await botClient.SendMessage(
							chatId: callbackQuery.Message.Chat.Id,
							text: "Чтобы вернуться к категориям, нажмите 'Назад'.",
							replyMarkup: backButton,
							cancellationToken: cancellationToken
						);
					}
					else
					{
						await botClient.SendMessage(
							chatId: callbackQuery.Message.Chat.Id,
							text: $"В категории '{selectedCategory.Name}' пока нет товаров.",
							cancellationToken: cancellationToken
						);
					}
				}
				else
				{
					await botClient.SendMessage(
						chatId: callbackQuery.Message.Chat.Id,
						text: "Неизвестная категория. Пожалуйста, выберите категорию из списка.",
						cancellationToken: cancellationToken
					);
				}
			}

			await botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
		}
	}

	private static async Task HandleProductsAsync(ITelegramBotClient botClient, Message message, List<Category> categories, CancellationToken cancellationToken)
	{
		await SendCategoryListAsync(botClient, message.Chat.Id, categories, cancellationToken);
	}

	private static async Task SendCategoryListAsync(ITelegramBotClient botClient, long chatId, List<Category> categories, CancellationToken cancellationToken)
	{
		var keyboard = new List<List<InlineKeyboardButton>>();

		for (int i = 0; i < categories.Count; i += 2)
		{
			var row = new List<InlineKeyboardButton>();

			row.Add(InlineKeyboardButton.WithCallbackData(categories[i].Name, categories[i].Name));

			if (i + 1 < categories.Count)
			{
				row.Add(InlineKeyboardButton.WithCallbackData(categories[i + 1].Name, categories[i + 1].Name));
			}

			keyboard.Add(row);
		}

		// Добавляем кнопку "Назад"
		keyboard.Add(new List<InlineKeyboardButton>
	{
		InlineKeyboardButton.WithCallbackData("Назад", "back_to_categories")
	});

		var inlineKeyboard = new InlineKeyboardMarkup(keyboard);

		await botClient.SendMessage(
			chatId: chatId,
			text: "Выберите категорию:",
			replyMarkup: inlineKeyboard,
			disableNotification: true,
			linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
			cancellationToken: cancellationToken
		);
	}

	private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
	{
		Console.WriteLine($"Ошибка в потоке получения обновлений: {exception.Message}");
		return Task.CompletedTask;
	}
}