
namespace Shop.Common.Models
{
    using System.Text.Json.Serialization;
    public class Product
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("lastPurchase")]
        public object LastPurchase { get; set; }

        [JsonPropertyName("lastSale")]
        public object LastSale { get; set; }

        [JsonPropertyName("isAvailabe")]
        public bool IsAvailabe { get; set; }

        [JsonPropertyName("stock")]
        public double Stock { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("imageFullPath")]
        public string ImageFullPath { get; set; }
    }
}
