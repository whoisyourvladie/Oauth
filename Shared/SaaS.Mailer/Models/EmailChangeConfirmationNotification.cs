using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class EmailChangeConfirmationNotification : Notification
    {
        public EmailChangeConfirmationNotification() { }
        public EmailChangeConfirmationNotification(Notification user, string newEmail, string confirmationLink) :
            base(user)
        {
            NewEmail = newEmail;
            ConfirmationLink = confirmationLink;
        }

        [XmlElement("newEmail")]
        public string NewEmail { get; set; }

        [XmlElement("confirmationLink")]
        public string ConfirmationLink { get; set; }
    }
}