using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.Linq;

namespace SmsIntegrationTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommandController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(body))
            return BadRequest("Empty body");

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(body);
        }
        catch (JsonException e)
        {
            return Ok(new { Command = "", Success = false, ErrorMessage = $"Invalid JSON: {e.Message}" });
        }

        var root = document.RootElement;
        if (!root.TryGetProperty("Command", out var commandProp))
        {
            return Ok(new { Success = false, ErrorMessage = "Command field is missing" });
        }

        if (!root.TryGetProperty("CommandParameters", out var commandParameters))
        {
            return Ok(new { Success = false, ErrorMessage = "Command parameters are missing" });
        }

        string command = commandProp.GetString();
        switch (command)
        {
            case "GetMenu":
                return Ok(GetMenuResponse(commandParameters));
            case "SendOrder":
                return Ok(SendOrderResponse(root));
            default:
                return Ok(new { Command = command, Success = false, ErrorMessage = $"Unknown command: {command}" });
        }
    }

    private object GetMenuResponse(JsonElement commandParameters)
    {
        if (!commandParameters.TryGetProperty("WithPrice", out var withPriceProp))
        {
            return new
            {
                Command = "GetMenu",
                Success = false,
                ErrorMessage = "WithPrice flag is missing"
            };
        }
        var withPrice = withPriceProp.GetBoolean();

        return new
        {
            Command = "GetMenu",
            Success = true,
            ErrorMessage = "",
            Data = new
            {
                MenuItems = new[]
                {
                    new {
                        Id = "5979224",
                        Article = "A1004292",
                        Name = "Каша гречневая",
                        Price = withPrice ? 50 : default,
                        IsWeighted = false,
                        FullPath = "ПРОИЗВОДСТВО\\Гарниры",
                        Barcodes = new[] { "57890975627974236429" }
                    },
                    new {
                        Id = "9084246",
                        Article = "A1004293",
                        Name = "Конфеты Коровка",
                        Price = withPrice ? 300 : default,
                        IsWeighted = true,
                        FullPath = "ДЕСЕРТЫ\\Развес",
                        Barcodes = new string[] { }
                    }
                }
            }
        };
    }

    private object SendOrderResponse(JsonElement commandParameters)
    {
        if (!commandParameters.TryGetProperty("OrderId", out var orderIdProp) ||
            string.IsNullOrEmpty(orderIdProp.GetString()))
        {
            return new
            {
                Command = "SendOrder",
                Success = false,
                ErrorMessage = "OrderId is missing or empty"
            };
        }

        return new
        {
            Command = "SendOrder",
            Success = true,
            ErrorMessage = ""
        };
    }
}