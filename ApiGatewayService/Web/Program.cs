using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Web.Auth;

var builder = WebApplication.CreateBuilder(args);

// Tell the Host to use Serilog, reading settings from configuration:
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
);

// Enable detailed error messages (PII) from the token validator - suppressed in production!
if (!builder.Environment.IsProduction())
{
    IdentityModelEventSource.ShowPII = true;
}
else
{
    IdentityModelEventSource.ShowPII = false;
}

// IF THESE DON'T WORK FOR DOCKER, TRY THE COMMENTED-OUT BLOCK BELOW
// Load Ocelot configuration based on environment
var envName = builder.Environment.EnvironmentName?.ToLowerInvariant();
if (envName == "development")
{
    builder.Configuration.AddJsonFile("ocelot.development.json", optional: false, reloadOnChange: true);
}
else if (envName == "docker")
{
    builder.Configuration.AddJsonFile("ocelot.docker.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}


// MAY NEED THESE FOR DOCKER - COMMENTED OUT FOR NOW
//// 1) Load the "standard" ASP NET Core appsettings pipeline...
//builder.Configuration
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)

//// 2) Always overlay the Development settings (so you get the same behavior
////    as if you ran in VS with ASPNETCORE_ENVIRONMENT=Development)
//    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)

//// 3) Then load any environment?specific file (for completeness)
//    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)

//// 4) Wire up Ocelot config: always load the base, then overlay the Docker one
//    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
//    .AddJsonFile("ocelot.docker.json", optional: true, reloadOnChange: true)
//    .AddEnvironmentVariables();

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

// JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.Authority = builder.Configuration["Authentication:Authority"];
      options.Audience = builder.Configuration["Authentication:Audience"];
      // Log all JWT errors - PII is shown in the logs - need to be careful with this in production
      options.Events = new JwtBearerEvents
      {
          OnAuthenticationFailed = ctx =>
          {
              if (!ctx.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsProduction())
              {
                  // resolve a logger from DI:  
                  var logger = ctx.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger("JwtAuth");
                  logger.LogError(ctx.Exception,
                      "[JwtAuth] AuthenticationFailed: {ExceptionType} — {Message}",
                      ctx.Exception.GetType().Name, ctx.Exception.Message);
                  if (ctx.Exception.InnerException != null)
                  {
                      logger.LogError(ctx.Exception.InnerException,
                          "[JwtAuth] Inner: {InnerType} — {InnerMessage}",
                          ctx.Exception.InnerException.GetType().Name,
                          ctx.Exception.InnerException.Message);
                  }
              }
              return Task.CompletedTask;
          },
          OnTokenValidated = ctx =>
          {
              if (!ctx.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsProduction())
              {
                  var logger = ctx.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger("JwtAuth");
                  logger.LogInformation("[JwtAuth] Token was valid.");
              }
              return Task.CompletedTask;
          }
      };
  });

// Register DelegatingHandler for injecting headers
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<GatewayAuthDelegatingHandler>();

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<GatewayAuthDelegatingHandler>(true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();  // log HTTP requests


// Log the Authorization header (masked) for security
app.Use(async (context, next) =>
{
    var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var rawAuth = context.Request.Headers["Authorization"].FirstOrDefault();

    if (!env.IsProduction())
    {
        logger.LogInformation("[Gateway] Raw Authorization header: '{rawAuth}'", rawAuth);
    }
    else if (!string.IsNullOrEmpty(rawAuth))
    {
        // Mask all but the first 6 and last 4 characters for security
        var masked = rawAuth.Length > 10
            ? $"{rawAuth.Substring(0, 6)}...{rawAuth.Substring(rawAuth.Length - 4)}"
            : "***";
        logger.LogInformation("[Gateway] Authorization header (masked): '{masked}'", masked);
    }

    await next();
});


// Enable CORS for the Angular app
app.UseCors("AllowAngularDev");

if (builder.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Use Ocelot as gateway middleware
await app.UseOcelot();

app.Run();
