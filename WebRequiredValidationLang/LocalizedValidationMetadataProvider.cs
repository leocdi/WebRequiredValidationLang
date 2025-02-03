using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace WebRequiredValidationLang
{
    public class LocalizedValidationMetadataProvider : IValidationMetadataProvider
    {
        public LocalizedValidationMetadataProvider()
        {
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context.Key.ModelType.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(context.Key.ModelType.GetTypeInfo()) == null && context.ValidationMetadata.ValidatorMetadata.Where(m => m.GetType() == typeof(RequiredAttribute)).Count() == 0)
                context.ValidationMetadata.ValidatorMetadata.Add(new RequiredAttribute());
            foreach (var attribute in context.ValidationMetadata.ValidatorMetadata)
            {
                var tAttr = attribute as ValidationAttribute;
                Debug.WriteLine(tAttr?.GetType());
                if (tAttr?.ErrorMessage == null && tAttr?.ErrorMessageResourceName == null && tAttr != null)
                {
                    var name = tAttr.GetType().Name;
                    if (Resources.ValidationMessages.ResourceManager.GetString(name) != null)
                    {
                        tAttr.ErrorMessageResourceType = typeof(Resources.ValidationMessages);
                        tAttr.ErrorMessageResourceName = name;
                        tAttr.ErrorMessage = null;
                    }
                }
            }
        }
    }
}
