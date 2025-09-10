using Application.Features.Handlers;
using Application.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Database  
builder.Services.AddDbContext<ProfileDbContext>(opts =>
   opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MediatR  
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetProfileLocationHandler).Assembly)
);

// Repository  
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();

// Add HttpContextAccessor to access user claims
builder.Services.AddHttpContextAccessor();

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
}

app.UseHttpsRedirection();

// Insert ApiKeyMiddleware BEFORE any routing/auth middleware
app.UseMiddleware<ApiKeyMiddleware>();

// Run Migrations and Seed Database at Startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProfileDbContext>();

    dbContext.Database.Migrate();
}

// Enable CORS for the Angular app
app.UseCors("AllowAngularDev");

app.UseAuthorization();

app.MapControllers();

app.Run();
