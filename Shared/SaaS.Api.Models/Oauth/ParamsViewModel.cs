using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Oauth
{
    public class ParamsViewModel
    {
        private string _cmp = null;
        private string _partner = null;
        private string _build = null;

        [MaxLength(128)]
        public string Cmp
        {
            get { return _cmp; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                _cmp = value.Trim();
            }
        }

        [MaxLength(128)]
        public string Partner
        {
            get { return _partner; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                _partner = value.Trim();
            }
        }

        [MaxLength(10)]
        public string Build
        {
            get { return _build; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                _build = value.Trim();
            }
        }

        public string Uid { get; set; }

        public int? GetUid()
        {
            int uid;
            if (int.TryParse(Uid, out uid))
                return uid;

            return null;
        }
    }
}
