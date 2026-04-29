namespace Sms.Domain.Models;

public record MenuItem(
     string Id,
     string Article,
     string Name,
     double Price,
     bool IsWeighted,
     string FullPath,
     List<string> Barcodes);