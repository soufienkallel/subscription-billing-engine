using Application;
using Application.Configuration;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<BillingOptions>(builder.Configuration.GetSection(BillingOptions.SectionName));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok());

app.Run();

public partial class Program { }
