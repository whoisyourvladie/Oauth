using System;
using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class ProductSuspendNotification : Notification
    {
        public ProductSuspendNotification() { }

        public ProductSuspendNotification(Notification user, string productName, DateTime? expireDate) :
            base(user)
        {
            ProductName = productName;
            ExpireDate = expireDate;
        }

        [XmlElement("productName")]
        public string ProductName { get; set; }

        [XmlElement("expireDate", IsNullable = true)]
        public DateTime? ExpireDate { get; set; }
    }
}