using Fanex.ExternalAccount.ReportsAndStatements.Services.Statements;
using Fanex.ExternalAccount.ReportsAndStatements.Services.Reports;
using Fanex.ExternalAccount.ReportsAndStatements.Controllers.Reports;
using Fanex.ExternalAccount.ReportsAndStatements.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Initialize configuration
Fanex.ExternalAccount.ReportsAndStatements.Utilities.Config.Initialize(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddRazorPages();

// Register services
builder.Services.AddScoped<Fanex.ExternalAccount.ReportsAndStatements.Services.Statements.IStatementService, Fanex.ExternalAccount.ReportsAndStatements.Services.Statements.StatementService>();
builder.Services.AddScoped<Fanex.ExternalAccount.ReportsAndStatements.Services.Reports.IReportService, Fanex.ExternalAccount.ReportsAndStatements.Services.Reports.ReportService>();
builder.Services.AddScoped<IWinLossDetailService, WinLossDetailService>();
builder.Services.AddScoped<IProductDetailService, ProductDetailService>();
builder.Services.AddSingleton<Fanex.ExternalAccount.ReportsAndStatements.Services.Configuration.StoredProcedureService>();

var app = builder.Build();

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
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
