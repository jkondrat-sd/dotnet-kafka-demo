using Confluent.Kafka;
using SharedModels;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Kafka producer config (change bootstrap.servers if running in docker)
builder.Services.AddSingleton<IProducer<Null, string>>(sp =>
{
    var cfg = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
    };
    return new ProducerBuilder<Null, string>(cfg).Build();
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.Run();
