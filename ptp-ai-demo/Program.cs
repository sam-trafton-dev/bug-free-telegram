using Microsoft.EntityFrameworkCore;
using ptp_ai_demo.Data;
using ptp_ai_demo.Models;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;



var flutterDevPort = "http://localhost:60412";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutterDev",
        policy =>
        {
            policy.WithOrigins(flutterDevPort)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
var keyVaultUri = new Uri(builder.Configuration["KeyVault:VaultUri"]!);
builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

var connection = string.Empty;
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    connection = builder.Configuration.GetConnectionString("AZURE-SQL-CONECTIONSTRING");
}
else
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.json");
    connection = builder.Configuration.GetConnectionString("AZURE-SQL-CONECTIONSTRING");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connection));

var app = builder.Build();


app.UseCors("AllowFlutterDev");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();


app.MapGet("/Application", (ApplicationDbContext context) => context.Users.ToList())
    .WithName("GetUsers")
    .WithOpenApi();

app.MapPost("/Application", (User user, ApplicationDbContext context) =>
    {
        context.Add(user);
        context.SaveChanges();
    })
    .WithName("CreateUser")
    .WithOpenApi();
app.Run();

