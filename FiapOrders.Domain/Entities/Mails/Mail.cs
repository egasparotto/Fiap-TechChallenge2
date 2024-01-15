using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiapOrders.Domain.Entities.Mails
{
    public class Mail
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

    }
}
