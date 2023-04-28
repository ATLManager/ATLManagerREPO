using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ATLManager.Data;
using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using ATLManager.Services;
using ATLManager;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.FileProviders;
using ATLManager.Controllers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ATLManagerAuthContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ATLManagerAuthContextConnection' not found.");

var keyVaultUrl = builder.Configuration["KeyVault:Vault"];
//var keyVaultCredential = new DefaultAzureCredential();

var clientId = builder.Configuration["KeyVault:ClientId"];
var clientSecret = builder.Configuration["KeyVault:ClientSecret"];
var tenantId = builder.Configuration["KeyVault:TenantId"];
var keyVaultCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);



var client = new SecretClient(new Uri(keyVaultUrl), keyVaultCredential);
builder.Services.AddSingleton(client);

/*
builder.Services.AddSingleton<IFileManager, FileManager>();
builder.Services.AddSingleton<IFileProvider>(
    new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))); */

var sendGridKeySecret = client.GetSecret("SendGridKey");

builder.Services.AddDbContext<ATLManagerAuthContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ATLManagerUser>(options => {
    // Sign in
    options.SignIn.RequireConfirmedAccount = true;

    // Password
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Lockout
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ATLManagerAuthContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<LanguageService>();
builder.Services.AddSingleton<IFileManager ,FileManager>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(ShareResource).GetTypeInfo().Assembly.FullName);
            return factory.Create("SharedResource", assemblyName.Name);
        };
    });

builder.Services.AddScoped<LanguageService>();

builder.Services.Configure<RequestLocalizationOptions>(
    options =>
    {
        var supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("pt-PT"),
            new CultureInfo("de-DE"),
            new CultureInfo("fr-FR"),
        };

        options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    });

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.Configure<AuthMessageSenderOptions>(options =>
{
    options.SendGridKey = sendGridKeySecret.Value.Value;
});
//builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

var facebookAppIdSecret = client.GetSecret("FacebookAppId");
var facebookAppSecretSecret = client.GetSecret("FacebookAppSecret");

builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = facebookAppIdSecret.Value.Value;
    facebookOptions.AppSecret = facebookAppSecretSecret.Value.Value;
    //facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    //facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
});

var googleClientIdSecret = client.GetSecret("GoogleClienteId");
var googleClientSecretSecret = client.GetSecret("GoogleClientSecret");

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = googleClientIdSecret.Value.Value;
    googleOptions.ClientSecret = googleClientSecretSecret.Value.Value;
    //googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    //googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

var twitterConsumerAPIKeySecret = client.GetSecret("TwitterConsumerApiKey");
var twitterConsumerSecretSecret = client.GetSecret("TwitterConsumerSecret");

builder.Services.AddAuthentication().AddTwitter(twitterOptions =>
{
    twitterOptions.ConsumerKey = twitterConsumerAPIKeySecret.Value.Value;
    twitterOptions.ConsumerSecret = twitterConsumerSecretSecret.Value.Value;
    //twitterOptions.ConsumerKey = builder.Configuration["Authentication:Twitter:ConsumerAPIKey"];
    //twitterOptions.ConsumerSecret = builder.Configuration["Authentication:Twitter:ConsumerSecret"];
});

var microsoftClientId = client.GetSecret("MicrosoftClientId");
var microsoftClientSecret = client.GetSecret("MicrosoftClientSecret");

builder.Services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = microsoftClientId.Value.Value;
    microsoftOptions.ClientSecret = microsoftClientSecret.Value.Value;
	//microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
	//microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
});

builder.Services.AddScoped<INotificacoesController, NotificacoesController>();
builder.Services.AddScoped<EstatisticasController>();
builder.Services.AddScoped<RefeicoesController>();



var app = builder.Build();

var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

using var scope = app.Services.CreateScope();
await Configurations.CreateRoles(scope.ServiceProvider);


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
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();