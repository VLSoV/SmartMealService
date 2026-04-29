using Microsoft.Extensions.Logging;
using Sms.Domain.Models;
using System.Globalization;

namespace Sms.ConsoleApp.Services;

public class ConsoleService(
    ILogger<ConsoleService> logger
    ) : IConsoleService
{
    public void DisplayMenu(IEnumerable<MenuItem> menuItems)
    {
        DisplayMessage("Список блюд:");
        foreach (var item in menuItems)
        {
            DisplayMessage($"{item.Name} – {item.Article} – {item.Price:F2}");
        }
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
        logger.LogInformation(message);
    }

    public IReadOnlyList<OrderItem> ReadOrder(HashSet<string> articles)
    {
        while (true)
        {
            DisplayMessage("Введите позиции заказа в формате: Код1:Количество1;Код2:Количество2;...");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayMessage("Пустой ввод. Попробуйте снова.");
                continue;
            }

            var parts = input.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var items = new List<OrderItem>();
            var error = false;

            foreach (var part in parts)
            {
                var orderItem = part.Split(':');
                if (orderItem.Length != 2)
                {
                    DisplayMessage("Неверный формат пары Код:Количество");
                    error = true;
                    break;
                }

                var article = orderItem[0].Trim();
                if (!double.TryParse(orderItem[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var qty))
                {
                    DisplayMessage($"Некорректное количество для кода {article}");
                    error = true;
                    break;
                }

                if (qty <= 0)
                {
                    DisplayMessage($"Количество для кода {article} должно быть больше нуля");
                    error = true;
                    break;
                }

                if (!articles.Contains(article))
                {
                    DisplayMessage($"Код {article} не найден в меню");
                    error = true;
                    break;
                }

                items.Add(new OrderItem(article, qty));
            }

            if (!error)
            {
                return items;
            }
        }
    }
}