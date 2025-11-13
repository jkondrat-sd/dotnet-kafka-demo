using Confluent.Kafka;
using System.Text.Json;

Console.WriteLine("Analytics service starting - consuming orders_processed...");
var cfg = new ConsumerConfig
{
    BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP") ?? "localhost:9092",
    GroupId = "analytics-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<Ignore, string>(cfg).Build();
consumer.Subscribe("orders_processed");

while (true)
{
    try
    {
        var cr = consumer.Consume();
        Console.WriteLine($"[Analytics] Message at {cr.TopicPartitionOffset}");
        var obj = JsonSerializer.Deserialize<JsonElement>(cr.Message.Value);
        Console.WriteLine($"[Analytics] Order total: {obj.GetProperty("Total").GetDecimal()}");
    }
    catch (ConsumeException ex)
    {
        Console.WriteLine($"Consume error: {ex.Error.Reason}");
    }
}
