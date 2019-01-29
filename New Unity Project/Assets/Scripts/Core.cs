using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour, ICollidable
{
    [NaughtyAttributes.ShowNonSerializedField] private int _maxPickups = 3;
    [SerializeField] private int _energyPerOrb = 1500;
    [SerializeField] private SphereCollider _suckBox;

    public System.Action<Pickup> OnPickup;

    private Transform _playerTransform = null;
    [SerializeField] public float _maxDistanceToShip = 15;

    [SerializeField]
    private int _targetEnergy;
    public int energy;
    public int erergyDecreaser;
    public Light coreLight;
    public GameObject sphere;
    public FMODUnity.StudioEventEmitter dropoffEmitter = null;
    public float NormalizedDistanceToShip
    {
        get
        {
            if (_playerTransform == null) return 0;
            return (transform.position - _playerTransform.transform.position).magnitude / _maxDistanceToShip;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _maxDistanceToShip / 3);
        Gizmos.DrawWireSphere(transform.position, _maxDistanceToShip / 3 * 2);
        Gizmos.DrawWireSphere(transform.position, _maxDistanceToShip);
    }


    private void Start()
    {
        _playerTransform = FindObjectOfType<Player>().transform;

        OnPickup += onPickup;

        //Some shit.
        Slavi.EventQueue.Instance.AddListener<Slavi.LevelInitEvent>(e =>
        {
            //Add energy stuff here?
            this._maxPickups = e.level.MaxPickups;
        });

        _targetEnergy = energy;
        _maxDistanceToShip = sphere.GetComponent<SphereCollider>().radius * 2;
    }

    private void onPickup(Pickup pickup)
    {
        GameplayManager.Instance.Pickup();
        AddEnergy(_energyPerOrb);

        if (dropoffEmitter != null)
            dropoffEmitter.Play();
    }

    public SphereCollider SuckBox
    {
        get { return _suckBox; }
    }
    private void Update()
    {
        sphere.transform.localScale = Vector3.Lerp(sphere.transform.localScale, new Vector3(1, 1, 1) * energy / 250, Time.deltaTime * 10);
        if (coreLight != null)
            coreLight.colorTemperature = energy;
        coreLight.intensity = 100;

        _targetEnergy = Mathf.Clamp(_targetEnergy -= 1, 1000, 20000);
        energy = (int)Mathf.Lerp(energy, _targetEnergy, Time.deltaTime);

    }

    public void AddEnergy(int value)
    {
        _targetEnergy = Mathf.Clamp((energy + value), 0, 20000);

        // Check and Invoke if above low health. Feedback event
        //if (_health / _maxHealth > _lowHealthPercentage && _isHealthLow)
        //{
        //    _isHealthLow = false;
        //    if (OnHealthStabilize != null)
        //        OnHealthStabilize.Invoke(this);
        //}

        //if (OnHealthChange != null)
        //    OnHealthChange.Invoke(_health);
    }
}
