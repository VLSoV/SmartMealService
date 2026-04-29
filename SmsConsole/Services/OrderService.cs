using Microsoft.Extensions.Logging;
using Sms.Application.Interfaces;
using Sms.ConsoleApp.Data;
using Sms.Domain.Models;

namespace Sms.ConsoleApp.Services;

public class OrderService(
    ISmsClient smsClient,
    IMenuRepository menuRepository,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<IReadOnlyList<MenuItem>> InvalidateMenuAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Получение меню с сервера");
        var menuItems = (await smsClient.GetMenuAsync(withPrice: true, cancellationToken)).ToList();

        logger.LogInformation("Сохранение меню в БД");
        await menuRepository.SaveMenuItemsAsync(menuItems, cancellationToken);
        return menuItems;
    }

    public async Task<SendOrderResult> SendOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        try
        {
            await smsClient.SendOrderAsync(order, cancellationToken);
            return new SendOrderResult(true, null);
        }
        catch (Exception ex)
        {
            return new SendOrderResult(false, ex.Message);
        }
    }
}