using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjMover : MonoBehaviour
{
    public Transform Start, End;
    public Vector2 SpeedRange;

    private float _speed;

    private void Awake()
    {
        _speed = Random.Range(SpeedRange.x, SpeedRange.y);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);
        if (transform.position.x > End.position.x)
        {
            ResetPos();
        }
    }

    private void ResetPos()
    {
        transform.position = new Vector3(Start.position.x, transform.position.y, transform.position.z);
        _speed = Random.Range(SpeedRange.x, SpeedRange.y);
    }
}
