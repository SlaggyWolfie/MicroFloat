using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private float xSpin;
    private float ySpin;
    private float zSpin;

    private Vector3 _speed = new Vector3(0.02f, 0.06f, 0.05f);

    void Start()
    {
        _speed = new Vector3(Random.Range(0.4f, 2.3f), Random.Range(0.5f, 1.5f), Random.Range(0.5f, 2.4f));
        xSpin = Random.Range(0, 360);
        ySpin = Random.Range(0, 360);
        zSpin = Random.Range(0, 360);
    }

    void Update()
    {
        xSpin += _speed.x * Time.deltaTime;
        ySpin += _speed.y * Time.deltaTime;
        zSpin += _speed.z * Time.deltaTime;

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(xSpin, ySpin, zSpin));
    }
}
