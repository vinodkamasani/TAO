<<<<<<< Updated upstream
=======
using TAO.Api;
using TAO.Api.Middleware;
using TAO.Application;
>>>>>>> Stashed changes
using TAO.Infrastructure.DependencyInjection;
using TAO.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


<<<<<<< Updated upstream
=======
builder.Services.AddApplication();
builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAiServices(builder.Configuration);


>>>>>>> Stashed changes
// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();


