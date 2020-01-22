using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Api.Oauth
{
    public class ParamsViewModel
    {
        private string _cmp = null;
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
    }
}