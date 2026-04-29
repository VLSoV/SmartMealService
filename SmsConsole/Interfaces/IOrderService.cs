using Sms.Domain.Models;

namespace Sms.ConsoleApp.Services;

public interface IOrderService
{
    Task<IReadOnlyList<MenuItem>> InvalidateMenuAsync(CancellationToken cancellationToken = default);
    Task<SendOrderResult> SendOrderAsync(Order order, CancellationToken cancellationToken = default);
}

public record SendOrderResult(bool Success, string? ErrorMessage);