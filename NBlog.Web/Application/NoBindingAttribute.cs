using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Web;

namespace NBlog.Web.Application
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