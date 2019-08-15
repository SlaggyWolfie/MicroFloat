using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField]
    private float _growTimer = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Slavi.CoroutineHelpers.Wait(_growTimer, SendAndRescheduleGrowEvent));
    }

    private void SendAndRescheduleGrowEvent()
    {
        Slavi.EventQueue.Instance.Send(new Slavi.ObstacleGrowEvent());
        StartCoroutine(Slavi.CoroutineHelpers.Wait(_growTimer, SendAndRescheduleGrowEvent));
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            Slavi.EventQueue.Instance.Send(new Slavi.ObstacleGrowEvent());
    }
}
