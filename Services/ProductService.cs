using StoreBotCSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace StoreBotCSharp.Services
{
    public class ProductService
    {
        private readonly List<Product> _products;
        private readonly List<Category> _categories;

        public ProductService(List<Product> products, List<Category> categories)
        {
            _products = products;
            _categories = categories;
        }

        public List<Product> GetProductsByCategory(string categoryName)
        {
            // Находим категорию по имени (с учетом регистра)
            var category = _categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                return new List<Product>(); // Возвращаем пустой список, если категория не найдена
            }

            // Фильтруем товары по CategoryId
            return _products.Where(p => p.CategoryId == category.Id).ToList();
        }
    }
}