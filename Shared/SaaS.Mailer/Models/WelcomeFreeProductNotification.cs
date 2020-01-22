using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class WelcomeFreeProductNotification : Notification
    {
        public WelcomeFreeProductNotification() { }
        public WelcomeFreeProductNotification(Notification user, string productName) :
            base(user)
        {
            ProductName = productName;
        }

        [XmlElement("productName")]
        public string ProductName { get; set; }
    }
}