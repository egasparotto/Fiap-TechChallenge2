using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiapOrders.DTO
{
    public class OrderDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IList<ItemDTO> Items { get; set; }
    }
}
