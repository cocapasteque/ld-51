using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspect : MonoBehaviour
{
    public float BaseAspect = 16f / 9f;

    private Camera _cam;
    private float _baseSize;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        _baseSize = _cam.orthographicSize;
        _cam.orthographicSize = BaseAspect >= _cam.aspect ? (BaseAspect / _cam.aspect * _baseSize) : _baseSize;
    }
}