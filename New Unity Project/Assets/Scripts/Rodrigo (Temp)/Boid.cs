using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour, ISteerable
{
    [SerializeField] private float _mass = 4;
    private Vector3 _velocity;
    protected SteeringManager _steering;

    public static HashSet<ISteerable> AllBoids = new HashSet<ISteerable>();

    // Use this for initialization
    void Awake()
    {
        _steering = GetComponent<SteeringManager>();
        AllBoids.Add(this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.position += _steering.CalculateSteering();
        LookWhereYoureGoing();
    }

    protected virtual void LookWhereYoureGoing()
    {
        if (_velocity.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(_velocity);
    }

    public Vector3 Velocity
    {
        get { return _velocity; }
        set { _velocity = value; }
    }

    public Vector3 Position
    {
        get { return transform.position; }
    }

    public float Mass
    {
        get { return _mass; }
    }
}
