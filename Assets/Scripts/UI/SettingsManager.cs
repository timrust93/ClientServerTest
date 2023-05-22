using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private ConfigsManager _configsManager;
    [SerializeField] private GameObject _settingsPanel;

    [Header("Network Inputs")]
    [SerializeField] private TMP_InputField _serverIpInputF;
    [SerializeField] private TMP_InputField _portInputF;
    [SerializeField] private TMP_InputField _videoLinkInputF;

    [Header("Sounds")]
    [SerializeField] private Button _toggleSoundsBtn;
    [SerializeField] private Sprite _soundOnSprite;
    [SerializeField] private Sprite _soundOffSprite;
    [SerializeField] private Slider _audioVolumeSld;

    [Header("Errors")]
    [SerializeField] private GameObject _settingsErrorPanel;
    [SerializeField] private TextMeshProUGUI _settingsErrorMessageTmp;
    [SerializeField] private string beginErrorMessage;

    [Header("Success")]
    [SerializeField] private GameObject _settingsSuccessPanel;

    // Start is called before the first frame update
    void Start()
    {
        SetLookSoundToggle();
        _audioVolumeSld.onValueChanged.AddListener(delegate { VolumeSliderChanged(); });
        _audioVolumeSld.value = SoundManager.Inst.SoundVol;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
   

    private void UpdateServerSettingsUI()
    {
        _serverIpInputF.text = _configsManager.ServerIPAdress;
        _portInputF.text = _configsManager.ServerPortNumber.ToString();
        _videoLinkInputF.text = _configsManager.VideoLink;        
    }

    public void OpenButton()
    {
        _settingsPanel.gameObject.SetActive(true);
        UpdateServerSettingsUI();
        SoundManager.Inst.PlayDefaultButtonSound();
    }

    public void CloseButton()
    {
        _settingsPanel.gameObject.SetActive(false);
        SoundManager.Inst.PlayDefaultButtonSound();
    }

    public void CloseErrorPanelButton()
    {
        _settingsErrorPanel.gameObject.SetActive(false);
        UpdateServerSettingsUI();
        SoundManager.Inst.PlayDefaultButtonSound();
    }

    public void CloseSuccesPanelButton()
    {
        _settingsSuccessPanel.gameObject.SetActive(false);
        SoundManager.Inst.PlayDefaultButtonSound();
    }

    public void ToggleSoundsButton()
    {
        SoundManager.Inst.ToggleMusic();
        SoundManager.Inst.PlayDefaultButtonSound();
        SetLookSoundToggle();
    }

    private void SetLookSoundToggle()
    {
        Sprite spriteForButton = SoundManager.Inst.SoundOn ? _soundOnSprite : _soundOffSprite;
        _toggleSoundsBtn.image.sprite = spriteForButton;
    }

    private void VolumeSliderChanged()
    {
        SoundManager.Inst.SetSoundVolume(_audioVolumeSld.value);        
    }

    public void ApplyConnectionsButton()
    {

        string errorMessageAdd = "";
        string ipValue = _serverIpInputF.text.TrimStart().TrimEnd();
        if (_configsManager.ValidateIP(ipValue) == false)
        {
            errorMessageAdd += " server IP; ";
        }
        else
        {
            _configsManager.ServerIPAdress = ipValue;
        }

        string portValue = _portInputF.text.TrimStart().TrimEnd();
        if (_configsManager.ValidatePort(portValue) == false)
        {
            errorMessageAdd += " server port; ";            
        }
        else
        {
            _configsManager.ServerPortNumber = System.Convert.ToInt32(portValue);
        }

        _configsManager.VideoLink = _videoLinkInputF.text;
        
        if (errorMessageAdd != string.Empty)
        {
            _settingsErrorPanel.gameObject.SetActive(true);
            _settingsErrorMessageTmp.text = beginErrorMessage + "\n" + errorMessageAdd;
            _settingsErrorMessageTmp.text = _settingsErrorMessageTmp.text + "\nAnyway restart app for changes!";
        }
        else
        {
            _settingsSuccessPanel.gameObject.SetActive(true);
        }
        SoundManager.Inst.PlayDefaultButtonSound();
    }
}
