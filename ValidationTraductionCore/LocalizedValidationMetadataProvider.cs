using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace ValidationTraductionCore
{
    public class LocalizedValidationMetadataProvider : IValidationMetadataProvider
    {
        public LocalizedValidationMetadataProvider()
        {
        }

        [Obsolete]
        public void CreateValidationMetadataOld(ValidationMetadataProviderContext context)
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
                    if (ValidationMessages.ResourceManager.GetString(name) != null){
                        tAttr.ErrorMessageResourceType = typeof(ValidationMessages);
                        tAttr.ErrorMessageResourceName = name;
                        tAttr.ErrorMessage = null;
                    }
                }
            }
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            Console.WriteLine($"🌍 LocalizedValidationMetadataProvider -> CreateValidationMetadata pour {context.Key.ModelType.Name}");
            var modelTypeInfo = context.Key.ModelType.GetTypeInfo();
            var validatorMetadata = context.ValidationMetadata.ValidatorMetadata;

            // Ajouter RequiredAttribute si le type est une valeur non nullable et n'a pas déjà un RequiredAttribute
            if (modelTypeInfo.IsValueType && Nullable.GetUnderlyingType(modelTypeInfo) == null &&
                !validatorMetadata.Any(m => m is RequiredAttribute))
            {
                validatorMetadata.Add(new RequiredAttribute());
            }

            // Vérifier et mettre à jour les messages d'erreur des ValidationAttributes
            foreach (var attribute in validatorMetadata.OfType<ValidationAttribute>())
            {
                Debug.WriteLine(attribute.GetType());

                if (attribute.ErrorMessage == null && attribute.ErrorMessageResourceName == null)
                {
                    string name = attribute.GetType().Name;
                    if (ValidationMessages.ResourceManager.GetString(name) != null)
                    {
                        attribute.ErrorMessageResourceType = typeof(ValidationMessages);
                        attribute.ErrorMessageResourceName = name;
                        attribute.ErrorMessage = null; // Facultatif mais explicite
                    }
                }
            }
        }
    }
}
