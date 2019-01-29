using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0414
public class SteeringManager : MonoBehaviour
{
    [SerializeField] private float _maxVelocity = 1;
    [SerializeField] private float _maxSteerForce = 0.25f;

    [Header("Flee Properties")]
    [SerializeField] private float _fleeRadius = 15;

    [Header("Arrive Properties")]
    [SerializeField] private float _arriveRadius = 15;

    [Header("Wander Properties")]
    [SerializeField] private float _jitterForce = 15;        // Erraticness 
    [SerializeField] private float _circleDistance = 5;
    [SerializeField] private float _circleRadius = 1;
    private float _wanderAngle = 0;

    [Header("Collision Avoidance")]
    //[SerializeField] private AvoidanceBox _avoidanceBox;
    [SerializeField] private float _maxAvoidForce = 1f;

    [Header("Hide Properties")]
    [SerializeField] private float _hideThreatRadius = 30f;

    [Header("Leader Following Properties")]
    [SerializeField] private float _leaderBehindDist = 5f;
    [SerializeField] private float _leaderSightRadius = 5f; 

    [Header("Flocking Properties")]
    [SerializeField] private float _flockingRadius = 15f;
    private HashSet<ISteerable> _neighbors = new HashSet<ISteerable>();

    [SerializeField] private Mult _mult;

    private Vector3 _desiredVelocity;
    private ISteerable _agent;

    private void Start()
    {
        _agent = GetComponent<ISteerable>();
        //_maxVelocity = UnityEngine.Random.Range(0.2f, 0.6f);
    }

    public Vector3 CalculateSteering()
    {
        // Add steering to velocity
        Vector3 steering = Vector3.ClampMagnitude(_desiredVelocity - _agent.Velocity, _maxSteerForce);
        steering = steering / _agent.Mass;

        // Add acceleration to velocity
        Vector3 velocity = Vector3.ClampMagnitude(_agent.Velocity + steering, _maxVelocity);
        velocity.y = 0;
        _agent.Velocity = velocity;

        //DrawVectors();
        _desiredVelocity = Vector3.zero;

        return velocity;
    }

    private bool AccumulateForce(Vector3 forceToAdd)
    {
        float runningTotal = _desiredVelocity.magnitude;

        float magnitudeRemaining = _maxSteerForce - runningTotal;

        if (magnitudeRemaining <= 0f) return false;

        float magnitudeToAdd = forceToAdd.magnitude;

        if (magnitudeToAdd < magnitudeRemaining) _desiredVelocity += forceToAdd;
        else _desiredVelocity += forceToAdd.normalized * magnitudeRemaining;

        return true;
    }

    public void Seek(Vector3 target, float slowRadius = 0) {
        AccumulateForce(m_Seek(target, slowRadius) * _mult.seek);
        DrawRadius(slowRadius);
        //transform.GetChild(0).position = target;
        print("seeking to" + target);
    }

    public void Flee(Vector3 target, float threatRadius = 15) {
        AccumulateForce(m_Flee(target, threatRadius) * _mult.flee);

        DrawRadius(threatRadius);
    }

    public void Wander() {
        AccumulateForce(m_Wander() * _mult.wander);
    }

    public void Pursue(ISteerable other) {
        AccumulateForce(m_Pursuit(other) * _mult.pursue);
    }

    public void Evade(ISteerable other) {
        AccumulateForce(m_Evade(other) * _mult.evade);
    }

    public void CollisionAvoidance() {
        //AccumulateForce(m_CollisionAvoidance() * _mult.colAvoidance);
    }

    public void WallAvoidance() {
        AccumulateForce(m_wallAvoidance() * _mult.wallAvoidance);
    }

