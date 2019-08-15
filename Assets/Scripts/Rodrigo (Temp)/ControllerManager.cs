using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    [SerializeField]
    private bool _debugMessages = false;

    public float controlsUpdateTime = 2;
    //[ReadOnly]
    public string controllerSuffix = "";

    [Header("Controller")]
    public bool useController = true;
    public ControllerTypes controllerType = default(ControllerTypes);
    public enum ControllerTypes { PS4, XBOX }
    
    private void Awake()
    {
        controlsUpdate += ControllerCheck;
        StartCoroutine(constantVariableUpdate());

        //reference provided by the Game Manager
        //GameManager.Instance.ControllerManager = this;
    }

    public delegate void ControlsUpdateDelegate();
    public event ControlsUpdateDelegate controlsUpdate;
    
    private IEnumerator constantVariableUpdate()
    {
        while (true)
        {
            ControllerCheck();
            yield return new WaitForSeconds(controlsUpdateTime);
        }
    }

    public void ControllerCheck()
    {
        string[] currentControllers = new string[Input.GetJoystickNames().Length];
        int numberOfControllers = 0;
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            currentControllers[i] = Input.GetJoystickNames()[i].ToLower();
            if ((currentControllers[i] == "controller (xbox 360 for windows)"
                || currentControllers[i] == "controller (xbox 360 wireless receiver for windows)"
                || currentControllers[i] == "controller (xbox one for windows)"))
            {
                useController = true;
                controllerType = ControllerTypes.XBOX;
                controllerSuffix = "XBOX";
            }
            else if (currentControllers[i] == "wireless controller")
            {
                useController = true;
                controllerType = ControllerTypes.PS4;
                controllerSuffix = "PS";
            }
            else if (currentControllers[i] == "")
            {
                numberOfControllers++;
            }

            if (currentControllers[i] != "")
            {
                if (_debugMessages) Debug.Log(currentControllers[i] + " is detected.");
            }
        }

        if (numberOfControllers == Input.GetJoystickNames().Length)
        {
            useController = false;
            controllerSuffix = "";
            if(_debugMessages) Debug.Log("No controller found, using mouse and keyboard settings!");
        }
    }
}
