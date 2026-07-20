using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Valkyrie.Api.MarketData;

public static class MarketDataEndpoints
{
    private record ClientMessage(string Action, long SecurityId);

    public static void MapMarketDataEndpoints(this WebApplication app)
    {
        app.Map("/ws/marketdata", async (HttpContext context, MarketDataHub hub, OrderGateway gateway)
            =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
                context.Response.StatusCode = 400;

            // this waits for the 101 status code so the app starts using ws to communicate with the browser
            using var socket = await context.WebSockets.AcceptWebSocketAsync();
            var connection = new MarketDataConnection(socket);

            // kick off the background loop that empties this connection's outbound queue into the socket
            var sending = connection.SendLoopAsync(context.RequestAborted);

            // honestly overkill... but it maps neatly to a single frame...
            // no point causing internal fragmentation for no gain 
            var buffer = new byte[4096];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer, context.RequestAborted);
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var message = JsonSerializer.Deserialize<ClientMessage>(text,
                        new JsonSerializerOptions(JsonSerializerDefaults.Web));

                    if (message == null)
                        continue;

                    if (message.Action == "subscribe")
                    {
                        // send a snapshot ASAP so the client isn't blind till the next trade
                        hub.Subscribe(connection, message.SecurityId);

                        if (gateway.TryGetBook(message.SecurityId, out var snapshot))
                            if (snapshot != null)
                                connection.Enqueue(JsonSerializer.SerializeToUtf8Bytes(
                                    new
                                    {
                                        type = "Book",
                                        snapshot.SecurityId,
                                        snapshot.Bid,
                                        snapshot.Ask,
                                        snapshot.Spread,
                                        snapshot.Bids,
                                        snapshot.Asks
                                    }, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
                    }
                    else if (message.Action == "unsubscribe")
                    {
                        hub.Unsubscribe(connection, message.SecurityId);
                    }
                }
            }
            catch (OperationCanceledException) // at some point the client is gonna disappear 
            {
                /*No_Op*/
            } 
            finally
            {
                hub.RemoveEveryWhere(connection);
                connection.Complete(); // allow the send loop finish
                await sending;
            }
        });
    }
}