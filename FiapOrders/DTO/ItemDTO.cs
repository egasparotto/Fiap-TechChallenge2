using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiapOrders.DTO
{
    public class ItemDTO
    {
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
