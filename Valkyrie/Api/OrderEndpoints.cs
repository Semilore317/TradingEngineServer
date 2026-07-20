using Valkyrie.Api.Dto;

namespace Valkyrie.Api;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapPost("/orders", (PlaceOrderRequest request, OrderGateway gateway) =>
        {
            try
            {
                var ack = gateway.Submit(request);
                return Results.Created($"/orders/{ack.OrderId}", ack);
            }
            catch (InvalidOperationException exception)
            {
                return Results.NotFound(exception.Message);
            }
        });

        app.MapDelete("/instruments/{securityId:long}/orders/{id:long}",
            (long id, long securityId, string username, OrderGateway gateway) =>
            {
                gateway.Cancel(id, securityId, username);
                return Results.NoContent();
            }
        );

        app.MapGet("/book/{securityId:long}", (long securityId, OrderGateway gateway) =>
        
            gateway.TryGetBook(securityId, out var book)
                ? Results.Ok(book)
                : Results.NotFound()
        );

        app.MapPut("orders/{id:long}", (long id, ModifyOrderRequest request, OrderGateway gateway) =>
        {
            if(id != request.OrderId)
                return Results.BadRequest("Order ID Mismatch between payload and route parameter");

            try
            {
                var ack = gateway.Modify(request);
                return Results.Ok(ack);
            }
            catch (InvalidOperationException e)
            {
                return Results.NotFound(e.Message);
            }
        });
    }
}