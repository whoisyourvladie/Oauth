using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class MergeConfirmationNotification : Notification
    {
        public MergeConfirmationNotification() { }
        public MergeConfirmationNotification(Notification user, string confirmationLink) :
            base(user)
        {
            ConfirmationLink = confirmationLink;
        }

        [XmlElement("confirmationLink")]
        public string ConfirmationLink { get; set; }
    }
}