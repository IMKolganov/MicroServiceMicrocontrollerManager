using MicroServiceMicrocontrollerManager.Models.Other;
using MicroServiceMicrocontrollerManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddSingleton<ProcessingService>();

builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MQTT"));
builder.Services.AddSingleton<MqttService>();


var app = builder.Build();

app.UseRouting();

var processingService = app.Services.GetRequiredService<ProcessingService>();
processingService.StartProcessing();

// Configure the HTTP request pipeline.
if(app.Environment.IsEnvironment("Docker") || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();