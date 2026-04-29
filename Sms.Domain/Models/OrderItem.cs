namespace Sms.Domain.Models;

public record OrderItem(
    string Article,
    double Quantity);