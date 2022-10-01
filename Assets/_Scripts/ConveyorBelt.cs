using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public Transform ArrowParent;   
    public List<Transform> ArrowObjs;
    public Transform ArrowBoundsEnd;
    public float ArrowResetAmount;

    private int _lastArrowIndex;
    private float _arrowParentResetXValue;

    private void Awake()
    {
        _lastArrowIndex = 0;
        _arrowParentResetXValue = 100f * ArrowResetAmount;
}

    private void Update()
    {
        ArrowParent.transform.Translate(Vector3.right * GameManager.Instance.ConveyorSpeed * Time.deltaTime);

        if (ArrowObjs[_lastArrowIndex].position.x >= ArrowBoundsEnd.position.x)
        {
            ArrowObjs[_lastArrowIndex].localPosition -= Vector3.right * ArrowResetAmount;
            _lastArrowIndex = (_lastArrowIndex + 1) % ArrowObjs.Count;
        }

        if (ArrowParent.localPosition.x > _arrowParentResetXValue)
        {
            ArrowParent.localPosition += Vector3.left * _arrowParentResetXValue;
            for (int i = 0; i < ArrowObjs.Count; i++)
            {
                ArrowObjs[i].localPosition += Vector3.right * _arrowParentResetXValue;
            }
        }       
    }
}