using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    [XmlRoot("xml")]
    public class RecoverPasswordNotification : Notification
    {
        public RecoverPasswordNotification() { }
        public RecoverPasswordNotification(Notification user, string recoverPasswordLink) :
            base(user)
        {
            RecoverPasswordLink = recoverPasswordLink;
        }


        [XmlElement("recoverPasswordLink")]
        public string RecoverPasswordLink { get; set; }
    }
}