using Microsoft.Extensions.Logging;
using Sms.Domain.Models;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            try
            {
                var parsedInput = ParseInput(articles, input);
                return parsedInput;
            }
            catch (FormatException e)
            { 
                DisplayMessage(e.Message);
            }
        }
    }

    private List<OrderItem> ParseInput(HashSet<string> articles, string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new FormatException("Пустой ввод. Попробуйте снова.");
        }

        var parts = input.Split(';', StringSplitOptions.RemoveEmptyEntries);
        var items = new List<OrderItem>();

        foreach (var part in parts)
        {
            var orderItem = part.Split(':');
            if (orderItem.Length != 2)
            {
                throw new FormatException("Неверный формат пары Код:Количество");
            }

            var article = orderItem[0].Trim();
            if (!double.TryParse(orderItem[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var qty))
            {
                throw new FormatException($"Некорректное количество для кода {article}");
            }

            if (qty <= 0)
            {
                throw new FormatException($"Количество для кода {article} должно быть больше нуля");
            }

            if (!articles.Contains(article))
            {
                throw new FormatException($"Код {article} не найден в меню");
            }

            items.Add(new OrderItem(article, qty));
        }

        return items;
    }
}