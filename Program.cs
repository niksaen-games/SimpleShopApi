using Amazon.S3;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleShopApi.Data;
using SimpleShopApi.Services;
using SimpleShopApi.Utils;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();

//Configuration
var configuration = builder.Configuration;

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var dbConfig = configuration.GetSection("PostgreSQL");
    var connStr = $"Host={dbConfig["Host"]};" +
    $"Port={dbConfig["Port"]};" +
    $"Database={dbConfig["Database"]};" +
    $"Username={dbConfig["User"]};" +
    $"Password={dbConfig["Password"]}";
    options.UseNpgsql(connStr);
});

// Настройка S3 (MinIO)
builder.Services.AddSingleton<IAmazonS3>(serviceProvider =>
{
    var minioConfig = configuration.GetSection("MinIO");

    var config = new AmazonS3Config
    {
        ServiceURL = minioConfig["Endpoint"],
        ForcePathStyle = true,
        AuthenticationRegion = "us-east-1"
    };

    var credentials = new Amazon.Runtime.BasicAWSCredentials(minioConfig["User"], minioConfig["Password"]);

    return new AmazonS3Client(credentials, config);
});

// JWT
var jwtConfig = configuration.GetSection("Jwt");
var secretKey = jwtConfig["SecretKey"];
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
var expiriesInMinutes = int.Parse(jwtConfig["ExpiriesInMinutes"]!);
var tokenParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidAudience = jwtConfig["Audience"],
    ValidIssuer = jwtConfig["Issuer"],
    ClockSkew = TimeSpan.Zero,
    IssuerSigningKey = signingKey
};

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = tokenParameters);

builder.Services
    .AddSingleton(s => new JwtTokenGenerator(tokenParameters, expiriesInMinutes));

//Services
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<MailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