    public void FollowLeader(ISteerable leader, float radius = 10) {
        UpdateFlockNeighbors(Boid.AllBoids, radius);
        AccumulateForce(m_LeaderFollowing(leader));
    }
    /// <summary>
    /// Dummy method for GGJ 2019, it's hardcoded stuff
    /// </summary>
    /// <param name="leader"></param>
    public void GetOutTheWay(ISteerable leader) {
        AccumulateForce(m_GetOutTheWay(leader));
    }

    public void Hide(ISteerable target, List<Collider> obstacles) {
        AccumulateForce(m_Hide(target, obstacles, _hideThreatRadius) * _mult.hide);
    }

    public void Separation(float radius = 10)
    {
        UpdateFlockNeighbors(Boid.AllBoids, radius);
        AccumulateForce(m_Separation(_neighbors) * _mult.separation);
    }

    public void Alignment(float radius = 10)
    {
        UpdateFlockNeighbors(Boid.AllBoids, radius);
        AccumulateForce(m_Alignment(_neighbors) * _mult.alignment);
    }

    public void Cohesion(float radius = 10)
    {
        UpdateFlockNeighbors(Boid.AllBoids, radius);
        AccumulateForce(m_Cohesion(_neighbors) * _mult.cohesion);
    }

    public void Flocking(float radius = 10)
    {
        UpdateFlockNeighbors(Boid.AllBoids, radius);
        AccumulateForce(m_Separation(_neighbors) * _mult.separation);
        AccumulateForce(m_Cohesion(_neighbors) * _mult.cohesion);
        AccumulateForce(m_Alignment(_neighbors) * _mult.alignment);
    }

    private Vector3 m_Seek(Vector3 target, float slowRadius = 0)
    {
        // Get target direction
        Vector3 dir = target - _agent.Position;

        // Gradually stop when near target
        if (dir.sqrMagnitude < Mathf.Pow(slowRadius, 2) && slowRadius > 0)
            return dir.normalized * _maxVelocity * (dir.magnitude / slowRadius);
        else
            return dir.normalized * _maxVelocity;
    }

    /// <summary>
    /// Opposite to Seek
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private Vector3 m_Flee(Vector3 target, float threatRadius = 15)
    {
        Vector3 desiredVelocity;

        // Get target direction
        Vector3 dir = _agent.Position - target;
        desiredVelocity = dir.normalized * _maxVelocity;

        if (dir.sqrMagnitude > Mathf.Pow(threatRadius, 2))
        {
            return Vector3.zero;
        }

        return desiredVelocity;
    }

    private Vector3 m_Wander()
    {
        // Place the center of the circle in front of the entity
        Vector3 circleCenter = _agent.Velocity.normalized * _circleDistance;

        // Rotate the wanderAngle slowly by a random number that goes from (-delta, delta)
        _wanderAngle += (UnityEngine.Random.Range(-_jitterForce, _jitterForce));
        #region otherMethod
        //_wanderAngle += (UnityEngine.Random.Range(0.0f, 1.0f) * 2 * _angleDelta) - _angleDelta; // Same thing as above
        #endregion

        // Stablish the displacement the _wanderAngle direction and force with magnitude = radius
        Vector3 displacement = (Quaternion.AngleAxis(_wanderAngle, Vector3.up) * Vector3.one) * _circleRadius;
        displacement.y = 0;

        Vector3 wanderForce = circleCenter + displacement;

        // Debug
        //DrawArrow.DrawDebugLine(transform.position, transform.position + wanderForce, Color.green);
        //transform.GetChild(0).position = transform.position + (circleCenter * 0.85f);
        //transform.GetChild(0).localScale = new Vector3(_circleRadius * 1.5f, 1, _circleRadius * 1.5f);

        return wanderForce;
    }

    /// <summary>
    /// A Seek looking into the future
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private Vector3 m_Pursuit(ISteerable other)
    {
        // Get the distance between units
        Vector3 dist = other.Position - transform.position;

        // Can be fixed number to manually control accuracy. But dynamic is better.
        float estimate = dist.magnitude / _maxVelocity;

        // Look ahead of the other unit
        Vector3 futurePos = other.Position + other.Velocity * estimate;
        return m_Seek(futurePos);
    }

