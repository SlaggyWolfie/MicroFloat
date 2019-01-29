using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : MonoBehaviour, ICollidable
{
    private bool _dead = false;
    private bool _started = false;

    private Controls _controls;
    private PlayerMovement _movement;

    [SerializeField] private int _nOrbs = 3;
    [NaughtyAttributes.ReadOnly, SerializeField]
    private List<Follower> _orbs = new List<Follower>();

    public FMODUnity.StudioEventEmitter evM = null;
    public FMODUnity.StudioEventEmitter explosionEmitter = null;
    public GameObject explosionPrefab = null;
    public bool IsDead { get { return _dead; } }

    // Start is called before the first frame update
    void Start()
    {
        _controls = GetComponent<Controls>();
        _movement = GetComponent<PlayerMovement>();

        Slavi.EventQueue.Instance.AddListener<Slavi.LevelInitEvent>(
            e => _nOrbs = e.level.MaxShipPickups);

        GameManager.Instance.LocalPlayer = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 0") && !_started)
        {
            _started = true;
        }
        if (!_dead && _started) _movement.RegisterInput(_controls);
    }

    private void FixedUpdate()
    {
        if (_dead) return;
        _movement.Move();
        if (_controls.Jump) _movement.Dash();
    }

    public bool RemoveFollower(Follower follower)
    {
        bridgeOrbSlots(follower);
        return _orbs.Remove(follower);
    }

    public bool AddFollower(Follower newFollower)
    {
        if (_orbs.Count < _nOrbs && !_orbs.Contains(newFollower))
        {
            AssignOrbSlot(newFollower, _orbs.Count <= 0 ? GetComponent<ISteerable>() : _orbs[_orbs.Count - 1]); ;
            Slavi.EventQueue.Instance.Send(new Slavi.PlayerSuckEvent());
            if (evM != null) evM.Play();
            return true;
        }
        return false;
    }

    private void AssignOrbSlot(Follower orb, ISteerable parent)
    {
        if (parent is Follower)
        {
            Follower fp = parent as Follower;
            fp.child = orb;
        }

        orb.setPapi(_movement);
        orb.setLeader(parent);
        _orbs.Add(orb);
    }

    private void bridgeOrbSlots(Follower follower)
    {
        int index = _orbs.IndexOf(follower);
        if (index > 0)
        {
            // Reassign child of the one ahead
            _orbs[index - 1].child = index + 1 == _orbs.Count ? null : follower.child;
        }
        if (index < _orbs.Count - 1)
        {
            // Reassing parent of the one behind
            if (follower != null && follower.child != null)
                follower.child.setLeader(index == 0 ? GetComponent<ISteerable>() : follower.Leader);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Obstacle>() ||
            collision.collider.GetComponent<RootObstacle>())
        {
            if (_orbs.Count != 0)
            {
                //Near death experience
                _orbs.ForEach(Destroy);
                _orbs.Clear();

                //Push back body to prevent another hit
                GetComponent<Rigidbody>().AddForce(collision.impulse * 2, ForceMode.Impulse);
                Slavi.EventQueue.Instance.Send(new Slavi.LoseLifeEvent());
            }
            else
            {
                Slavi.EventQueue.Instance.Send(new Slavi.LoseLevelEvent());
                if (explosionPrefab != null)
                {
                    Debug.Log("Test SHIT");
                    var g = Instantiate(explosionPrefab, transform);
                    //g.transform.position = transform.position;
                    g.transform.localPosition = Vector3.zero;
                    g.transform.localRotation = Quaternion.Euler(UnityEngine.Random.insideUnitSphere * 360);
                }
                Debug.Log("Test SHIT2222");
                if (explosionEmitter != null) explosionEmitter.Play();
                _dead = true;
                //death stuff
            }
        }
    }
}
