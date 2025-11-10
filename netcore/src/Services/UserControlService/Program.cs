using Fanex.ExternalAccount.Core.Authorization;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Service.Impl.Login;
using Fanex.ExternalAccount.Service.Impl.UserControl;
using Fanex.ExternalAccount.Service.Interface.Login;
using Fanex.ExternalAccount.Service.Products;
using Fanex.ExternalAccount.Service.UserControl;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Register services
builder.Services.AddScoped<ICurrentSession, CurrentSession>();
builder.Services.AddScoped<ICurrentContext, CurrentContext>();
builder.Services.AddScoped<ICachingService, CachingService>();
builder.Services.AddScoped<IFunctionService, FunctionService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IGroupConfigurationService, GroupConfigurationService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISubGroupService, SubGroupService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<AuthorizationService, AuthorizationServiceImpl>();

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

