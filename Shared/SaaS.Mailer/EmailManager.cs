using System.Net.Mail;
using System.Text;

namespace SaaS.Mailer
{
    public static class EmailManager
    {
        public static void Send(SendEmailAction emailAction, string subject, string body)
        {
            using (MailMessage message = new MailMessage())
            {
                foreach (var mailAddress in emailAction.EmailToList)
                    message.To.Add(mailAddress.Email);

                message.Subject = subject;
                message.SubjectEncoding = Encoding.UTF8;
                message.Body = body;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;

                using (var client = new SmtpClient())
                    client.Send(message);
            }
        }
    }
}
