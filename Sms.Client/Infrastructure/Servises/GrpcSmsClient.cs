using Grpc.Net.Client;
using Sms.Application.Interfaces;
using Sms.Test;

namespace Sms.Infrastructure.Servises;

public class GrpcSmsClient : ISmsClient
{
    private readonly SmsTestService.SmsTestServiceClient _client;

    public GrpcSmsClient(string serverAddress)
    {
        var channel = GrpcChannel.ForAddress(serverAddress);
        _client = new SmsTestService.SmsTestServiceClient(channel);
    }

    public async Task<IEnumerable<Domain.Models.MenuItem>> GetMenuAsync(bool withPrice, CancellationToken cancellationToken = default)
    {
        var request = new Google.Protobuf.WellKnownTypes.BoolValue { Value = withPrice };
        var response = await _client.GetMenuAsync(request, cancellationToken: cancellationToken);

        if (!response.Success)
            throw new Exception($"Server returned error: {response.ErrorMessage}");

        return response.MenuItems.Select(m => new Domain.Models.MenuItem(
            Id: m.Id,
            Article: m.Article,
            Name: m.Name,
            Price: m.Price,
            IsWeighted: m.IsWeighted,
            FullPath: m.FullPath,
            Barcodes:  m.Barcodes.ToList()));
    }

    public async Task<bool> SendOrderAsync(Domain.Models.Order order, CancellationToken cancellationToken = default)
    {
        var grpcOrder = new Order
        {
            Id = order.Id
        };
        grpcOrder.OrderItems.AddRange(order.Items.Select(i => new Sms.Test.OrderItem
        {
            Id = i.Article,
            Quantity = (double)i.Quantity
        }));

        var response = await _client.SendOrderAsync(grpcOrder, cancellationToken: cancellationToken);
        if (!response.Success)
            throw new Exception($"Server returned error: {response.ErrorMessage}");

        return true;
    }
}