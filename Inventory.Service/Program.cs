using Confluent.Kafka;
using System.Text.Json;

Console.WriteLine("Inventory service starting - consuming orders_processed...");
var cfg = new ConsumerConfig
{
    BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP") ?? "localhost:9092",
    GroupId = "inventory-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<Ignore, string>(cfg).Build();
consumer.Subscribe("orders_processed");

while (true)
{
    try
    {
        var cr = consumer.Consume();
        Console.WriteLine($"[Inventory] Message at {cr.TopicPartitionOffset}");
        // In a real service you'd deserialize and update DB
        var obj = JsonSerializer.Deserialize<JsonElement>(cr.Message.Value);
        Console.WriteLine($"[Inventory] OrderId: {obj.GetProperty("OrderId").GetString()}");
    }
    catch (ConsumeException ex)
    {
        Console.WriteLine($"Consume error: {ex.Error.Reason}");
    }
}
