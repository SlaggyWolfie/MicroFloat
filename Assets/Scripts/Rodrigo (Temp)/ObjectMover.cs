using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour, IActivatable {

    [SerializeField] private bool _activated = false;

    [Tooltip("The object to move")]
    [SerializeField] private Transform _object;
    [Tooltip("Starting Position")]
    [SerializeField] private Transform _startPosition;
    [Tooltip("Position of the destination")]
    [SerializeField] private Transform _endPosition;

    [Tooltip("Mesh that the gizmos will display")]
    [SerializeField] private Transform _gizmoMesh;

    [Tooltip("If the object should move back and forth")]
    [SerializeField] private bool _returns;
    [Tooltip("Speed of the interpolation")]
    [SerializeField] private float _speed = 2.5f;
    [Tooltip("Interval between moving changing destination")]
    [SerializeField] private float _resetTime;

    [Header("Ticking Options")]
    [SerializeField] private bool _ticking = false;
    [SerializeField] private float _tickInterval = 1;
    [SerializeField] private float _moveInterval = 0.5f;

    [SerializeField] private bool _drawGizmos = true;

    private bool _moving = false;
    private Rigidbody _rbPlatform;

    private Vector3 _direction;
    private Transform _destination;

    private void Start()
    {
        SetDestination(_startPosition);

        _rbPlatform = _object.GetComponent<Rigidbody>();
        if(_rbPlatform == null)
        {
            _rbPlatform = _object.gameObject.AddComponent<Rigidbody>();
        }
        _rbPlatform.isKinematic = true;
        _rbPlatform.useGravity = false;
    }

    private void OnDrawGizmos()
    {
        if(_object.GetComponent<MeshFilter>() != null) { 
            Mesh mesh = _object.GetComponent<MeshFilter>().sharedMesh;
            if (mesh != null)
            {
                if (_drawGizmos)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(mesh, _startPosition.position, _object.rotation, _object.localScale);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireMesh(mesh, _endPosition.position, _object.rotation, _object.localScale);
                }
            }
        }
        else
        {
            if(_gizmoMesh != null)
            {
                Mesh mesh = _gizmoMesh.GetComponent<MeshFilter>().sharedMesh;
                if (_drawGizmos)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(mesh, _startPosition.position, _gizmoMesh.rotation, _gizmoMesh.localScale);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireMesh(mesh, _endPosition.position, _gizmoMesh.rotation, _gizmoMesh.localScale);
                }
            }
        }
    }

    private void Update()
    {
        if(_activated)
        { 
            if(Vector3.Distance(_object.position, _destination.position) < 5 * Time.deltaTime)
            {
                _object.position = _destination.position;

                if (_returns) StartCoroutine(returnTimer());
                else
                {
                    _object.position = _destination.position;
                    _activated = false;
                    return;
                }
                
            }

            if(_ticking)
            {
                if (_moving)
                {
                    //_rbPlatform.MovePosition(_platform.position + _direction * _speed * Time.fixedDeltaTime);
                    _object.position = (_object.position + _direction * _speed * Time.deltaTime);
                }
                    //transform.position = Vector3.Lerp(transform.position, _newPosition, _speed * Time.deltaTime);
            }
            else
            {
                _object.position = (_object.position + _direction * _speed * Time.deltaTime);
                //_rbPlatform.MovePosition(_platform.position + _direction * _speed * Time.fixedDeltaTime);
            }
        }
    }

    private IEnumerator returnTimer()
    {
        _activated = false;
        yield return new WaitForSeconds(_resetTime);
        _activated = true;
        SetDestination(_destination == _startPosition ? _endPosition : _startPosition);
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(_tickInterval);
            _moving = true;
            yield return new WaitForSeconds(_moveInterval);
            _moving = false;
        }
    }

    public void SetDestination(Transform dest)
    {
        _destination = dest;
        _direction = (_destination.position - _object.position).normalized;
    }

    private void OnEnable()
    {
        StartCoroutine(Tick());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Activate()
    {
        _activated = true;
    }
}