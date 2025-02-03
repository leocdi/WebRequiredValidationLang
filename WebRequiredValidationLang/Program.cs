using Microsoft.Extensions.Localization;
using WebRequiredValidationLang;
using WebRequiredValidationLang.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ######## Dans mes recherche on passe par addMVC, cela m'embete un peu de faire addmvc sans connaitre les consequences, cela semble fonctionner si mis dans dans AddControllersWithViews
//builder.Services.AddMvc(o => o.ModelMetadataDetailsProviders.Add(new LocalizedValidationMetadataProvider()));


// ######## builder.Services.AddLocalization(o => o.ResourcesPath ="Resources"); pas d'accord avec chat gpt, si je fait ça il faut aller chercher dans Resources.Resources
// ######## au passage sur le fichier ressource, bien penser a spécifier PublicResXFileCodeGenerator comme outil de génération et s'assurer que la ressource est publique
builder.Services.AddLocalization();
builder.Services.AddControllersWithViews(o => o.ModelMetadataDetailsProviders.Add(new LocalizedValidationMetadataProvider()))
    //.AddDataAnnotationsLocalization(o => o.DataAnnotationLocalizerProvider = (Type,factory)=> new StringLocalizer<ValidationMessages>(factory)); j'ai fait ça dans un autre projet, je ne sais pas pourquoi ?
    .AddDataAnnotationsLocalization();

var app = builder.Build();

// ######## Ne semble pas utile, a confirmer ?
//var supportedCultures = new[] { "en-US", "fr-FR" };
//var localizationOptions = new RequestLocalizationOptions()
//    .SetDefaultCulture("fr-FR")
//    .AddSupportedCultures(supportedCultures)
//    .AddSupportedUICultures(supportedCultures);
//app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
