using Microsoft.EntityFrameworkCore;
using Sms.DAL.Mappers;
using Sms.Domain.Models;

namespace Sms.ConsoleApp.Data;

public class MenuRepository : IMenuRepository
{
    private readonly SmsDbContext _context;

    public MenuRepository(SmsDbContext context)
    {
        _context = context;
    }

    public async Task SaveMenuItemsAsync(IEnumerable<MenuItem> items, CancellationToken cancellationToken = default)
    {
        // Удаляем старые записи и добавляем новые
        _context.MenuItems.RemoveRange(_context.MenuItems);
        await _context.MenuItems.AddRangeAsync(items.Select(item => item.MapToDto()), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}