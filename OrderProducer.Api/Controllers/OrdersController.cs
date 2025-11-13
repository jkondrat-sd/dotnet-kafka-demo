using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Text.Json;

namespace OrderProducer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<OrdersController> _logger;
    private const string Topic = "orders";

    public OrdersController(IProducer<Null, string> producer, ILogger<OrdersController> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Order order)
    {
        var payload = JsonSerializer.Serialize(order);
        try
        {
            var dr = await _producer.ProduceAsync(Topic, new Message<Null, string> { Value = payload });
            _logger.LogInformation("Produced message to {Topic} partition {Partition} offset {Offset}", dr.Topic, dr.Partition, dr.Offset);
            return Accepted(new { dr.Topic, dr.Partition, dr.Offset });
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, "Failed to produce message");
            return StatusCode(500, ex.Error.Reason);
        }
    }
}
