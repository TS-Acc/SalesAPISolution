using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        [StringLength(35)]
        public string Name { get; set; } = string.Empty;
        [Column(TypeName = "decimal(7,2)")]
        public decimal Price { get; set; } = 0;
    }
}
