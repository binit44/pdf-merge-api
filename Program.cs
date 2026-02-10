var builder = WebApplication.CreateBuilder(args);

// ✅ DO NOT SET PORT OR URL MANUALLY ON RAILWAY
// Railway automatically provides ASPNETCORE_URLS

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Swagger ENABLED in production
app.UseSwagger();
app.UseSwaggerUI();

// ❌ Do NOT use HTTPS redirection on Railway
// app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();
