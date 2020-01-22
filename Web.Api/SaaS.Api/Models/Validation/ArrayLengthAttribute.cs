using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Validation
{
    public class ArrayMaxLengthAttribute : ValidationAttribute
    {
        private readonly int _length;

        public ArrayMaxLengthAttribute(int length)
        {
            _length = length;
        }

        public override bool IsValid(object value)
        {
            if (value is ICollection == false)
            {
                return false;
            }

            return ((ICollection) value).Count <= _length;
        }
    }
}