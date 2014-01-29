using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Validation
{
    internal static class BaseValidation
    {
        internal static  object GetValue(object objectInstance, string propertyName)
        {
            if (objectInstance == null)
                throw new ArgumentNullException("The Object Instance is null");
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException(String.Format("The property {0} is missing or is empty", propertyName));
            var propertyInfo = objectInstance.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentNullException(String.Format("The property {0} is missing", propertyName));          
            return propertyInfo.GetValue(objectInstance);
        }

        internal static T GetValue<T>(ModelMetadata metadata, ControllerContext context,string propertyName)
        {
            if(metadata ==null||context==null)
            {
                throw new ArgumentNullException("Metadata or Context");
            }
            var parentMetaData = ModelMetadataProviders.Current
                .GetMetadataForProperties(context.Controller.ViewData.Model, metadata.ContainerType);
            var propertyInfo = parentMetaData.FirstOrDefault(p =>
                p.PropertyName == propertyName);
            if (propertyInfo == null)
                throw new ArgumentNullException("The property {0} is missing", propertyName);
            if(propertyInfo.ModelType.GenericTypeArguments.FirstOrDefault()!=typeof(T))        
                throw new ArgumentException(String.Format("The property {0} must be a {1}", propertyName, typeof(T).ToString()));
            return (T)propertyInfo.Model;
        }
    }
}