    /// <summary>
    /// Same as pursuit, but Flee instead of Seek
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private Vector3 m_Evade(ISteerable other)
    {
        // Get the distance between units
        Vector3 dist = other.Position - transform.position;

        // Can be fixed number to manually control accuracy. But dynamic is better.
        float estimate = dist.magnitude / _maxVelocity;

        // Look ahead of the other unit
        Vector3 futurePos = other.Position + other.Velocity * estimate;
        return m_Flee(futurePos, 15);
    }

    #region Collision Avoidance 
    /*
    private Vector3 m_CollisionAvoidance()
    {
        #region generic more complex way
        //// Find all obstacles ¯\_(ツ)_/¯
        //Preyoid[] obstacles = FindObjectsOfType<Preyoid>();
        //if (obstacles.Length <= 0) return Vector3.zero;

        //// Go through all found obstacles to find the closest
        //Transform closestObstacle = obstacles[0].transform;
        //float closest = Mathf.Infinity;
        //for(int i = 0; i < obstacles.Length; i++)
        //{
        //    obstacles[i].GetComponent<Renderer>().material.color = Color.white;
        //    Vector3 dir = obstacles[i].transform.position - _agent.Position;

        //    // Dot product to see if it's in front
        //    if (Vector3.Dot(dir, _agent.Velocity.normalized) > 0)
        //    {
        //        // Distance check to discard the farther ones
        //        if(dir.sqrMagnitude < closest)
        //        {
        //            closestObstacle = obstacles[i].transform;
        //            closest = dir.sqrMagnitude;
        //        }
        //    }
        //}
        #endregion

        // WTF ARE THESE MAGIC NUMBERS JESUS - Move the box
        _avoidanceBox.transform.localPosition = new Vector3(0, 0, _agent.Velocity.magnitude * 6.5f);
        _avoidanceBox.Self.size = new Vector3(_avoidanceBox.Self.size.x, 1, _agent.Velocity.magnitude * 15);

        // Get all the colliders intersecting the box
        List<Collider> obstacles = _avoidanceBox.GetColliders();
        if (obstacles.Count <= 0) return Vector3.zero;

        // Get closest obstacle
        Transform closestObstacle = obstacles[0].transform;
        float closest = Mathf.Infinity;
        for (int i = obstacles.Count -1; i >= 0; i--)
        {
            Vector3 dir = obstacles[i].transform.position - _agent.Position;
            // Distance check to discard the farther ones
            if (dir.sqrMagnitude < closest)
            {
                closestObstacle = obstacles[i].transform;
                closest = dir.sqrMagnitude;
            }
        }
        closestObstacle.GetComponent<Renderer>().material.color = Color.red;

        // Calculate avoidance force from the closest point to obstacle
        Vector3 ahead = _agent.Position + _agent.Velocity;
        Vector3 avoidance = Vector3.Min(ahead - closestObstacle.position, 
                                        _agent.Position - closestObstacle.position
                                        ).normalized * _maxAvoidForce;
        return avoidance;
    }
    */
    #endregion

    private Vector3 m_wallAvoidance()
    {
        Vector3 steering = Vector3.zero;

        float farSight = 30;

        // Create feelers
        Vector3 xFeel = Quaternion.AngleAxis(45, Vector3.up) * _agent.Velocity * (farSight * 0.5f);
        Vector3 yFeel = Quaternion.AngleAxis(-45, Vector3.up) * _agent.Velocity * (farSight * 0.5f);
        Vector3 vFeel = _agent.Velocity * farSight;
        Vector3[] feelers = { xFeel, yFeel, vFeel };

        for(int i = 0; i < feelers.Length; i++)
        {
            //Debug.DrawRay(transform.position, feelers[i], Color.black);
            RaycastHit hit;
            // Detects if there's a wall intersecting
            if(Physics.Raycast(_agent.Position, feelers[i], out hit, feelers[i].magnitude) /*&& hit.transform.GetComponent<Wall>()*/)
            {
                // Calculate by what distance the vector overshoots the wall
                Vector3 overshoot = feelers[i] - hit.point;

                // Create a force in the direction of the wall normal
                steering += hit.normal * overshoot.magnitude;
            }
        }

        return steering;
    }

