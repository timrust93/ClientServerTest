using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net.NetworkInformation;

public class ClientConnect : MonoBehaviour
{
    private const int sendChunkSize = 256;
    private const int receiveChunkSize = 64;
    public static ClientWebSocket _webSocket;
    public static TcpClient tcpClient;
    public static string OdometerValueMessage { get; private set; }

    private static Thread readServerDataThread;

    public static void ConnectNew()
    {       
        if (readServerDataThread != null)
        {
            readServerDataThread.Abort();
            
        }
        readServerDataThread = new Thread(ConnectThreadMethod);
        readServerDataThread.Start();        
    }

    private static void ConnectThreadMethod()
    {
        Connect("ws://185.246.65.199:9090/ws", () =>
        {
            Debug.Log("connect callback");                        
        }).Wait();
        Task.WhenAll(Receive(_webSocket));
    }

    private static async Task Connect(string uri, System.Action onConnected)
    {
        Debug.Log("connect acti");
        _webSocket = null;

        try
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            onConnected?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.Log($"Exception: {ex}");
        }
        finally
        {

        }
    }


    private static async Task Send(ClientWebSocket webSocket, byte[] data)
    {
        while (webSocket.State == WebSocketState.Open)
        {     
            await webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, false, CancellationToken.None);
            //LogStatus(false, data, data.Length);
            //await Task.Delay(delay);
        }
    }

    private static async Task Receive(ClientWebSocket webSocket)
    {
        byte[] buffer = new byte[receiveChunkSize];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                string receivedMessage = Encoding.ASCII.GetString(buffer);                
                Debug.Log(receivedMessage);
                OdometerValueMessage = receivedMessage;                
            }
        }
    }


    public static bool IsSocketConnected()
    {
        if (_webSocket == null)
            return false;
        if (_webSocket.State == WebSocketState.Aborted)
            return false;
        return _webSocket.State == WebSocketState.Open;
    }

    public static bool IsSocketConnecting()
    {
        if (_webSocket == null)
            return false;
        return _webSocket.State == WebSocketState.Connecting;
    }

    public static string StateOfWebSocket()
    {
        return _webSocket.State.ToString();
    }


    public static void CloseWebsocket()
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "appClose", CancellationToken.None);
    }

    public static bool IsInternetOnUnity()
    {
        return false;
    }
}


// OLD
//class Client
//{
//    private static object consoleLock = new object();
//    private const int sendChunkSize = 256;
//    private const int receiveChunkSize = 64;
//    private const bool verbose = true;
//    private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(1000);
//    public static ClientWebSocket _webSocket;
//    public static TcpClient tcpClient;

//    public static void Main()
//    {
//        byte[] data = Encoding.ASCII.GetBytes(JsonStuff.getOdometerValJSON);

//        //Connect("ws://185.246.65.199:9090/ws").Wait();
//        //tcpClient = new TcpClient("185.246.65.199", 9090);
//        //Debug.Log("connected: " +  tcpClient.Connected);

//        Connect("ws://185.246.65.199:9090/ws", () =>
//        {
//            Debug.Log("connect callback");
//            //Send(_webSocket, data);
//            //byte[] data2 = Encoding.UTF8.GetBytes(JsonStuff.getRandomStatusJSON);
//            //Send(_webSocket, data2);
//        }).Wait();
//        Task.WhenAll(Receive(_webSocket));
//    }

//    public static async Task Connect(string uri, System.Action finsh)
//    {
//        _webSocket = null;

//        try
//        {
//            _webSocket = new ClientWebSocket();
//            await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
//            //await Task.WhenAll(Receive(_webSocket), Send(_webSocket));
//        }
//        catch (Exception ex)
//        {
//            Debug.Log($"Exception: {ex}");
//        }
//        finally
//        {
//            //if (_webSocket != null)
//            //    _webSocket.Dispose();
//            //lock (consoleLock)
//            //{                    
//            //    Debug.Log("WebSocket closed.");                    
//            //}
//        }
//    }


//    private static async Task Send(ClientWebSocket webSocket, byte[] data)
//    {
//        //var random = new System.Random();
//        //byte[] buffer = new byte[sendChunkSize];

//        while (webSocket.State == WebSocketState.Open)
//        {
//            //random.NextBytes(buffer);
//            //buffer = Encoding.ASCII.GetBytes(JsonStuff.testRequest);

//            await webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, false, CancellationToken.None);
//            LogStatus(false, data, data.Length);

//            await Task.Delay(delay);
//        }
//    }

//    private static async Task Receive(ClientWebSocket webSocket)
//    {
//        byte[] buffer = new byte[receiveChunkSize];
//        while (webSocket.State == WebSocketState.Open)
//        {
//            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//            if (result.MessageType == WebSocketMessageType.Close)
//            {
//                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
//            }
//            else
//            {
//                string receivedMessage = Encoding.ASCII.GetString(buffer);
//                Debug.Log(receivedMessage);
//                //LogStatus(true, buffer, result.Count);
//            }
//        }
//    }

//    private static void LogStatus(bool receiving, byte[] buffer, int length)
//    {
//        lock (consoleLock)
//        {
//            Console.ForegroundColor = receiving ? ConsoleColor.Green : ConsoleColor.Gray;
//            Console.WriteLine("{0} {1} bytes... ", receiving ? "Received" : "Sent", length);

//            if (verbose)
//                Console.WriteLine(BitConverter.ToString(buffer, 0, length));
//        }
//    }
//}
