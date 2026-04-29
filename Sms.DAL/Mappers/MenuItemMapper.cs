using Sms.Domain.Models;
using SmsConsole.DAL.DTOs;

namespace Sms.DAL.Mappers;

public static class MenuItemMapper
{
    public static MenuItemDto MapToDto(this MenuItem m) =>
        new()
        {
            Id = m.Id,
            Article = m.Article,
            Name = m.Name,
            Price = m.Price,
            IsWeighted = m.IsWeighted,
            FullPath = m.FullPath,
            Barcodes = m.Barcodes 
        };

    public static MenuItem MapToModel(this MenuItemDto dto) =>
        new(
            Id: dto.Id,
            Article: dto.Article,
            Name: dto.Name,
            Price: dto.Price,
            IsWeighted: dto.IsWeighted,
            FullPath: dto.FullPath,
            Barcodes: dto.Barcodes);
}