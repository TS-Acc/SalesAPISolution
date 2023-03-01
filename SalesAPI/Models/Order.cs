using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [StringLength(150)]
        public string? Description { get; set; }
        [StringLength(20)]
        public string Status { get; set; } = "NEW";
        [Column(TypeName = "decimal(9,2)")]
        public decimal Total { get; set; } = 0;

        public int? CustomerId { get; set; } = null;
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderLine>? OrderLines { get; set; }
    }
}
