using Fanex.ExternalAccount.Reports.Helpers;
using Fanex.Customer.Client;
using Fanex.Customer.Client.Configurations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Fanex.ExternalAccount.Utilities;

var builder = WebApplication.CreateBuilder(args);

    // Configure CustomerClientManager before building the app
    var customerConfig = new CustomerClientConfiguration();
    // Note: CustomerClientConfiguration properties are read-only in the real NuGet package
    // The configuration values are set via appsettings, but we need to call UseConfig with a valid instance
    CustomerClientManager.UseConfig(customerConfig);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Configure authentication
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/Login";
            options.ExpireTimeSpan = TimeSpan.FromHours(24);
        });

    // Initialize the Config class with the configuration
    Config.Initialize(builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    StaticResources.Init(app.Environment);

    app.Run();
