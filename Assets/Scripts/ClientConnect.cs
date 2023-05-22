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

    [SerializeField] private ConfigsManager _configsManager;
    //private static string link;


    public static void ConnectNew(string link)
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
        Connect(ConfigsManager.Instance.SocketLink, () =>
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
