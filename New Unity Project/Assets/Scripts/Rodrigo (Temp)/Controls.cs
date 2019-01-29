using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    private ControllerManager _controllerManager = null;

    //private bool _buttonHold = false;
    //private bool _useInvertedControlsUpsideDown = false;

    private ControlWrapper m_interactControl = null;
    private ControlWrapper m_modifierLeftButton = null;
    private ControlWrapper m_jumpControl = null;
    private ControlWrapper m_playerHorizontalInput = null;
    private ControlWrapper m_playerVerticalInput = null;
    private ControlWrapper m_cameraHorizontalInput = null;
    private ControlWrapper m_cameraVerticalInput = null;
    private ControlWrapper m_objectiveHintControl = null;
    private ControlWrapper m_menuControl = null;

    private void InitialControls()
    {
        //_useInvertedControlsUpsideDown = SettingsComponent.gl_UseInvertedControlsUpsideDown;

        //Controls
        m_cameraHorizontalInput = new ControlWrapper("HorizontalLookAround" + _controllerManager.controllerSuffix);
        m_cameraVerticalInput = new ControlWrapper("VerticalLookAround" + _controllerManager.controllerSuffix);
        m_playerHorizontalInput = new ControlWrapper("Horizontal" + _controllerManager.controllerSuffix);
        m_playerVerticalInput = new ControlWrapper("Vertical" + _controllerManager.controllerSuffix);
        m_jumpControl = new ControlWrapper("Jump" + _controllerManager.controllerSuffix);
        m_interactControl = new ControlWrapper("Interact" + _controllerManager.controllerSuffix);
        m_modifierLeftButton = new ControlWrapper("Modifier" + _controllerManager.controllerSuffix);
        m_objectiveHintControl = new ControlWrapper("ObjectiveHint" + _controllerManager.controllerSuffix);
        m_menuControl = new ControlWrapper("Menu" + _controllerManager.controllerSuffix);
    }

    private void UpdateControls()
    {
        //_useInvertedControlsUpsideDown = SettingsComponent.gl_UseInvertedControlsUpsideDown;

        //OH GOD MY EYES HURT
        m_cameraHorizontalInput.SetAxis("HorizontalLookAround" + _controllerManager.controllerSuffix);
        m_cameraVerticalInput.SetAxis("VerticalLookAround" + _controllerManager.controllerSuffix);
        m_playerHorizontalInput.SetAxis("Horizontal" + _controllerManager.controllerSuffix);
        m_playerVerticalInput.SetAxis("Vertical" + _controllerManager.controllerSuffix);
        m_jumpControl.SetAxis("Jump" + _controllerManager.controllerSuffix);
        m_interactControl.SetAxis("Interact" + _controllerManager.controllerSuffix);
        m_modifierLeftButton.SetAxis("Modifier" + _controllerManager.controllerSuffix);
        m_objectiveHintControl.SetAxis("ObjectiveHint" + _controllerManager.controllerSuffix);
        m_menuControl.SetAxis("Menu" + _controllerManager.controllerSuffix);
    }

    // Use this for initialization
    private void Start()
    {
        _controllerManager = GameManager.Instance.ControllerManager;
        Debug.Assert(_controllerManager != null);

        // Set Up Controls (for updating)
        InitialControls();
        _controllerManager.controlsUpdate += UpdateControls;
    }

    #region Control (Code) Properties
    public float HorizontalInput
    {
        get { return m_playerHorizontalInput.GetAxis(); }
    }

    public float VerticalInput
    {
        get { return m_playerVerticalInput.GetAxis(); }
    }

    public float CameraHorizontalInput
    {
        get { return m_cameraHorizontalInput.GetAxis(); }
    }

    public float CameraVerticalInput
    {
        get { return m_cameraVerticalInput.GetAxis(); }
    }

    public bool Interact
    {
        get { return m_interactControl.GetAxisOnce(); }
    }

    public bool InteractHeld
    {
        get { return m_interactControl.GetAxisBool(); }
    }

    public bool Jump
    {
        get { return m_jumpControl.GetAxisOnce(); }
    }

    public bool JumpHeld
    {
        get { return m_jumpControl.GetAxisBool(); }
    }

    public bool Pause
    {
        get { return m_menuControl.GetAxisOnce(); }
    }
    #endregion
}
