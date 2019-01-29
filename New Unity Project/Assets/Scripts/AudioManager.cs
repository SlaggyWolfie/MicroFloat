using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMOD.Studio;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    private Core _core = null;
    private PlayerMovement _pm = null;
    private Player _player = null;

    private EventInstance _musicEvent = default(EventInstance);
    private EventInstance _characterEvent = default(EventInstance);

    private bool _init = false;

    [SerializeField, EventRef] private string _musicEventRef = string.Empty;
    [SerializeField, EventRef] private string _characterEventRef = string.Empty;

    // Start is called before the first frame update
    private void Start()
    {
        _pm = FindObjectOfType<PlayerMovement>();
        _player = _pm.GetComponent<Player>();
        _core = FindObjectOfType<Core>();

        _musicEvent = RuntimeManager.CreateInstance(_musicEventRef);
        //_musicEvent = RuntimeManager.CreateInstance("event:/Music/Music");
        //_characterEvent = RuntimeManager.CreateInstance("event:/Character/CharacterMovement");
        _characterEvent = RuntimeManager.CreateInstance(_characterEventRef);

        _musicEvent.start();
        _characterEvent.start();

        //RuntimeManager.AttachInstanceToGameObject(_musicEvent, transform, null);
        //RuntimeManager.AttachInstanceToGameObject(_characterEvent, _player.transform, null);

        _init = true;
    }

    private void OnDisable()
    {
        _musicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _characterEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        _musicEvent.release();
        _characterEvent.release();
    }

    private void Update()
    {
        UpdatePlayer();
        UpdateCore();
    }

    // Update is called once per frame
    private void UpdateCore()
    {
        if (!_init) return;
        _musicEvent.setParameterValue("Distance", _core.NormalizedDistanceToShip * 3);
        float warmth = _core.energy / 20000.0f;
        _musicEvent.setParameterValue("Intensity", (_core.NormalizedDistanceToShip + (1 - warmth)) / 2);
        _musicEvent.setParameterValue("Warmth", warmth);
    }
    private void UpdatePlayer()
    {
        if (!_init) return;
        _musicEvent.setParameterValue("IsDead", _player.IsDead ? 1 : 0);

        //if (_pm == null) return;
        _characterEvent.setParameterValue("Movement", (_pm.CachedMoveDirForAudio.x + _pm.CachedMoveDirForAudio.y) / 2);
        //(new EventInstance).setParameterValue("Distance", _core.Normalized3DistanceToShip);
    }
}
