open System
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Threading;
open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Binance.Common
open Binance.Spot
open Binance.Spot.Models


[<EntryPoint>]
let main argv =
    let loggerFactory = LoggerFactory.Create(fun (builder:ILoggingBuilder) -> 
        builder.AddConsole() |> ignore
    )
    let logger = loggerFactory.CreateLogger()

    let websocket = new WebSocketApi("wss://ws-api.testnet.binance.vision/ws-api/v3", "apiKey", new BinanceHmac("apiSecret"));

    let onlyOneMessage = new TaskCompletionSource<string>();

    websocket.OnMessageReceived(
        (fun data ->
            onlyOneMessage.SetResult(data)
            Task.CompletedTask)
    );

    websocket.ConnectAsync() |> Async.AwaitTask |> Async.RunSynchronously

    websocket.AccountTrade.CancelOcoOrderAsync(symbol = "BNBUSDT", listClientOrderId = "listClientOrderId") |> Async.AwaitTask |> Async.RunSynchronously

    let message = onlyOneMessage.Task |> Async.AwaitTask |> Async.RunSynchronously

    logger.LogInformation(message);

    websocket.DisconnectAsync() |> Async.AwaitTask |> Async.RunSynchronously
    
    0
    