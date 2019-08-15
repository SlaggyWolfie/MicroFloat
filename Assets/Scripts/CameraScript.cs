using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public FMODUnity.StudioEventEmitter pressASoundEmitter = null;

    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _cameraBox;
    private Transform _distObject;

    public float smoothTime = 0.15f;
    public Vector3 cameraOffset;

    public bool yMaxEnabled = false;
    public float yMaxValue = 0;

    public bool yMinEnabled = false;
    public float yMinValue = 0;

    public bool xMaxEnabled = false;
    public float xMaxValue = 0;

    public bool xMinEnabled = false;
    public float xMinValue = 0;

    public bool zMinEnabled = false;
    public float zMinValue = 0;

    public bool zMaxEnabled = false;
    public float zMaxValue = 0;

    private bool start = false;

    Vector3 cameraVelocity = Vector3.zero;
    Vector3 cameraPos;

    Camera cam;
    float startingFOV;

    void Start()
    {
        cameraPos = _cameraBox.transform.position;
        cameraPos.z = _player.gameObject.transform.position.z;

        cam = GameObject.FindObjectOfType<Camera>();
        startingFOV = cam.fieldOfView;

        StartCoroutine(wait(() => shouldZoom = true, 5));
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown("joystick button 0") && !start)
        {
            start = true;
            if (pressASoundEmitter != null)
                pressASoundEmitter.Play();
        }
        if (!start) return;
        xMinValue = 0;
        cameraPos = _cameraBox.transform.position;
        Vector3 targetPos = _player.gameObject.transform.position;
        //y
        if (yMaxEnabled && yMinEnabled)
        {
            targetPos.y = Mathf.Clamp(targetPos.y, yMinValue, yMaxValue);
        }
        else if (yMinEnabled)
        {
            targetPos.y = Mathf.Clamp(targetPos.y, yMinValue, targetPos.y);
        }
        else if (yMaxEnabled)
        {

            targetPos.y = Mathf.Clamp(targetPos.y, targetPos.y, yMaxValue);
        }
        //x
        if (xMaxEnabled && xMinEnabled)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, xMinValue, xMaxValue);
        }
        else if (xMinEnabled)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, xMinValue, targetPos.x);
        }
        else if (xMaxEnabled)
        {

            targetPos.x = Mathf.Clamp(targetPos.x, targetPos.x, xMaxValue);
        }

        //z
        if (zMaxEnabled && zMinEnabled)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, xMinValue, xMaxValue);
        }
        else if (zMinEnabled)
        {
            targetPos.z = Mathf.Clamp(targetPos.z, zMinValue, targetPos.z);
        }
        else if (zMaxEnabled)
        {

            targetPos.z = Mathf.Clamp(targetPos.z, targetPos.z, zMaxValue);
        }

        cameraPos = Vector3.SmoothDamp(cameraPos, new Vector3(targetPos.x, targetPos.y, targetPos.z) + cameraOffset, ref cameraVelocity, smoothTime);

        _cameraBox.transform.position = cameraPos;
        Zoom(0);
    }

    bool shouldZoom = false;
    public void Zoom(float value)
    {
        if (shouldZoom)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, startingFOV + _player.GetComponent<PlayerMovement>().Velocity.magnitude, Time.deltaTime);
    }

    private IEnumerator wait(System.Action action, float time)
    {
        yield return new WaitForSeconds(time);
        if (action != null) action.Invoke();
    }
}
