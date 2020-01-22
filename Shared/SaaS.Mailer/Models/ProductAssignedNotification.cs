using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class ProductAssignedNotification : CreatePasswordNotification
    {
        public ProductAssignedNotification() { }

        public ProductAssignedNotification(Notification user, string productName, Notification ownerUser) :
            base(user)
        {
            ProductName = productName;

            OwnerFirstName = ownerUser.FirstName;
            OwnerLastName = ownerUser.LastName;
            OwnerEmail = ownerUser.Email;
        }

        [XmlElement("productName")]
        public string ProductName { get; set; }

        [XmlElement("ownerFirstName")]
        public string OwnerFirstName { get; set; }

        [XmlElement("ownerLastName")]
        public string OwnerLastName { get; set; }

        [XmlElement("ownerEmail")]
        public string OwnerEmail { get; set; }

        [XmlIgnore]
        public string OwnerFullName
        {
            get
            {
                if (string.IsNullOrEmpty(OwnerFirstName) && string.IsNullOrEmpty(OwnerLastName))
                    return OwnerEmail;

                return string.Format("{0} {1}", OwnerFirstName, OwnerLastName);
            }
        }
    }
}