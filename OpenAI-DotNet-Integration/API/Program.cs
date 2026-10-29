using OpenAI_DotNet_Integration.API.Middleware;
using OpenAI_DotNet_Integration.Data;
using OpenAI_DotNet_Integration.Domain.Interfaces;
using OpenAI_DotNet_Integration.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "OpenAI-DotNet-Integration API",
        Version = "v1",
        Description = "OpenAI .NET Integration ASP.NET Core Web API"
    });
});

builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISemanticKernelChatService, SemanticKernelChatService>();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();

//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenAI .NET Integration API v1");
        c.RoutePrefix = "swagger";
    });
//}

app.UseAuthorization();
app.MapControllers();

app.Run();
