using Microsoft.Extensions.Logging;
using Sms.ConsoleApp.Services;
using Sms.Domain.Models;

namespace SmsConsole.Services;

public class SmsConsoleService(
    IOrderService _orderService,
    IConsoleService _console,
    ILogger<SmsConsoleService> _logger)
{
    public async Task RunAsync()
    {
        try
        {
            _logger.LogInformation("Запуск работы сервиса");

            // Получение меню и сохранение в БД
            var menuItems = await _orderService.InvalidateMenuAsync();
            _logger.LogInformation("Меню успешно получено и сохранено в БД. Количество позиций: {Count}", menuItems.Count);

            // Вывод меню
            _console.DisplayMenu(menuItems);

            // Ввод заказа пользователем
            var orderItems = _console.ReadOrder(menuItems.Select(m => m.Article).ToHashSet());
            _logger.LogInformation("Пользователь ввёл {Count} позиций", orderItems.Count);

            // Создание заказа
            Order order = Order.CreateNew(orderItems);
            _logger.LogInformation("Создан заказ с ID {OrderId}", order.Id);

            // Отправка заказа
            var result = await _orderService.SendOrderAsync(order);
            if (result.Success)
            {
                _logger.LogInformation("Заказ {OrderId} успешно отправлен", order.Id);
                _console.DisplayMessage($"УСПЕХ");
            }
            else
            {
                _logger.LogError("Ошибка отправки заказа: {Error}", result.ErrorMessage);
                _console.DisplayMessage(result.ErrorMessage ?? "Неизвестная ошибка");
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Критическая ошибка выполнения приложения");
            _console.DisplayMessage($"Критическая ошибка: {ex.Message}");
        }
    }
}