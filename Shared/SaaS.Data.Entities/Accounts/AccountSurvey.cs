using System;

namespace SaaS.Data.Entities.Accounts
{
    public class AccountSurvey : AccountEntity<int>
    {
        public string Lang { get; set; }
        public DateTime CreateDate { get; set; }
        public int? QuizTypeId { get; set; }
        public byte[] CollectedData { get; set; }
    }
}
