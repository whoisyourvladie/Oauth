using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class ProductRenewalOffNotification : Notification
    {
        public ProductRenewalOffNotification() { }

        public ProductRenewalOffNotification(Notification user, string productName) :
            base(user)
        {
            ProductName = productName;
        }

        [XmlElement("productName")]
        public string ProductName { get; set; }
    }
}