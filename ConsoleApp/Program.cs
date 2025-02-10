// See https://aka.ms/new-console-template for more information
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel;
using ValidationTraductionCore;
using System.Diagnostics.CodeAnalysis;

Console.WriteLine("Hello, World!");

var serviceProvider = new ServiceCollection()
    .AddSingleton<IValidationMetadataProvider, LocalizedValidationMetadataProvider>() // Fournisseur personnalisé
    .AddSingleton<ICompositeMetadataDetailsProvider, CustomCompositeMetadataDetailsProvider>(sp =>
        new CustomCompositeMetadataDetailsProvider(
            sp.GetServices<IValidationMetadataProvider>().ToList()
        ))
.AddSingleton<IModelMetadataProvider, DefaultModelMetadataProvider>(sp =>

    //var compositeProvider = sp.GetRequiredService<ICompositeMetadataDetailsProvider>();
    //return new DefaultModelMetadataProvider(compositeProvider);
    new DefaultModelMetadataProvider(sp.GetRequiredService<ICompositeMetadataDetailsProvider>())
)
    .AddSingleton<IObjectModelValidator, CustomObjectModelValidator>()
    .BuildServiceProvider();

var compositeProvider = serviceProvider.GetRequiredService<ICompositeMetadataDetailsProvider>();
Console.WriteLine($"📢 CompositeMetadataDetailsProvider utilisé : {compositeProvider.GetType().Name}");

var validator = serviceProvider.GetRequiredService<IObjectModelValidator>();
var metadataProvider = serviceProvider.GetRequiredService<IModelMetadataProvider>();

foreach (var provider in serviceProvider.GetServices<IValidationMetadataProvider>())
{
    Console.WriteLine($"📢 ValidationMetadataProvider enregistré : {provider.GetType().Name}");
}


var testObject = new TestClass();
ValidateModel(validator, metadataProvider,testObject);

static void ValidateModel(IObjectModelValidator validator, IModelMetadataProvider metadataProvider, object model)
{
    var actionContext = new ActionContext();
    var modelState = new ModelStateDictionary();

    Console.WriteLine($"📢 IModelMetadataProvider utilisé : {metadataProvider.GetType().Name}");
    Console.WriteLine("🔍 Forçage de l'exécution des métadonnées...");
    Console.WriteLine($"🔍 Demande des métadonnées pour {model.GetType().Name}");
    var modelMetadata = metadataProvider.GetMetadataForType(model.GetType());
    Console.WriteLine($"📢 Métadonnées récupérées : {modelMetadata.GetType().Name}");



    if (metadataProvider is DefaultModelMetadataProvider defaultMetadataProvider)
    {
        Console.WriteLine($"🛠️ Récupération des métadonnées pour {model.GetType().Name}");
        var metadata = defaultMetadataProvider.GetMetadataForType(model.GetType());

        Console.WriteLine($"📢 Métadonnées récupérées : {metadata.GetType().Name}");
    }

    // ✅ Exécuter la validation avec métadonnées personnalisées
    validator.Validate(actionContext, null, "", model);

    // 📌 Afficher les erreurs de validation
    if (actionContext.ModelState.IsValid)
    {
        Console.WriteLine("✅ Modèle valide !");
    }
    else
    {
        foreach (var error in actionContext.ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine($"❌ {error.ErrorMessage}");
        }
    }
}


// 🎯 Fournisseur composite personnalisé (remplace DefaultCompositeMetadataDetailsProvider)
public class CustomCompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
{
    private readonly IEnumerable<IValidationMetadataProvider> _validationProviders;

    public CustomCompositeMetadataDetailsProvider(IEnumerable<IValidationMetadataProvider> validationProviders)
    {
        _validationProviders = validationProviders.ToList();
        Console.WriteLine($"📢 Nombre de ValidationMetadataProviders enregistrés : {_validationProviders.Count()}");
    }

    public void CreateBindingMetadata(BindingMetadataProviderContext context) { }

    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        Console.WriteLine($"🔄 CustomCompositeMetadataDetailsProvider -> CreateValidationMetadata pour {context.Key.ModelType.Name}");

        foreach (var attribute in context.Attributes)
        {
            Console.WriteLine($"📌 Attribut trouvé : {attribute.GetType().Name}");
        }

        foreach (var provider in _validationProviders)
        {
            Console.WriteLine($"🔄 Appel de CreateValidationMetadata sur {provider.GetType().Name}");
            provider.CreateValidationMetadata(context);
        }
    }

    public void CreateDisplayMetadata(DisplayMetadataProviderContext context) { }
}

// 🎯 Implémentation du validateur respectant `ModelMetadata`
public class CustomObjectModelValidator : IObjectModelValidator
{
    private readonly IModelMetadataProvider _metadataProvider;

    public CustomObjectModelValidator(IModelMetadataProvider metadataProvider)
    {
        _metadataProvider = metadataProvider;
    }

    public void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
    {
        if (model == null) return;

        Console.WriteLine($"🔍 Chargement des métadonnées pour {model.GetType().Name}");
        var modelMetadata = _metadataProvider.GetMetadataForType(model.GetType()); // ⚠️ Force l'exécution des providers

        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();

        // 🔥 Appliquer la validation
        bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        if (!isValid)
        {
            foreach (var validationResult in validationResults)
            {
                var memberName = validationResult.MemberNames.FirstOrDefault() ?? "";
                actionContext.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
            }
        }
    }
}

public class TestClass()
{

    [Required]
    [DisplayName("Ma prop")]
    public string MaProp { get; set; } = null!;

    [Required]
    public string UneAutre { get; set; } = null!;
}