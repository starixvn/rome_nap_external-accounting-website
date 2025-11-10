using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddLogging();

// Register services
builder.Services.AddScoped<ICurrentSession, CurrentSession>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IExchangeService, ExchangeService>();
builder.Services.AddScoped<ISuperWinLossDetailByPTService, SuperWinLossDetailByPTService>();
builder.Services.AddScoped<IProductDetailService, ProductDetailService>();
builder.Services.AddScoped<ITotalBetService, TotalBetService>();
builder.Services.AddScoped<IWinLossDetailService, WinLossDetailService>();

var app = builder.Build();

// Initialize Excel template base paths
var webHostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();
Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport.TemplateSettings.SetBasePath(webHostEnvironment.ContentRootPath);
Fanex.ExternalAccount.Excel.Customer.CustomerListPage.TemplateSettings.SetBasePath(webHostEnvironment.ContentRootPath);
Fanex.ExternalAccount.Excel.Statement.StatementPage.TemplateSettings.SetBasePath(webHostEnvironment.ContentRootPath);
Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport.TemplateSettings.SetBasePath(webHostEnvironment.ContentRootPath);
Fanex.ExternalAccount.Excel.Reports.WinLossDetailIBCBetLiveCasinoReport.TemplateSettings.SetBasePath(webHostEnvironment.ContentRootPath);
Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT.TemplateSettings.SetBasePath(webHostEnvironment.ContentRootPath);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

