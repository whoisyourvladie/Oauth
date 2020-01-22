using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class EmailChangeNotification : Notification
    {
        public EmailChangeNotification() { }
        public EmailChangeNotification(Notification user, string newEmail) : 
            base(user)
        {
            NewEmail = newEmail;
        }

        [XmlElement("newEmail")]
        public string NewEmail { get; set; }
    }
}
