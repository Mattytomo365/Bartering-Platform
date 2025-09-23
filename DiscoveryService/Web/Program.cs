using Application.Features.Commands;
using Application.Features.Handlers;
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Messaging.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Ensure DbContext is registered as scoped (already correct)
builder.Services.AddDbContext<SearchDbContext>(options =>
   options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOpts => sqlOpts.UseNetTopologySuite()
    )
);

// Register ListingRepository for DI
builder.Services.AddScoped<IListingRepository, ListingRepository>();

// MediatR for CQRS  
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(UpsertListingHandler).Assembly)
);

// RabbitMQ consumer as hosted service  
builder.Services.AddHostedService<ListingIndexConsumer>();

// Add CORS policy to allow Angular app to access the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy
          .WithOrigins("http://localhost:4200")
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

    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Run Migrations and Seed Database at Startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SearchDbContext>();

    dbContext.Database.Migrate();
}

// Enable CORS for the Angular app
app.UseCors("AllowAngularDev");

app.UseAuthorization();

app.MapControllers();

app.Run();
