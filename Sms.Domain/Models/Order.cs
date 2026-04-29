namespace Sms.Domain.Models;

public record Order(
    string Id,
    List<OrderItem> Items)
{
    public static Order CreateNew(IEnumerable<OrderItem> items)
    {
        var newOrderId = Guid.NewGuid().ToString().ToUpper();

        return new(
            Id: newOrderId,
            Items: items.ToList());
    }
}