using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ControlWrapper
{
    [SerializeField]
    private string _unityStringAxis = null;

    public ControlWrapper()
    {
    }

    public ControlWrapper(string inputAxis)
    {
        _unityStringAxis = inputAxis;
    }

    public void SetAxis(string inputAxis)
    {
        _unityStringAxis = inputAxis;
    }

    #region Axis Raw
    public float GetAxisRaw()
    {
        return Input.GetAxisRaw(_unityStringAxis);
    }


    public bool GetAxisRawBool()
    {
        return (Input.GetAxisRaw(_unityStringAxis) != 0);
    }

    public bool GetAxisRawOnce()
    {
        return Input.GetButtonDown(_unityStringAxis);
    }

    public float GetInvertedAxisRaw(bool pInvert)
    {
        return GetAxisRaw() * (pInvert ? -1 : 1);
    }
    #endregion
    #region Axis
    public float GetAxis()
    {
        return Input.GetAxis(_unityStringAxis);
    }

    public bool GetAxisBool()
    {
        return (Input.GetAxis(_unityStringAxis) != 0);
    }

    public bool GetAxisOnce()
    {
        return Input.GetButtonDown(_unityStringAxis);
    }

    public float GetInvertedAxis(bool pInvert)
    {
        return GetAxis() * (pInvert ? -1 : 1);
    }
    #endregion
}
