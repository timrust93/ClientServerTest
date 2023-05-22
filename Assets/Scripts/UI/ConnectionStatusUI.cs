using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectionStatusUI : MonoBehaviour
{
    [SerializeField] private ConnectionManager _mainManager;

    [SerializeField] private Image _onlineIndicator;
    [SerializeField] private Color _onColor;
    [SerializeField] private Color _offColor;

    [SerializeField] private string _statusOnlineStr;
    [SerializeField] private string _statusOfflineStr;
    [SerializeField] private string _statusNoInternetStr;

    [SerializeField] private TextMeshProUGUI _connectionStatusTmp;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        _mainManager.OnConnectionStatusChanged += UpdateUI;
    }


    private void UpdateUI(ConnectionStatus status)
    {
        if (status == ConnectionStatus.Online)
        {
            _onlineIndicator.color = _onColor;
            _connectionStatusTmp.text = _statusOnlineStr;
        }
        else if (status == ConnectionStatus.Offline)
        {
            _onlineIndicator.color = _offColor;
            _connectionStatusTmp.text = _statusOfflineStr;
        }
        else if (status == ConnectionStatus.NoInternet)
        {
            _onlineIndicator.color = _offColor;
            _connectionStatusTmp.text = _statusNoInternetStr;
        }
    }

    private void OnDisable()
    {
        _mainManager.OnConnectionStatusChanged -= UpdateUI;
    }

    private void OnDestroy()
    {
        _mainManager.OnConnectionStatusChanged -= UpdateUI;
    }
}
