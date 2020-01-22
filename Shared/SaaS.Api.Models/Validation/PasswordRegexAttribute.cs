using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SaaS.Api.Models.Validation
{
    public class PasswordRegexAttribute : ValidationAttribute
    {
        string[] _args;

        private static readonly Regex _regex1 = new Regex(@"^((?=.*\d)(?=.*[^\s0-9!""\#$%&'()*+,\-./:;<=>?@\[\\\]^_`{|}~]).{6,})$", RegexOptions.IgnoreCase);

        private static readonly Regex _regex2 = new Regex(@"^(?!.*(\d)\1{2})(?!.*(0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)|9(?=0)){2}).{6,100}$", RegexOptions.IgnoreCase);
        private static readonly Regex _regex3 = new Regex(@"^(?!.*(abc|bcd|cde|def|efg|fgh|ghi|hij|ijk|jkl|klm|lmn|mno|nop|opq|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz)).{6,100}$", RegexOptions.IgnoreCase);
        private static readonly Regex _regex4 = new Regex(@"^(?!.*(.)\1{2}).*$", RegexOptions.IgnoreCase);

        private static readonly Regex _regex5 = new Regex(@"^(?!.*(admin|administrator|password)).{6,100}$", RegexOptions.IgnoreCase);

        public PasswordRegexAttribute(params string[] args)
        {
            _args = args;
        }

        public bool IsAllowNull { get; set; }

        protected override ValidationResult IsValid(object obj, ValidationContext validationContext)
        {
            string value = (string)obj;

            if (IsAllowNull && string.IsNullOrEmpty(value))
                return ValidationResult.Success;

            if (!_regex1.IsMatch(value))
                return new ValidationResult("Your password must be at least 6 characters and have both letters and numbers.");

            if (!_regex2.IsMatch(value) || !_regex3.IsMatch(value) || !_regex4.IsMatch(value))
                return new ValidationResult("Your password cannot contain 3 or more sequential characters or have the same character repeated sequentially (eg. 123, ABC, AAA, 111)");

            if (!_regex5.IsMatch(value))
                return new ValidationResult("Your password cannot contain \"password\", \"admin\" or \"administrator\"");

            return ValidationResult.Success;
        }
    }
}