    Vector3 _behindLeaderPos;
    Vector3 _aheadLeaderPos;
    private Vector3 m_LeaderFollowing(ISteerable leader)
    {
        Vector3 tv = leader.Velocity;
        Vector3 force = new Vector3();

        if (leader.Velocity.magnitude != 0)
        {
            // Calculate the ahead point
            tv = tv.normalized * _leaderBehindDist;
            _aheadLeaderPos = leader.Position + tv;

            // Calculate the behind point
            _behindLeaderPos = leader.Position + (tv * -1);
        }

        // If the character is on the leader's sight,
        // add a force to evade the route immediately
        if (IsOnLeaderSight(leader, _aheadLeaderPos)) force += m_Evade(leader) * _mult.evade;

        // Creates a force to arrive at the behind point
        force += m_Seek(_behindLeaderPos, 15) * _mult.seek;

        // Add separation Force
        force += m_Separation(_neighbors) * _mult.separation;

        return force;
    }

    /// <summary>
    /// Specific method for Game Jam - It's hardcoded stuff
    /// </summary>
    /// <param name="leader"></param>
    /// <returns></returns>
    private Vector3 m_GetOutTheWay(ISteerable leader)
    {
        Vector3 tv = leader.Velocity;
        Vector3 force = new Vector3();

        if (leader.Velocity.magnitude != 0)
        {
            // Calculate the ahead point
            tv = tv.normalized * _leaderBehindDist * 2;
            _aheadLeaderPos = leader.Position + tv;
        }

        // If the character is on the leader's sight,
        // add a force to evade the route immediately
        if (IsOnLeaderSight(leader, _aheadLeaderPos)) force += m_Evade(leader) * _mult.evade;

        return force;
    }

    public float LeaderSightRadius {
        get { return _leaderSightRadius; }
        set { _leaderSightRadius = value; }
    }
    private bool IsOnLeaderSight(ISteerable leader, Vector3 leaderAheadPos)
    {
        return Vector3.Distance(leaderAheadPos, _agent.Position) <= LeaderSightRadius || Vector3.Distance(leader.Position, _agent.Position) <= LeaderSightRadius;
    }

    private Vector3 m_Hide(ISteerable target, List<Collider> obstacles, float threatRadius = 30)
    {
        // Prevent further calculations if there's not threat
        bool inRadius = (target.Position - _agent.Position).sqrMagnitude <= Mathf.Pow(threatRadius, 2);
        if (!inRadius) return Vector3.zero;

        float closestDist = Mathf.Infinity;
        Vector3 bestHidingSpot = Vector3.zero;

        // Iterate through all obstacles
        for(int i = obstacles.Count -1; i >= 0; i--)
        {
            // Get the hiding position - use extents instead of radius cause box.
            Vector3 hidingSpot = GetHidingPos(obstacles[i].transform.position, obstacles[i].bounds.extents.x, target.Position);

            // Evaluate if closest
            float dist = (hidingSpot - _agent.Position).sqrMagnitude;
            if(dist < closestDist)
            {
                bestHidingSpot = hidingSpot;
                closestDist = dist;
            }
        }

        // No hiding spot :(
        if (closestDist == Mathf.Infinity) return m_Evade(target);

        // Else, hide
        return m_Seek(bestHidingSpot, 3);

        /// TODO
        /// Only hide if there is a *mutual* line of sight between agent and target
        /// Try to hide on the sides or rear of target by biasing distances returned from GetHidingPos with Dot product (check behind)
    }

