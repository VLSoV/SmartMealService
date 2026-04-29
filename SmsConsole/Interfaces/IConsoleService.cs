using Sms.Domain.Models;

namespace Sms.ConsoleApp.Services;

public interface IConsoleService
{
    void DisplayMenu(IEnumerable<MenuItem> menuItems);
    void DisplayMessage(string message);
    IReadOnlyList<OrderItem> ReadOrder(HashSet<string> articles);
}