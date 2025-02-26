using Microsoft.EntityFrameworkCore;
using ptp_ai_demo.Data;
using ptp_ai_demo.Models;
using Azure.Identity;


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

var connection = string.Empty;

var keyVaultUri = new Uri("https://kv-samatraf523931822466.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

connection = builder.Configuration["ConnectionString:AZURE-SQL-CONNECTIONSTRING"];
Console.WriteLine("Connection value");
Console.WriteLine(connection);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connection, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

var app = builder.Build();


app.UseCors("AllowFlutterDev");
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

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
Console.WriteLine("App starting...");
app.Run();

