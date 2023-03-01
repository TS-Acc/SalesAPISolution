using System.Text.Json.Serialization;

namespace SalesAPI.Models
{
    public class OrderLine
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;

        public int OrderId { get; set; }
        [JsonIgnore]
        public virtual Order? Order { get; set; } = null!;

        public int ItemId { get; set; }
        public virtual Item? Item { get; set; } = null!;
    }
}
