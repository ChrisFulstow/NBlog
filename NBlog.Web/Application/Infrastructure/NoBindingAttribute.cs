using System;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace NBlog.Web.Application.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NoBindingAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
                throw new SecurityException(
                    "Binding to this property is not allowed: "
                    + validationContext.ObjectType + "." + validationContext.DisplayName);

            return null;
        }
    }

}