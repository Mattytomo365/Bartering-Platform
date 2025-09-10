using Application.Features.Commands;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Config;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Messaging.RabbitMQ;

//using Messaging.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

//using RabbitMQ.Client;
using System.Reflection;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ListDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application Services
builder.Services.AddScoped<IListingRepository, ListingRepository>();
builder.Services.AddScoped<IListingService, ListingService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateListingHandler).Assembly)
);

// RabbitMq START Configuration *****************************************************************************************************
var rabbitCfg = builder.Configuration.GetSection("RabbitMQ");

// 1) single, long‐lived IConnection (async open, synchronously unwrapped)
builder.Services.AddSingleton<IConnection>(sp =>
{
    var cfg = rabbitCfg;
    var factory = new ConnectionFactory
    {
        HostName = cfg["Host"]!,
        Port = cfg.GetValue("Port", 5672),
        UserName = cfg["Username"]!,
        Password = cfg["Password"]!,
        VirtualHost = cfg["VirtualHost"] ?? "/"
    };

    return factory
        .CreateConnectionAsync()
        .GetAwaiter()
        .GetResult();
});

// 2) single, long‐lived IChannel (async open, synchronously unwrapped)
builder.Services.AddSingleton<IChannel>(sp =>
{
    var conn = sp.GetRequiredService<IConnection>();
    return conn
        .CreateChannelAsync()
        .GetAwaiter()
        .GetResult();
});

// 3) now your publisher takes IChannel
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

// RabbitMq END Configuration *******************************************************************************************************

// Add CORS policy to allow Angular app to access the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy
          .WithOrigins("http://localhost:4200", "https://localhost:4200")
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Insert ApiKeyMiddleware BEFORE any routing/auth middleware
app.UseMiddleware<ApiKeyMiddleware>();

// Run Migrations and Seed Database at Startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ListDbContext>();

    dbContext.Database.Migrate();
}

// Enable CORS for the Angular app
app.UseCors("AllowAngularDev");

if (builder.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
