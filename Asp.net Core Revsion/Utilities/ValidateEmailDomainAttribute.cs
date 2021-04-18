using System;
using System.ComponentModel.DataAnnotations;

namespace Asp.net_Core_Revsion.Utilities
{
    public class ValidateEmailDomainAttribute : ValidationAttribute
    {
        private readonly string _allowedDomain;

        public ValidateEmailDomainAttribute(string allowedDomain)
        {
            _allowedDomain = allowedDomain;
        }
        public override bool IsValid(object value)
        {
            //            var domainToCompare = "pragimtech.com";

            var email = value.ToString();
            var domain = email.Substring(email.IndexOf("@", StringComparison.Ordinal) + 1);

            return domain.ToLower().Equals(_allowedDomain.ToLower());

        }
    }
}