using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json.Linq;
using SaaS.Api.Models.Validation;

namespace SaaS.Api.Models.Api.Oauth
{
    public class SurveyViewModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        public string Lang { get; set; }
        [Required]
        public JArray Data { get; set; }
    }

    public class SurveySection
    {
        public string Id { get; set; }
        public IEnumerable<SurveySectionGroup> Groups { get; set; }
    }

    public class SurveySectionGroup
    {
        public string Id { get; set; }
        public IEnumerable<SurveyQA> Qa { get; set; }
    }

    public class SurveyQA
    {
        public string Q { get; set; }
        public string A { get; set; }
    }

    public class SectionTableViewModel
    {
        public string Id { get; set; }

        public IList<SectionGroupTableViewModel> Groups { get; set; }

        public int QaCount
        {
            get
            {
                return Groups.Sum((group) => group.QAs.Count);
            }
        }

        public SectionTableViewModel(string id)
        {
            Id = id;
            Groups = new List<SectionGroupTableViewModel>();
        }
    }

    public class SectionGroupTableViewModel
    {
        public string Id { get; set; }
        public IList<string> QAs { get; set; }

        public SectionGroupTableViewModel(string id)
        {
            Id = id;
            QAs = new List<string>();
        }
    }

    public class SectionTableRowViewModel
    {
        public Guid AccountId { get; set; }
        public DateTime CreateDate { get; set; }

        public List<SurveyQATable> QAs { get; set; }

        public SectionTableRowViewModel(Guid accountId, DateTime createDate)
        {
            AccountId = accountId;
            CreateDate = createDate;
            QAs = new List<SurveyQATable>();
        }
    }

    public class SurveyQATable
    {
        public string QId { get; set; }
        public string Q { get; set; }
        public string A { get; set; }

        public SurveyQATable(string qId, string q, string a)
        {
            QId = qId;
            Q = q;
            A = a;
        }
    }
}