    private Vector3 GetHidingPos(Vector3 obstaclePos, float obstacleRadius, Vector3 targetPos)
    {
        // How far the agent wants to be from the obstacle
        float distFromBoundary = 5f;
        float separationDist = obstacleRadius + distFromBoundary;

        Vector3 dir = (obstaclePos - targetPos).normalized;

        return (dir * separationDist) + obstaclePos;
    }

    private void UpdateFlockNeighbors(HashSet<ISteerable> boids, float radius)
    {
        _neighbors.Clear();

        //for (int i = boids.Count -1; i > 0; i--) 
        foreach (ISteerable boid in boids)
        {
            //boids.Remove
            //Boid boid = boids.GetEnumerator().;
            // If it's us, skip
            if (boid == _agent) continue;

            Vector3 distanceToAgent = _agent.Position - boid.Position;

            // Radius of the other boid
            float boundingRadius = 0.5f;
            // If it's outside our radius add the the list of neighbors
            if (distanceToAgent.sqrMagnitude <= Mathf.Pow(radius + boundingRadius, 2))
                _neighbors.Add(boid);
        }
    }

    private Vector3 m_Separation(HashSet<ISteerable> neighbors)
    {
        Vector3 steeringForce = Vector3.zero;

        foreach (ISteerable neighbor in neighbors)
        {
            Vector3 distanceToAgent = _agent.Position - neighbor.Position; 

            // Scale force inversely proportional to the distance from neighbor to agent
            steeringForce += distanceToAgent.normalized / distanceToAgent.magnitude;
        }

        return steeringForce;
    }

    private Vector3 m_Alignment(HashSet<ISteerable> neighbors)
    {
        Vector3 averageHeading = Vector3.zero;

        // Sum all forward vectors;
        foreach(ISteerable neighbor in neighbors)
        {
            averageHeading += neighbor.Velocity.normalized;
        }

        if(neighbors.Count > 0)
        {
            // Average all headingVectors
            averageHeading /= (float)neighbors.Count;
            averageHeading -= _agent.Velocity.normalized;
        }

        return averageHeading;
    }

    private Vector3 m_Cohesion(HashSet<ISteerable> neighbors)
    {
        Vector3 centerOfMass = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;

        // Sum all of the positions
        foreach (ISteerable neighbor in neighbors)
        {
            centerOfMass += neighbor.Position;
        }

        if(neighbors.Count > 0)
        {
            // The center of mass is the average of all positions
            centerOfMass /= (float)neighbors.Count;

            // Seek to that position
            steeringForce = m_Seek(centerOfMass);
        }

        return steeringForce;
    }

    #region Debug 
    private void DrawRadius(float radius)
    {
        //transform.GetChild(0).localScale = new Vector3(radius, 1, radius);
    }

    private void DrawVectors()
    {
        //float scale = _maxVelocity * 20;

        //DrawArrow.DrawDebugLine(transform.position, transform.position + _agent.Velocity * scale, Color.blue);
        //DrawArrow.DrawDebugLine(transform.position + _agent.Velocity * scale, transform.position + _desiredVelocity * scale, Color.magenta);
        //DrawArrow.DrawDebugLine(transform.position, transform.position + _desiredVelocity * scale, Color.red);

        //DrawArrow.DrawDebugLine(transform.position + _desiredVelocity * scale, MousePos, Color.yellow);
    }

    #endregion

    [Serializable]
    struct Mult
    {
        [Header("Unit steering")]
        public float seek;
        public float flee;
        public float wander;
        public float pursue;
        public float evade;
        public float leaderFollowing;
        public float hide;
        public float colAvoidance;
        public float wallAvoidance;

        [Header("Group steering")]
        public float separation;
        public float alignment;
        public float cohesion;
    }
}

#pragma warning restore 0414