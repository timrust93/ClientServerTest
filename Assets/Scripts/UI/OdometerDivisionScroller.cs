using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OdometerDivisionScroller : MonoBehaviour
{
    [SerializeField] private GameObject _parentGo;

    [Header("Basic Settings")]
    [SerializeField] private OdometerNumber _numberPref;
    [SerializeField] private float _heightOfElement;
    [SerializeField] private int _count;
    [SerializeField] private float _moveSpeed;

    [Header("Runtime/Circle values")]
    [SerializeField] private float _c;
    [SerializeField] private float _oneAngle;
    private float _zeroAnglePos;
    private float _fullRoundAnglePos;

    [Header("Runtime/State values")]
    [SerializeField] private List<RectTransform> _allRtList;
    [SerializeField] private int _targetVal;
    [SerializeField] private float _currentAngle;
    [SerializeField] private float _targetAngle;
    
    private float _offsetAngle = -180;
    private Tween _moveTween;


    // Start is called before the first frame update
    void Start()
    {
        //Initialize(_count, _heightOfElement, _numberPref, 0, _moveSpeed);
    }


    // Update is called once per frame
    void Update()
    {
        PositionElementsByAngles();        
    }

    #region initialization
    public void Initialize(int count, float heightOfElement, OdometerNumber numberPrefab, int startingNumber, float moveSpeed)
    {
        _moveSpeed = moveSpeed;
        _count = count;
        _heightOfElement = heightOfElement;
        _numberPref = numberPrefab;
        InitializeCircleValues();
        CreateNumbers();
        _currentAngle = CountAngleOfValue(startingNumber);
        PositionElementsByAngles();
    }

    private void InitializeCircleValues()
    {
        _c = _count * _heightOfElement;
        _oneAngle = 360f / _count;
        float _halfHeight = _c / 2;
        _zeroAnglePos = -_halfHeight;
        _fullRoundAnglePos = _halfHeight;
        _currentAngle = _offsetAngle;
    }

    private void CreateNumbers()
    {
        for (int i = 0; i < _count; i++)
        {
            OdometerNumber oNum = Instantiate(_numberPref);
            oNum.SetNumber(i);
            oNum.gameObject.name = "odometerNum_" + i;
            oNum.transform.SetParent(_parentGo.transform, false);
            _allRtList.Add(oNum.GetComponent<RectTransform>());
        }
    }
    #endregion

    #region positioning
    private void PositionElementsByAngles()
    {
        for (int i = 0; i < _allRtList.Count; i++)
        {
            var rt = _allRtList[i];
            float angle = i * _oneAngle + _currentAngle;

            float angleRem = angle % 360f;
            if (angle < 0)
                angleRem = 360 + angleRem;

            float arcLen = (angleRem) / 360f;
            float y = Mathf.Lerp(_zeroAnglePos, _fullRoundAnglePos, arcLen);
            rt.anchoredPosition = new Vector2(0, y);
        }
    }

    private void MoveToAngle()
    {
        float moveTime = Mathf.Abs(_targetAngle - _currentAngle) / _moveSpeed;
        if (_moveTween != null)
            _moveTween.Kill();
        _moveTween = DOTween.To(() => _currentAngle, x => _currentAngle = x, _targetAngle, moveTime);
    }

    public void MoveToValue(int value)
    {
        _targetAngle = CountAngleOfValue(value);
        MoveToAngle();
    }
    #endregion

    private float CountAngleOfValue(int forValue)
    {
        int fullRounds = Mathf.CeilToInt((_currentAngle - _offsetAngle) / 360f);
        float targetVal = fullRounds * 360f + (-180) - _oneAngle * forValue;
        return targetVal;
    }


    public void MoveButton()
    {
        _targetAngle = CountAngleOfValue(_targetVal);
        MoveToAngle();
    }

    private float AngleToDist(float angle)
    {
        float dist = Mathf.Lerp(0, _c, angle / 360f);
        return dist;
    }

    private float DistToAngle(float dist)
    {
        int sign = dist > 0 ? 1 : -1;
        int fullRounds = Mathf.FloorToInt(Mathf.Abs(dist) / _c);
        float restDist = dist - _c * fullRounds;
        float angle = Mathf.Lerp(0, 360, Mathf.Abs(restDist / _c));
        angle = sign * angle + sign * (fullRounds * 360f);
        return angle;
    }
}
