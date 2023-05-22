using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Globalization;

public class OdometerDisplayer : MonoBehaviour
{
    [SerializeField] private List<OdometerDivisionScroller> _integerDisplayers;
    [SerializeField] private List<OdometerDivisionScroller> _floatDisplayers;
    
    [Header("Divisions settings")]
    [SerializeField] private OdometerNumber _intNumberPref;
    [SerializeField] private OdometerNumber _floatNumberPref;
    [SerializeField] private float _heightOfElement;
    [SerializeField] private int _countPerScroller;
    [SerializeField] private float _moveSpeed;
    [Header("Debug")]
    [SerializeField] private OdometerValueResp _lastOdometerValueResp;
    [SerializeField] private string _prevOdometerRecievedVal;

    /// <summary>
    /// For json data convertion double-direction
    /// </summary>
    [System.Serializable]
    private class OdometerValueResp
    {
        public string operation;
        public float value;
    }

    private void Start()
    {
        InitializeDivisionScrollers();

    }

    private void Update()
    {
        string odometerMessage = ClientConnect.OdometerValueMessage;
        if (_prevOdometerRecievedVal != odometerMessage && odometerMessage != string.Empty )
        {
            OdometerValueResp respValue = JsonUtility.FromJson<OdometerValueResp>(odometerMessage);
            if (respValue != null)
                UpdateDisplayers(respValue.value);
        }
    }

    private void InitializeDivisionScrollers()
    {
        foreach (OdometerDivisionScroller ds in _integerDisplayers)
        {
            ds.Initialize(_countPerScroller, _heightOfElement, _intNumberPref, 0, _moveSpeed);
        }
        foreach (OdometerDivisionScroller ds in _floatDisplayers)
        {
            ds.Initialize(_countPerScroller, _heightOfElement, _floatNumberPref, 0, _moveSpeed);
        }
    }

    public void UpdateDisplayers(float value)
    {
        string[] stringValArr = value.ToString("0.00", CultureInfo.InvariantCulture).Split(".");
        
        for (int i = 0; i < stringValArr[0].Length; i++)
        {
            int val = stringValArr[0][i] - '0';
            if (i < _integerDisplayers.Count)
            {               
                _integerDisplayers[i].MoveToValue(val);
            }
        }

        for (int i = 0; i < stringValArr[1].Length; i++)
        {           
            int val = stringValArr[1][i] - '0';
            if (i < _floatDisplayers.Count)
            {
                _floatDisplayers[i].MoveToValue(val);
            }
        }
    }
}
