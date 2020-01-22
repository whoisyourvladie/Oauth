using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class WelcomePurchaseNotification : CreatePasswordNotification
    {
        public WelcomePurchaseNotification() { }
        public WelcomePurchaseNotification(Notification user, string productName) :
            base(user)
        {
            ProductName = productName;
        }

        [XmlElement("productName")]
        public string ProductName { get; set; } 
    }
}