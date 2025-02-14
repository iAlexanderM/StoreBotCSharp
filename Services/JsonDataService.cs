using StoreBotCSharp.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace StoreBotCSharp.Services
{
    public class JsonDataService
    {
        private readonly string filePath;

        public List<Category> Categories { get; private set; } = new();
        public List<Product> Products { get; private set; } = new();

        public JsonDataService(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            LoadData();
        }

        public void LoadData()
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл данных не найден.", filePath);
            }

            var jsonData = File.ReadAllText(filePath);

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Игнорировать регистр символов
                };

                var root = JsonSerializer.Deserialize<Root>(jsonData, options);

                if (root == null)
                {
                    throw new InvalidOperationException("Неверный формат JSON-файла.");
                }

                // Проверяем, что Categories и Products не null
                Categories = root.Categories ?? new List<Category>();
                Products = root.Products ?? new List<Product>();

                foreach (var product in Products)
                {
                    product.Category = Categories.Find(c => c.Id == product.CategoryId) ?? new Category { Id = -1, Name = "Неизвестная категория" };
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Ошибка при десериализации JSON-файла.", ex);
            }
        }
    }

    public class Root
    {
        public List<Category> Categories { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }
}