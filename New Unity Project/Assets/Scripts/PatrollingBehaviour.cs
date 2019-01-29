using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PatrollingBehaviour : MonoBehaviour
{
    private bool _towardEnd = true;
    private bool _initialized = false;
    private Vector3 _startPoint = Vector3.zero;
    private Vector3 _endPoint = Vector3.zero;

    [SerializeField] private Vector2 _offset = Vector2.zero;
    [SerializeField] private bool _beginPatrolAtStart = false;
    [SerializeField] private float _rangeX = 1;
    [SerializeField] private float _rangeY = 1;
    [SerializeField] private float _patrolSpeed = 1;

    private void OnDrawGizmosSelected()
    {
        if (!_initialized)
        {
            Vector3 pos = new Vector3(_offset.x, transform.position.y, _offset.y);
            Vector3 size = new Vector3(_rangeX, 0.1f, _rangeY);
            Gizmos.DrawWireCube(pos + size / 2, size);
        }
        else
        {
            Gizmos.DrawLine(_startPoint, _endPoint);
        }
    }

    private Vector3 GeneratePoint()
    {
        //Generate absolute point
        Vector2 twoDimPos = new Vector2(_rangeX * Random.value, _rangeY * Random.value) + _offset;
        return new Vector3(twoDimPos.x, transform.position.y, twoDimPos.y);
    }

    private void GeneratePatrolPoints()
    {
        _startPoint = GeneratePoint();
        _endPoint = GeneratePoint();
        _initialized = true;
    }

    private Vector3 GetCurrentTarget() { return _towardEnd ? _endPoint : _startPoint; }
    //private Vector3 GetOtherTarget() { return (!_towardEnd) ? _endPoint : _startPoint; }

    private void CheckTargetDistance()
    {
        if ((transform.position - GetCurrentTarget()).sqrMagnitude < Mathf.Epsilon)
            _towardEnd = !_towardEnd;
    }

    // Start is called before the first frame update
    private void Start()
    {
        GeneratePatrolPoints();
        if (_beginPatrolAtStart) transform.position = _startPoint;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, GetCurrentTarget(), _patrolSpeed * Time.deltaTime);
        CheckTargetDistance();
    }
}
