using MicroServiceMicrocontrollerManager.Models.Other;
using MicroServiceMicrocontrollerManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddSingleton<ProcessingService>();

builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MQTT"));
builder.Services.AddSingleton<MqttService>();


var app = builder.Build();

var processingService = app.Services.GetRequiredService<ProcessingService>();
processingService.StartProcessing();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
