namespace SharedModels
{
    public record OrderItem(string Id, int Quantity);
    public record Order(
        string OrderId,
        string CustomerId,
        System.Collections.Generic.List<OrderItem> Items,
        decimal Total,
        System.DateTimeOffset Timestamp
    );
}
