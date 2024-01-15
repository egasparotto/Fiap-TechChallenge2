using System.Net;

using FiapOrders.Domain.Entities.Mails;
using FiapOrders.Domain.Entities.Orders;
using FiapOrders.Domain.Interfaces.Mails;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FiapOrders.Functions.Orders
{
    public class UserPaymentNotification
    {
        private readonly ISmtpMailSender _smtpMailSender;
        public UserPaymentNotification(ISmtpMailSender smtpMailSender)
        {
            _smtpMailSender = smtpMailSender;
        }

        [Function(nameof(UserPaymentNotification))]
        public void SayHello([ActivityTrigger] Mail mail, FunctionContext executionContext)
        {           
            _smtpMailSender.SendMail(mail);
        }
    }
}
