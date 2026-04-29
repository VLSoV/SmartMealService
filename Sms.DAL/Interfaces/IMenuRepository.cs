using Sms.Domain.Models;

namespace Sms.ConsoleApp.Data;

public interface IMenuRepository
{
    Task SaveMenuItemsAsync(IEnumerable<MenuItem> items, CancellationToken cancellationToken = default);
}