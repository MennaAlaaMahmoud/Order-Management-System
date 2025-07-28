using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Invoice : BaseEntity<int>
    {
        //  public int InvoiceId { get; set; }
        public int OrderId { get; set; } 
        public Order  order { get; set; }  // One
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }



    }
}
