using Fanex.Caching;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Service.Customer;
using Fanex.ExternalAccount.Service.Customer.CustomerList;
using Fanex.ExternalAccount.Service.Customer.MarkAccountType;
using Fanex.ExternalAccount.Service.DataRange;
using Fanex.ExternalAccount.Service.Impl.Customer;
using Fanex.ExternalAccount.Service.Impl.UserControl;
using Fanex.ExternalAccount.Service.Interface.Customer;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddLogging();

// Register services
builder.Services.AddScoped<ICurrentSession, CurrentSession>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IDataRangeService, DataRangeService>();
builder.Services.AddScoped<ISiteDBConfigService, SiteDBConfigService>();
builder.Services.AddScoped<IEditSuperListService, EditSuperListService>();
builder.Services.AddScoped<ICurrentContext, CurrentContext>();
builder.Services.AddScoped<ISuperService, SuperService>();
builder.Services.AddScoped<IAssociationAccountService, AssociationAccountService>();
builder.Services.AddScoped<IMarkAccountTypeService, MarkAccountTypeService>();
builder.Services.AddScoped<ICustomerListService, CustomerListService>();
// Note: ICustomerService and IAuthorizationService are defined in individual controller files
// and registered as scoped services within those controllers

var app = builder.Build();

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

