using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class EmailConfirmationNotification : Notification
    {
        public EmailConfirmationNotification() { }
        public EmailConfirmationNotification(Notification user, string confirmationLink) : 
            base(user)
        {
            ConfirmationLink = confirmationLink;
        }

        [XmlElement("confirmationLink")]
        public string ConfirmationLink { get; set; }
    }
}
