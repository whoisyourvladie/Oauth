using System;
using System.Reflection;
using System.Xml.Serialization;

namespace SaaS.Mailer.Models
{
    public class Notification
    {
        public Notification() { }

        public Notification(Notification notification)
        {
            AccountId = notification.AccountId;
            FirstName = notification.FirstName;
            LastName = notification.LastName;
            Email = notification.Email;

            DownloadLink = notification.DownloadLink;
        }


        [XmlElement("accountId")]
        public Guid AccountId { get; set; }

        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }

        [XmlElement("downloadLink")]
        public string DownloadLink { get; set; }


        [XmlIgnore]
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                    return Email;

                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public string this[string key]
        {
            get
            {
                var type = GetType();
                var property = type.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (object.Equals(property, null))
                    return null;

                var value = property.GetValue(this);
                if (object.Equals(value, null))
                    return null;

                return value.ToString();
            }
        }
    }
}