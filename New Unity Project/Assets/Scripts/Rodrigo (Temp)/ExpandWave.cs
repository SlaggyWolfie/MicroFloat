using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandWave : MonoBehaviour
{
    public static float stress = 3f;
    public Light sceneLight;

    public float frequency = 1;
    //Color color0 = new Color(160, 160, 160, 50);
    //Color color1 = new Color(210, 210, 210, 50);
    Color color0 = new Color(0.3f, 0.3f, 0.3f);
    Color color1 = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        frequency = frequency / 100;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localScale *= 1.25f;
        //if(transform.localScale.magnitude > 200)
        //{
        //    transform.localScale = Vector3.one;
        //}
        print("Hwllo");
        float st = Mathf.Sin(((20000 / sceneLight.colorTemperature)) * Time.time * frequency);
        //float t = Mathf.PingPong(Time.time, frequency) / (sceneLight.colorTemperature / 20000);
        sceneLight.color = Color.Lerp(color0, color1, st);

    }
}
