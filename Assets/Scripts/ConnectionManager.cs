using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private ConnectionStatus _connectionStatus;
    [SerializeField] private ConfigsManager _configsManager;

    public ConnectionStatus ConnectionStatus => _connectionStatus;
    public System.Action<ConnectionStatus> OnConnectionStatusChanged;

    private float _internetCheckTimeOut = 2f;
    private float _internetCheckTimer;

    // Start is called before the first frame update
    void Start()
    {        
        _connectionStatus = ConnectionStatus.Offline;
        OnConnectionStatusChanged?.Invoke(_connectionStatus);        
        ClientConnect.ConnectNew(_configsManager.SocketLink);
    }

    private void Update()
    {
        ManageConnection();
    }

    private void ManageConnection()
    {
        if (_internetCheckTimer > _internetCheckTimeOut)
        {
            CheckConnection();
            _internetCheckTimer = 0;
        }
        if (_connectionStatus == ConnectionStatus.NoInternet)
        {
            OnConnectionStatusChanged?.Invoke(_connectionStatus);
        }
        _internetCheckTimer += Time.deltaTime;
        if (ClientConnect.IsSocketConnected())
        {
            _connectionStatus = ConnectionStatus.Online;
            OnConnectionStatusChanged?.Invoke(_connectionStatus);
        }
        else
        {
           if (_connectionStatus != ConnectionStatus.NoInternet)
            {
                _connectionStatus = ConnectionStatus.Offline;
                OnConnectionStatusChanged?.Invoke(_connectionStatus);
                if (!ClientConnect.IsSocketConnecting())
                    ClientConnect.ConnectNew(_configsManager.SocketLink);
            }
        }    
    }

    #region Internet checking
    public void CheckConnection()
    {
        //Check if the device cannot reach the internet at all (that means if the "cable", "WiFi", etc. is connected or not)
        //if not, don't waste your time.
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _connectionStatus = ConnectionStatus.NoInternet;
        }
        else
        {
            StartCoroutine(DoPing()); //It could be a network connection but not internet access so you have to ping your host/server to be sure.
        }
    }

    IEnumerator DoPing()
    {
        TestPing.DoPing();
        yield return new WaitUntil(() => TestPing.isDone);
        bool connected = TestPing.status;

        if (connected)
        {
            if (_connectionStatus == ConnectionStatus.NoInternet)
                _connectionStatus = ConnectionStatus.Undefined;
        }
        else
        {
            _connectionStatus = ConnectionStatus.NoInternet;
        }
    }

    #endregion


    private void OnApplicationQuit()
    {
        ClientConnect.CloseWebsocket();        
    }
}
