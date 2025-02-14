using StoreBotCSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace StoreBotCSharp.Services
{
    public class CategoryService
    {
        private readonly List<Category> _categories;
        private readonly List<Product> _products;

        public CategoryService(List<Category> categories, List<Product> products)
        {
            _categories = categories;
            _products = products;
        }

        public List<Category> GetCategories()
        {
            return _categories;
        }

        public List<Product> GetProductsByCategory(string categoryName)
        {
            var category = _categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                return new List<Product>(); 
            }

            return _products.Where(p => p.CategoryId == category.Id).ToList();
        }
    }
}