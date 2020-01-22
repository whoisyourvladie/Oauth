using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Nps
{
    public class NpsViewModel
    {
        [MaxLength(128)]
        public string Questioner { get; set; }

        [Required, MaxLength(100)]
        public string ClientName { get; set; }

        [MaxLength(20)]
        public string ClientVersion { get; set; }

        [Range(0, 10)]
        public byte Rating { get; set; }

        [Range(0, 5)]
        public byte? RatingUsage { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }
    }
}