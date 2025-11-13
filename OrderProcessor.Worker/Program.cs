using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedModels;
using System.Text.Json;

namespace OrderProcessor.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    services.AddHostedService<OrderProcessorService>();
                })
                .Build();

            await host.RunAsync();
        }
    }

    public class OrderProcessorService : BackgroundService
    {
        private readonly ILogger<OrderProcessorService> _logger;
        private IConsumer<Ignore, string>? _consumer;
        private IProducer<Null, string>? _producer;
        public OrderProcessorService(ILogger<OrderProcessorService> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cfg = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP") ?? "localhost:9092",
                GroupId = "order-processor-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(cfg).Build();
            _consumer.Subscribe("orders");

            var prodCfg = new ProducerConfig { BootstrapServers = cfg.BootstrapServers };
            _producer = new ProducerBuilder<Null, string>(prodCfg).Build();

            return Task.Run(() =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var cr = _consumer.Consume(stoppingToken);
                            if (cr?.Message?.Value != null)
                            {
                                _logger.LogInformation("Consumed order message at {TopicPartitionOffset}", cr.TopicPartitionOffset);
                                // simple processing: deserialize and add processedAt
                                var order = JsonSerializer.Deserialize<Order>(cr.Message.Value);
                                var processed = new
                                {
                                    order!.OrderId,
                                    order.CustomerId,
                                    order.Items,
                                    order.Total,
                                    order.Timestamp,
                                    ProcessedAt = DateTimeOffset.UtcNow
                                };
                                var payload = JsonSerializer.Serialize(processed);
                                _producer.Produce("orders_processed", new Message<Null, string> { Value = payload }, (r) =>
                                {
                                    if (r.Error.IsError) _logger.LogError("Produce error: {Reason}", r.Error.Reason);
                                    else _logger.LogInformation("Produced to orders_processed offset {Offset}", r.Offset);
                                });
                            }
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError(ex, "Consume error");
                        }
                    }
                }
                finally
                {
                    _consumer.Close();
                }
            }, stoppingToken);
        }

        public override void Dispose()
        {
            _consumer?.Dispose();
            _producer?.Dispose();
            base.Dispose();
        }
    }
}
