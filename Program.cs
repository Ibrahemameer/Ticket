using Radzen;
using RhinoTicketingSystem.Components;
using Microsoft.EntityFrameworkCore;
using RhinoTicketingSystem.Data;
using Microsoft.AspNetCore.Identity;
using RhinoTicketingSystem.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.Components.Authorization;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using RhinoTicketingSystem.Controllers;
using RhinoTicketingSystem.Services;
using Microsoft.Graph;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);
builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "RhinoTicketingSystemTheme";
    options.Duration = TimeSpan.FromDays(365);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<OneDriveService>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<MegaUploadService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<EmailService>();


builder.Services.AddScoped<GraphServiceClient>(provider =>
{
    var clientId = builder.Configuration["AzureAd:ClientId"];
    var clientSecret = builder.Configuration["AzureAd:ClientSecret"];
    var tenantId = builder.Configuration["AzureAd:TenantId"];

    var credentials = new ClientSecretCredential(
        tenantId,
        clientId,
        clientSecret);

    return new GraphServiceClient(credentials);
});

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


builder.Services.AddHttpClient();
builder.Services.AddScoped<RhinoTicketingSystem.db_a79800_ticketService>();
builder.Services.AddScoped<UploadController>();

builder.Services.AddDbContext<RhinoTicketingSystem.Data.db_a79800_ticketContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("db_a79800_ticketConnection"));
});
builder.Services.AddLocalization();
builder.Services.AddHttpClient("RhinoTicketingSystem").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false }).AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<RhinoTicketingSystem.SecurityService>();
builder.Services.AddScoped<CultureService>();
//builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("db_a79800_ticketConnection"));
//});

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("db_a79800_ticketConnection"),
        sqlOptions => sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory_Identity"));
});


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllers().AddOData(o =>
{
    var oDataBuilder = new ODataConventionModelBuilder();
    oDataBuilder.EntitySet<ApplicationUser>("ApplicationUsers");
    var usersType = oDataBuilder.StructuralTypes.First(x => x.ClrType == typeof(ApplicationUser));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Password)));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.ConfirmPassword)));
    oDataBuilder.EntitySet<ApplicationRole>("ApplicationRoles");
    o.AddRouteComponents("odata/Identity", oDataBuilder.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});

////hold this if we needed 

//builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
//{
//    // Identity options here
//})
//.AddEntityFrameworkStores<ApplicationIdentityDbContext>()
//.AddDefaultTokenProviders();

builder.Services.AddScoped<AuthenticationStateProvider, RhinoTicketingSystem.ApplicationAuthenticationStateProvider>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseRequestLocalization(options => options.AddSupportedCultures("en", "ar-SA").AddSupportedUICultures("en", "ar-SA").SetDefaultCulture("en"));
app.UseHeaderPropagation();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
//app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().Database.Migrate();


app.Run();