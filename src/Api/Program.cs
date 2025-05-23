using MongoDB.Driver;
using Application.Profiles;
using Application.Contracts;
using Application.Services;
using Domain.Contracts;
using Domain.Services;
using Domain.Contracts.Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(CustomerProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:9090")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("MongoDb");
var client = new MongoClient(connectionString);
var database = client.GetDatabase("cadastro");
builder.Services.AddSingleton<IMongoDatabase>(database);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IBasicsValidator, BasicsValidator>();
builder.Services.AddScoped<IFinancialValidator, FinancialValidator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IMessageService, RabbitMQMessage>();

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.Run();