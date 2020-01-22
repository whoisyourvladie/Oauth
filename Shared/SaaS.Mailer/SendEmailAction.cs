using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SaaS.Mailer
{
    [XmlType(AnonymousType = true)]
    [XmlRoot("xml")]
    public class SendEmailAction
    {
        [XmlArray("to")]
        [XmlArrayItem("email")]
        public List<XmlEmail> EmailToList { get; set; }

        [XmlAttribute("templateId")]
        public EmailTemplate TemplateId { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class XmlEmail
    {
        [XmlAttribute("email")]
        public string Email { get; set; }
    }


    [XmlType(AnonymousType = true)]
    [XmlRoot("xml")]
    public class SendEmailAction2
    {
        [XmlArray("to")]
        [XmlArrayItem("email")]
        public List<XmlEmail> EmailToList { get; set; }

        //[XmlAttribute("templateId")]
        //public string TemplateId { get; set; }

        [XmlIgnore]
        public EmailTemplate EmailTemplateValue { get; set; }

        [XmlIgnore]
        public string EmailTemplateDBValue { get; set; }

        [XmlAttribute("templateId")]
        public string TemplateId
        {
            get { return EmailTemplateValue.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value) || Enum.IsDefined(typeof(EmailTemplate), value) == false)
                {
                    EmailTemplateDBValue = value;
                    //EmailTemplateValue = default(EmailTemplate);
                    EmailTemplateValue = EmailTemplate.None;
                }
                else
                {
                    EmailTemplateValue = (EmailTemplate)Enum.Parse(typeof(EmailTemplate), value);
                }
            }
        }
    }
}