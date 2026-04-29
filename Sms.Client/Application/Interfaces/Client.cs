using Sms.Domain.Models;

namespace Sms.Application.Interfaces;

public interface ISmsClient
{
    Task<IEnumerable<MenuItem>> GetMenuAsync(bool withPrice, CancellationToken cancellationToken = default);
    Task<bool> SendOrderAsync(Order order, CancellationToken cancellationToken = default);
}
