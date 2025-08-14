using Deploy.Api.Application.Installers;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Adiciona CORS permitindo tudo
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SupportNonNullableReferenceTypes();
});

builder.Services.AddCustomServices();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue; // Kestrel
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue; // Sem limite prático
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");

app.MapControllers();
app.Run();
