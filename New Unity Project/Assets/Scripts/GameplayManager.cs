using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Slavi;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
            //Destroy(gameObject);
        }
    }

    private bool RulesExist() { return _rules != null; }

    [SerializeField] private RulesSettings _rules = null;

    [Space]
    [ShowIf("RulesExist")]
    [SerializeField] private bool _autoComplete = true;
    [ShowIf("RulesExist")]
    [SerializeField] private bool _selfUpdate = false;

    [Space]
    [ShowIf("RulesExist")]
    [ReadOnly, SerializeField] private int _pickupsLeft = -1;
    [ShowIf("RulesExist")]
    [ReadOnly, SerializeField] private float _timer = -1;
    [ShowIf("RulesExist")]
    [ReadOnly, SerializeField] private bool _levelEnded = false;
    [Space]
    [ShowIf("RulesExist")]
    [ReadOnly, SerializeField] private LevelSettings _currentLevel = null;

    private RulesSettings _rulesInstance = null;
    public RulesSettings RulesInstance
    {
        get { return _rulesInstance ?? (_rulesInstance = Instantiate(_rules)); }
    }

    public bool AutoComplete
    {
        get { return _autoComplete; }
        set { _autoComplete = value; }
    }

    public bool SelfUpdate
    {
        get { return _selfUpdate; }
        set { _selfUpdate = value; }
    }

    public LevelSettings CurrentLevel
    {
        get { return _currentLevel; }
    }

    public int PickupsLeft
    {
        get { return _pickupsLeft; }
    }

    public void Init(LevelSettings level)
    {
        if (level == null)
        {
            Debug.LogError("End of levels");
            return;
        }

        _currentLevel = level;

        Debug.Log("Init " + level.ToString());
        _timer = level.LightTimer;
        _pickupsLeft = level.MaxPickups;
        _levelEnded = false;

        EventQueue.Instance.SendAndDeliver(new LevelInitEvent(level));
    }

    public void Pickup(int amount = 1)
    {
        _pickupsLeft -= amount;
        DoAutoComplete();
    }
    public void TimerTick()
    {
        _timer -= Time.deltaTime;
        DoAutoComplete();
    }

    private void DoAutoComplete() { if (AutoComplete) CheckAndComplete(); }

    public void CheckAndComplete()
    {
        Debug.Assert(_currentLevel != null);
        if (_levelEnded) return;

        if (_pickupsLeft == 0)
        {
            RulesInstance.CompleteLevel(_currentLevel);
            EventQueue.Instance.Send(new WinLevelEvent());
            _pickupsLeft = -1;
            _levelEnded = true;
        }

        if (RulesInstance.IncompleteLevelCount <= 0)
        {
            EventQueue.Instance.Send(new WinGameEvent());
            _levelEnded = true;
        }

        if (_timer <= 0)
        {
            EventQueue.Instance.Send(new LoseLevelEvent());
            _levelEnded = true;
        }
    }

    private void Start()
    {
        Init(RulesInstance.GetFirstIncompleteLevel());
    }

    private void Update()
    {
        if (!SelfUpdate) return;
        TimerTick();

        //Already being called otherwise.
        if (!AutoComplete) DoAutoComplete();
    }

    private void OnDisable() { _rulesInstance = null; }
    private void OnDestroy() { _rules = null; }
}
