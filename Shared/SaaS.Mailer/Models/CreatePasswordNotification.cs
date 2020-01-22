using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class CreatePasswordNotification : Notification
    {
        public CreatePasswordNotification() { }
        public CreatePasswordNotification(Notification user) : base(user) { }

        [XmlElement("createPasswordLink")]
        public string CreatePasswordLink { get; set; }
    }
}