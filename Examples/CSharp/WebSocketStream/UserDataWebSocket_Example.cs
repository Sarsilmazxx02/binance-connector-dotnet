namespace Binance.Spot.UserDataWebSocketExamples
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Common;
    using Binance.Spot;
    using Binance.Spot.Models;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class UserDataWebSocket_Example
    {
        public static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<UserDataWebSocket_Example>();
            HttpMessageHandler loggingHandler = new BinanceLoggingHandler(logger: logger);
            HttpClient httpClient = new HttpClient(handler: loggingHandler);

            // Create listen key
            var baseUrl = "https://testnet.binance.vision";
            var apiKey = "apiKey";
            var apiSecret = "apiSecret";

            var userDataStreams = new UserDataStreams(httpClient, baseUrl, apiKey: apiKey, apiSecret: apiSecret);
            var response = await userDataStreams.CreateSpotListenKey();
            string listenKey = JsonConvert.DeserializeObject<dynamic>(response).listenKey.ToString();

            // Subscribe to user data stream
            var websocket = new UserDataWebSocket(listenKey, "wss://stream.testnet.binance.vision");

            var onlyOneMessage = new TaskCompletionSource<string>();
            websocket.OnMessageReceived(
                async (data) =>
            {
                onlyOneMessage.SetResult(data);
                await Task.CompletedTask;
            }, CancellationToken.None);

            await websocket.ConnectAsync(CancellationToken.None);
            string message = await onlyOneMessage.Task;
            logger.LogInformation(message);
            await websocket.DisconnectAsync(CancellationToken.None);
        }
    }
}