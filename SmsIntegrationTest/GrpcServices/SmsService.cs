using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Sms.Test;

namespace SmsIntegrationTest.Services;

public class SmsService : SmsTestService.SmsTestServiceBase
{
    public override Task<GetMenuResponse> GetMenu(BoolValue withPrice, ServerCallContext context)
    {
        var response = new GetMenuResponse
        {
            Success = true,
            ErrorMessage = ""
        };
        response.MenuItems.Add(new MenuItem
        {
            Id = "5979224",
            Article = "A1004292",
            Name = "Каша гречневая",
            Price = withPrice.Value ? 50 : default,
            IsWeighted = false,
            FullPath = @"ПРОИЗВОДСТВО\Гарниры",
            Barcodes = { "57890975627974236429" }
        });
        response.MenuItems.Add(new MenuItem
        {
            Id = "9084246",
            Article = "A1004293",
            Name = "Конфеты Коровка",
            Price = withPrice.Value ? 300 : default,
            IsWeighted = true,
            FullPath = @"ДЕСЕРТЫ\Развес"
        });
        return Task.FromResult(response);
    }

    public override Task<SendOrderResponse> SendOrder(Order request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            return Task.FromResult(new SendOrderResponse
            {
                Success = false,
                ErrorMessage = "OrderId is empty"
            });
        }
        return Task.FromResult(new SendOrderResponse
        {
            Success = true,
            ErrorMessage = ""
        });
    }
}