using SmartHome.Web.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Validation
{
    public class MinTempRangeAlertAttribute : ValidationAttribute, IClientValidatable
    {
        public string MaxTempProperty { get; set; }

        public MinTempRangeAlertAttribute(string maxTempPropertyName)
        {
            MaxTempProperty = maxTempPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var maxTempAlertFromOtherProperty = BaseValidation.GetValue(validationContext.ObjectInstance, MaxTempProperty);
            if (maxTempAlertFromOtherProperty == null || value == null)
            {
                return null;
            }
            double maxTempAlertFromOtherPropertyValue = Convert.ToDouble(maxTempAlertFromOtherProperty);
            double maxTemp = maxTempAlertFromOtherPropertyValue > Settings.Default.MaxTempAlert ?
                Settings.Default.MaxTempAlert : maxTempAlertFromOtherPropertyValue;
            double minTemp = Convert.ToDouble(value);
            if (minTemp < Settings.Default.MinTempAlert || minTemp > maxTemp)
            {
                return new ValidationResult(FormatErrorMessage(String.Empty));
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "mintemprange",
                ErrorMessage = FormatErrorMessage(String.Empty),
            };
            modelClientValidationRule.ValidationParameters["mintemp"] = Settings.Default.MinTempAlert;
            modelClientValidationRule.ValidationParameters["maxtempproperty"] = MaxTempProperty;
            yield return modelClientValidationRule;
        }

        public override string FormatErrorMessage(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return String.Format("The Temperature must be between {0} and the value from {1}",
                     Settings.Default.MinTempAlert.ToString(), MaxTempProperty);
            }
            else
            {
                return ErrorMessage;
            }
        }

    }

    public class MaxTempRangeAlertAttribute : ValidationAttribute, IClientValidatable
    {
        public string MinTempProperty { get; set; }
        public MaxTempRangeAlertAttribute(string minTempProperty)
        {
            MinTempProperty = minTempProperty;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var minTempAlertFromOtherProperty = BaseValidation.GetValue(validationContext.ObjectInstance, MinTempProperty);
            if (minTempAlertFromOtherProperty == null || value == null)
            {
                return null;
            }
            double minTempAlertFromOtherPropertyValue = Convert.ToDouble(minTempAlertFromOtherProperty);
            double minTempAlert = minTempAlertFromOtherPropertyValue < Settings.Default.MinTempAlert ?
            Settings.Default.MinTempAlert : minTempAlertFromOtherPropertyValue;
            double maxTempAlert = Convert.ToDouble(value);
            if (maxTempAlert > Settings.Default.MaxTempAlert || maxTempAlert < minTempAlert)
            {
                return new ValidationResult(FormatErrorMessage(String.Empty));
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "maxtemprange",
                ErrorMessage = FormatErrorMessage(String.Empty)
            };
            modelClientValidationRule.ValidationParameters["maxtemp"] = Settings.Default.MaxTempAlert;
            modelClientValidationRule.ValidationParameters["mintempproperty"] = MinTempProperty;
            yield return modelClientValidationRule;
        }

        public override string FormatErrorMessage(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return String.Format("The Temperature must be between the value from {0} and {1}",
                    MinTempProperty, Settings.Default.MaxTempAlert.ToString());
            }
            else
            {
                return ErrorMessage;
            }
        }
    }

    public class ThermostatTempRangeAttribute : RangeAttribute, IClientValidatable
    {
        public ThermostatTempRangeAttribute() : base(Settings.Default.MinTempThermostat, Settings.Default.MaxTempThermostat) { }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.DisplayName);
            rule.ValidationParameters.Add("min", Settings.Default.MinTempThermostat);
            rule.ValidationParameters.Add("max", Settings.Default.MaxTempThermostat);
            rule.ValidationType = "range";
            yield return rule;
        }
    }

    public class RequiredIfTrueAttribute : ValidationAttribute, IClientValidatable
    {
        public string BooleanPropertyName { get; set; }
        public RequiredIfTrueAttribute(string booleanPropertyName)
        {
            this.BooleanPropertyName = booleanPropertyName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if ((bool)BaseValidation.GetValue(validationContext.ObjectInstance, BooleanPropertyName))
            {
                return new RequiredAttribute().IsValid(value) ?
                    ValidationResult.Success :
                    new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "requirediftrue",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            modelClientValidationRule.ValidationParameters.Add("boolprop", BooleanPropertyName);
            yield return modelClientValidationRule;
        }
    }

    public class MinDateRangeAttribute : ValidationAttribute
    {
        public string MaxDateProperty { get; set; }

        public MinDateRangeAttribute(string maxDateProperty)
        {
            MaxDateProperty = maxDateProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var maxDateFromOtherProperty = BaseValidation.GetValue(validationContext.ObjectInstance, MaxDateProperty);
            if (maxDateFromOtherProperty == null || value == null)
            {
                return null;
            }       
            if ((DateTime)value > (DateTime)maxDateFromOtherProperty)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                return null;
            }
        }

    }

    public class MaxDateRangeAttribute : ValidationAttribute
    {
        public string MinDateProperty { get; set; }

        public MaxDateRangeAttribute(string minDateProperty)
        {        
            MinDateProperty = minDateProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var minDateFromOtherProperty = BaseValidation.GetValue(validationContext.ObjectInstance, MinDateProperty);
            if (minDateFromOtherProperty == null || value == null)
            {
                return null;
            }
            DateTime currentDate = Convert.ToDateTime(value);
            if (currentDate > DateTime.Now || currentDate <(DateTime) minDateFromOtherProperty)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                return null;
            }
        }

    }


  
}