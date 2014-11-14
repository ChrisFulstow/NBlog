using System.Net.Mail;

namespace NBlog.Web.Application.Service
{
    public interface IMessageService
    {
        void SendEmail(MailMessage mailMessage);
    }
}