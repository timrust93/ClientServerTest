using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Net;
using System.Linq;

public class ConfigsManager : MonoBehaviour
{
    private const int ignoreTillLineIndex = 1;

    private const string SERVER_IP_Key = "Server address";
    private const string SERVER_PORT_KEY = "Port";
    private const string VIDEO_LINK_KEY = "Video link";

    private Dictionary<string, string> _configsDict = new Dictionary<string, string>();
    private Dictionary<string, int> _configKeyToLineDict = new Dictionary<string, int>();

    #region config properties
    public string ServerIPAdress
    {
        get
        {
            string correctKey = SERVER_IP_Key;
            if (ValidateIP(_configsDict[correctKey]))
            {
                return _configsDict[correctKey];
            }
            else
            {
                if (_configKeyToLineDict.ContainsKey(correctKey))
                {
                    throw new Exception("Config file error in line " + _configKeyToLineDict[correctKey] + " Directory: " + Application.streamingAssetsPath);
                }
                throw new Exception(@"Config file error!  Add """ + correctKey + @" :""  followed by a valid ip address to you config file. Directory: " + Application.streamingAssetsPath);
            }
        }
        set
        {
            string correctKey = SERVER_IP_Key;
            string newValue = correctKey + " : " + value;
            if (_configKeyToLineDict.ContainsKey(SERVER_IP_Key))
            {
                ModifyLineInConfigFile(newValue, _configKeyToLineDict[SERVER_IP_Key]);
            }
            else
            {
                AddNewLineInConfigFile(newValue);
            }                        
        }
    }
    public int ServerPortNumber
    {
        get
        {
            if (ValidatePort(_configsDict[SERVER_PORT_KEY]))
            {
                return Convert.ToInt32(_configsDict[SERVER_PORT_KEY]);
            }     
            else
            {
                if (_configKeyToLineDict.ContainsKey(SERVER_PORT_KEY))
                {
                    throw new Exception("Config file error in line " + _configKeyToLineDict[SERVER_PORT_KEY] + " Directory: " + Application.streamingAssetsPath);
                }
                throw new Exception(@"Config file error! Add """ + SERVER_PORT_KEY + @" : "" followed by a valid port number to your config file. Directory: " + Application.streamingAssetsPath);
            }
        }    
        set
        {
            string newValue = SERVER_PORT_KEY + " : " + value;
            if (_configKeyToLineDict.ContainsKey(SERVER_PORT_KEY))
            {
                ModifyLineInConfigFile(newValue, _configKeyToLineDict[SERVER_PORT_KEY]);
            }
            else
            {
                AddNewLineInConfigFile(newValue);
            }
        }
    }

    public string VideoLink
    {
        get
        {
            return _configsDict[VIDEO_LINK_KEY];
        }
        set
        {
            string newValue = VIDEO_LINK_KEY + " : " + value;
            if (_configKeyToLineDict.ContainsKey(VIDEO_LINK_KEY))
            {
                ModifyLineInConfigFile(newValue, _configKeyToLineDict[VIDEO_LINK_KEY]);
            }
            else
            {
                AddNewLineInConfigFile(newValue);
            }
        }
    }
    #endregion

    public string ConfigFilePath()
    {
        return Application.streamingAssetsPath + "/config.txt";
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeConfigsDictionary();        
        ReadFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeConfigsDictionary()
    {
        _configsDict.Add(SERVER_IP_Key, "");
        _configsDict.Add(SERVER_PORT_KEY, "");
        _configsDict.Add(VIDEO_LINK_KEY, "");
    }

    private void ReadFile()
    {        
        string configPath = ConfigFilePath();
        File.ReadAllText(configPath);
        if (File.Exists(configPath))
        {
           using (StreamReader streamReader = new StreamReader(configPath))
           {
                int lineIndex = -1;
                while (!streamReader.EndOfStream)
                {                    
                    lineIndex += 1;
                    string line = streamReader.ReadLine();                    
                    string[] lineSplitArr = line.Split(new[] { ':' }, 2);
                    string configNameStr = lineSplitArr[0];
                    configNameStr = configNameStr.TrimEnd().TrimStart();
                    //Debug.Log("config name str: " + configNameStr + "_ _");
                    if (_configsDict.ContainsKey(configNameStr) && _configKeyToLineDict.ContainsKey(configNameStr) == false)
                    {                       
                        _configKeyToLineDict.Add(configNameStr, lineIndex);                        
                        string valueStr = lineSplitArr[1];
                        valueStr = string.Join("", valueStr.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                        _configsDict[configNameStr] = valueStr;                        
                    }                   
                }                                
           }
        }
    }

    private void ModifyLineInConfigFile(string newText, int lineIndexToChange)
    {
        string configFilePath = ConfigFilePath();
        string[] arrLine = File.ReadAllLines(configFilePath);
        arrLine[lineIndexToChange] = newText;
        File.WriteAllLines(configFilePath, arrLine);
    }

    private void AddNewLineInConfigFile(string newText)
    {
        string configFilePath = ConfigFilePath();
        File.AppendAllText(configFilePath, newText + Environment.NewLine);
    }

    public bool ValidateIP(string ipStr)
    {
        IPAddress ip = null;
        bool isIPValid = IPAddress.TryParse(ipStr, out ip);
        return isIPValid;
    }

    public bool ValidatePort(string portStr)
    {
        return int.TryParse(portStr, out int result);
    }
}
