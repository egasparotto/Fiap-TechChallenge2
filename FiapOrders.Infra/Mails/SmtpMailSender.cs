using FiapOrders.Domain.Entities.Mails;
using FiapOrders.Domain.Interfaces.Mails;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FiapOrders.Infra.Mails
{
    public class SmtpMailSender : ISmtpMailSender
    {
        private readonly SmtpConfig _smtpConfig;

        public SmtpMailSender(IOptions<SmtpConfig> options)
        {
            _smtpConfig = options.Value;
        }

        public void SendMail(Mail mail)
        {
            using var smtpClient = new SmtpClient(_smtpConfig.Host, _smtpConfig.Port);
            var credentials = new NetworkCredential(_smtpConfig.User, _smtpConfig.Password);
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;

            var to = new MailAddress(mail.To);
            var from = new MailAddress(_smtpConfig.From);

            using var mailMessage = new MailMessage(from, to);
            mailMessage.Subject = mail.Subject;
            mailMessage.Body = mail.Message;
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);
        }
    }
}
