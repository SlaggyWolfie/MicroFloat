using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{

    private GameObject _gameObject;
    //private System.Action<Player> OnPlayerDeath;

    public GameObject GameObject
    {
        get { return _gameObject; }
    }

    // Singleton Reference to this class
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
                _instance._gameObject = new GameObject("_gameManager");
                Object.DontDestroyOnLoad(_instance._gameObject);
            }
            //if (_instance._pauseManager == null)
            //    _instance._pauseManager = _instance._gameObject.AddComponent<Pause>();
            if (_instance._cm == null && _instance._gameObject != null)
                _instance._cm = _instance._gameObject.AddComponent<ControllerManager>();
            return _instance;
        }
    }

    private ControllerManager _cm = null;
    /// <summary>
    /// Controller Manager.
    /// </summary>
    public ControllerManager ControllerManager
    {
        get
        {
            return _cm;
        }
        //set { _cm = value; }
    }

    #region Camera (NotImplemented plz Implement)
    //private ThirdCamera _cam;
    ///// <summary>
    ///// Returns the camera component
    ///// </summary>
    //public ThirdCamera Camera
    //{
    //    get { return _cam; }
    //    set { _cam = value; }
    //}

    //private CameraRadialController _camera = null;
    ///// <summary>
    ///// The camera component;
    ///// </summary>
    //public CameraRadialController CameraComponent
    //{
    //    get { return _camera; }
    //    set { _camera = value; }
    //}
    #endregion

    #region Player (notimplemented)
    private Player _localPlayer;
    /// <summary>
    /// Reference to the main player.
    /// </summary>
    public Player LocalPlayer
    {
        get { return _localPlayer; }
        set
        {
            _localPlayer = value;
            //LocalPlayer.OnRespawn.AddListener(PlayerDeathHandler);
        }
    }
    #endregion

    #region Pause (Not Implemented)
    //private GameObject _pauseMenu = null;
    //private GameObject _pauseMenuReference = null;
    //public bool Paused
    //{
    //    get { return PauseManager.paused; }
    //    set
    //    {
    //        PauseManager.SetPause(value);

    //        if (value)
    //        {
    //            var canvas = Object.FindObjectOfType<Canvas>();
    //            if (canvas == null)
    //            {
    //                Object.Instantiate(Resources.Load<GameObject>("EventSystem"));
    //                canvas = Object.Instantiate(Resources.Load<GameObject>("Canvas")).GetComponent<Canvas>();
    //            }

    //            if (_pauseMenu == null) _pauseMenu = Resources.Load<GameObject>("Pause Menu");
    //            _pauseMenuReference = Object.Instantiate(_pauseMenu);
    //            _pauseMenuReference.transform.SetParent(canvas.transform);
    //            _pauseMenuReference.transform.rotation = canvas.transform.rotation;
    //            _pauseMenuReference.transform.localScale = canvas.transform.localScale;

    //            //var eventSystem = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
    //            //canvas.event
    //            //eventSystem.firstSelectedGameObject = _pauseMenuReference.GetComponentInChildren<ResumeButton>().gameObject;
    //            _pauseMenuReference.GetComponentInChildren<ResumeButton>().GetComponent<UnityEngine.UI.Button>().Select();
    //            RectTransform rt = _pauseMenuReference.GetComponent<RectTransform>();
    //            rt.anchoredPosition = new Vector2(0.5f, 0.5f);
    //        }
    //        else
    //        {
    //            if (_pauseMenuReference != null)
    //            {
    //                Object.Destroy(_pauseMenuReference);
    //                _pauseMenuReference = null;
    //            }
    //        }
    //    }
    //}

    //private Pause _pauseManager = null;
    //public Pause PauseManager
    //{
    //    get
    //    {
    //        return _pauseManager;
    //    }
    //    //set { _pauseManager = value; }
    //}
    #endregion

    #region LevelLoading (notImplemented)
    //private LoadLevel _levelLoader;
    //public LoadLevel LevelLoader
    //{
    //    get { return _levelLoader; }
    //    set { _levelLoader = value; }
    //}
    #endregion

    public static bool isQuitting = false;
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDisable()
    {
        Object.Destroy(_instance.GameObject);
        _instance = null;
    }

    public void Delete()
    {
        Object.Destroy(_instance.GameObject);
        _instance = null;
    }
}
