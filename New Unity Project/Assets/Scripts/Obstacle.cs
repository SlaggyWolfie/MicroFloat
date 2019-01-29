
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, ICollidable
{
    float scaleFactor;
    Vector3 initialScale;
    bool done;
    public GameObject ob;
    void Start()
    {
        this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        done = false;
        scaleFactor = Random.Range(0.8f, 1.2f);
        this.transform.localRotation = Quaternion.Euler(
            new Vector3(Random.Range(0, 90), Random.Range(0, 35), Random.Range(0, 90)));


    }

    void Update()
    {

        initialScale = Vector3.Lerp(this.transform.localScale, Vector3.one * scaleFactor, Time.deltaTime);
        this.transform.localScale = initialScale;
    }

}
