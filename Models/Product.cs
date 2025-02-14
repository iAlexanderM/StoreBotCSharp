namespace StoreBotCSharp.Models
{
    public class Product
    {
        public required int Id { get; set; }
        public required int CategoryId { get; set; }
        public required string Structure { get; set; }
        public required decimal Price { get; set; }
        public required string[] ImageUrls { get; set; }
        public required bool InStock { get; set; }
        public required string Size { get; set; }
        public required bool IsInStock { get; set; }
        public required string ProductId { get; set; }
        public Category? Category { get; set; }
    }
}
