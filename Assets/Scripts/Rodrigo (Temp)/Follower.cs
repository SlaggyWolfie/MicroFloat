using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Boid
{
    private ISteerable target;

    // Start is called before the first frame update
    void Start()
    {
        _steering = GetComponent<SteeringManager>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (target != null)
        {
            _steering.GetOutTheWay(papi);
            _steering.FollowLeader(target);
        }

        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player p = other.GetComponent<Player>();
        if (p != null)
        {
            p.AddFollower(this);
        }
    }

    public Follower child { get; set; }
    public ISteerable Leader { get { return target; } }

    private ISteerable papi;
    public void setPapi(ISteerable newPapi)
    {
        papi = newPapi;
    }
    public void setLeader(ISteerable newLeader)
    {
        target = newLeader;
    }

    private void OnDestroy()
    {
        AllBoids.Remove(this);

        GameManager.Instance.LocalPlayer.RemoveFollower(this);
    }
}
