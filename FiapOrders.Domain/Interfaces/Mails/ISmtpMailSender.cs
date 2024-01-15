using FiapOrders.Domain.Entities.Mails;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiapOrders.Domain.Interfaces.Mails
{
    public interface ISmtpMailSender
    {
        void SendMail(Mail mail);
    }
}
