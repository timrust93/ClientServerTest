using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OdometerNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _numberTmp;
    [SerializeField] private int _myNumber;

    public int MyNumber => _myNumber;

    public void SetNumber(int number)
    {
        _myNumber = number;
        _numberTmp.text = number.ToString();
    }
}
