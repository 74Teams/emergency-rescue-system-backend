using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using RescueSystem.Infrastructure.Seeders;
using RescueSystem.Application;
using RescueSystem.Infrastructure;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "RescueSystem.Api.xml");
    if (File.Exists(xmlFile))
    {
        c.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br/>
                      Enter 'Bearer' [space] and then your token in the text input below.<br/>
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(document => new()
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddProblemDetails();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
    else
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    }
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var response = new
            {
                success = false,
                statusCode = 401,
                message = "Yêu cầu không được xác thực. Vui lòng cung cấp token JWT hợp lệ",
                data = (object?)null
            };

            await context.Response.WriteAsJsonAsync(response);
        },

        OnForbidden = async context =>
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            var response = new
            {
                success = false,
                statusCode = 403,
                message = "Bạn không có quyền truy cập tài nguyên này",
                data = (object?)null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        // options.JsonSerializerOptions.Converters.Add(
        //    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddAuthorization();
var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?.Error;

        var response = new
        {
            success = false,
            message = exception?.Message ?? "Internal Server Error",
            statusCode = 500
        };

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(response);
    });
});

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await ApplicationSeeder.SeedAsync(scope.ServiceProvider, logger);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rescue System API v1");
        c.RoutePrefix = string.Empty;
    });

}

app.UseMiddleware<RescueSystem.Api.Middlewares.GlobalExceptionMiddleware>();

app.UseCors(builder.Environment.IsDevelopment() ? "AllowAll" : "AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
