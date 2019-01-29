using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ISteerable
{
    [Header("Movement Parameters")]

    [Tooltip("Movement speed of the player grounded")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _maxVelocity = 1;
    private float _startingMaxVelocity;

    [Tooltip("The speed that the player rotates towards the direction of movement")]
    [SerializeField] private float _rotationSpeed = 25f;

    [Header("Dashing Parameters")]
    [Tooltip("The speed of the dash")]
    [SerializeField] private float _dashForce = 25;

    [Tooltip("Dash cooldown")]
    [SerializeField] private float _dashCooldown = 2;
    private bool _canDash = true;

    private Vector3 _moveInputDirection = Vector3.zero;
    private Vector3 _rotInputDirection = Vector3.zero;

    private Vector3 _velocity;
    private Vector3 _updateVector = Vector3.zero;
    private Vector3 _moveDir;
    private Vector3 _cachedMoveDirForAudio = Vector3.zero;
    public Vector3 CachedMoveDirForAudio { get { return _cachedMoveDirForAudio; } }

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _startingMaxVelocity = _maxVelocity;
    }

    public void Move()
    {
        _moveDir = (_moveInputDirection) * _moveSpeed;
        CalculateMove();
    }

    public void CalculateMove()
    {
        // Clamp to the maximum speed;
        _moveDir = Vector3.ClampMagnitude(_moveDir, _moveSpeed) * Time.deltaTime;
        _cachedMoveDirForAudio = _moveDir / (_maxVelocity / 1.5f);
        _velocity = _moveDir / Time.deltaTime;

        if (_velocity.magnitude < _maxVelocity) rb.AddForce(_velocity.x, 0, _velocity.z);
        //rb.velocity = Vector3.ClampMagnitude(rb.velocity, _maxVelocity);
        
        RotateCharacter();

        _moveDir = Vector3.zero;
    }

    bool run = false;

    /// <summary>
    /// Used to pass the controller input into the movement function returns false if there was no input
    /// </summary>
    /// <param name="inputHorizontal"></param>
    /// <param name="inputVertical"></param>
    /// <returns></returns>
    public bool RegisterInput(Controls controls)
    {
        // Movement
        _moveInputDirection.x = controls.HorizontalInput;
        _moveInputDirection.z = controls.VerticalInput;
        _moveInputDirection = Vector3.ClampMagnitude(_moveInputDirection, 1f);

        // Rotation
        _rotInputDirection.x = controls.CameraHorizontalInput;
        _rotInputDirection.z = controls.CameraVerticalInput;
        _rotInputDirection = Vector3.ClampMagnitude(_rotInputDirection, 1f);

        if (_moveInputDirection.magnitude == 0) { return false; }
        return true;
    }

    Vector3 currentDirection;
    Vector3 lookDirection;

    //private void RotateCharacter()
    //{

    //    if (Mathf.Abs(_rotInputDirection.x) > 0.01f || Mathf.Abs(_rotInputDirection.z) > 0.01f)
    //    {
    //        Vector3 currentDirection = transform.forward;
    //        Vector3 lookDirection = _rotInputDirection;

    //        //_updateVector = Vector3.Lerp(currentDirection, lookDirection, Time.deltaTime * 15f);
    //        _updateVector = Vector3.RotateTowards(currentDirection, lookDirection, _rotationSpeed * Time.deltaTime, 0.0f);
    //    }

    //    Quaternion lookRotation =
    //        (_updateVector.magnitude > float.Epsilon) ?
    //        Quaternion.LookRotation(_updateVector) : Quaternion.identity;
    //    transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.fixedDeltaTime * _rotationSpeed);

    //    _rotInputDirection = Vector3.zero;
    //}

    private Quaternion targetRotation;
    private void RotateCharacter()
    {
        if (_moveInputDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(_moveInputDirection, Vector3.up);
            rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    public void Dash()
    {
        if (_canDash)
        {
            _maxVelocity = _maxVelocity * 1.5f;
            rb.velocity = transform.forward * _dashForce;
            //rb.AddForce(transform.forward * _dashForce, ForceMode.Impulse);
            _canDash = false;
            StartCoroutine(delay(() => _canDash = true, _dashCooldown));
            StartCoroutine(delay(() => _maxVelocity = _startingMaxVelocity, 1f));
        }
    }

    private IEnumerator delay(System.Action action, float time)
    {
        yield return new WaitForSeconds(time);
        if (action != null) action.Invoke();
    }

    #region ISteerable Implementation
    public Vector3 Velocity
    {
        get { return _velocity; }
        set { _velocity = _velocity; }
    }

    public Vector3 Position
    {
        get { return transform.position; }
    }

    public float Mass
    {
        get { return rb.mass; }
    }


    #endregion
}
