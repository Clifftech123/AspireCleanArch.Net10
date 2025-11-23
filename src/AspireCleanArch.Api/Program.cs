var builder = WebApplication.CreateBuilder(args);

// ============================================
// Add services to the container
// ============================================

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// TODO: Add Application Services (MediatR, Repositories, etc.)
// TODO: Add Infrastructure Services (DbContext, Redis, RabbitMQ)

var app = builder.Build();

// ============================================
// Configure the HTTP request pipeline
// ============================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
