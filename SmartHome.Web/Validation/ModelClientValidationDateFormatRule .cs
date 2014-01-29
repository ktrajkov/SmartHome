using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Validation
{
    public class ModelClientValidationDateFormatRule : ModelClientValidationRule
    {
        public ModelClientValidationDateFormatRule(string errorMessage, string dateFormat)
        {
            this.ErrorMessage = errorMessage;
            this.ValidationType = "dateformat";
            this.ValidationParameters.Add("dateformat", dateFormat);
        }
    }
}