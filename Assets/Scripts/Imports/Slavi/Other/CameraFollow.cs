using UnityEngine;

namespace Slavi
{
    public class CameraFollow : MonoBehaviour
    {
        private Vector3 _movingOffsetLeft = new Vector3();
        private Vector3 _movingOffsetRight = new Vector3();

        private Vector3 _staticOffset = new Vector3();
        private float _centreTimer = 0;

        private Vector3 _oldTargetPosition = new Vector3();
        private Vector3 _oldTargetForward = new Vector3();

        [SerializeField] private Transform _targetToFollow = null;
        [SerializeField] private float _smoothing = 5;

        [SerializeField] private float _timeToCenter = 2;

        [SerializeField, Range(0, 10)] private float _offsetFront = 10;
        [SerializeField, Range(0, 10)] private float _offsetUp = 2;
        [SerializeField, Range(0, 10)] private float _cameraDistance = 10;

        private void OnValidate()
        {
            _movingOffsetLeft = new Vector3(_offsetFront, _offsetUp, -_cameraDistance);
            _movingOffsetRight = new Vector3(-_offsetFront, _offsetUp, -_cameraDistance);
            _staticOffset = new Vector3(0, _offsetUp, -_cameraDistance);
        }

        private void Start()
        {
            Debug.Assert(_targetToFollow != null, "Please assign target to follow to the camera.");
            transform.LookAt(transform.position + Vector3.forward);
            //_offset = transform.position - _targetToFollow.position;
        }

        private void FixedUpdate()
        {
            if (_targetToFollow == null) return;

            Vector3 targetPosition = _targetToFollow.position;
            float deltaTime = Time.fixedDeltaTime;

            if ((targetPosition - _oldTargetPosition).sqrMagnitude < Mathf.Epsilon &&
                Vector3.Dot(_targetToFollow.forward, _oldTargetForward) > 0)
            {
                _centreTimer -= deltaTime;

                if (_centreTimer <= 0)
                {
                    MoveToTarget(targetPosition, _staticOffset, _smoothing * deltaTime);
                    _centreTimer = 0;
                    return;
                }
            }
            else
            {
                _centreTimer = _timeToCenter;
            }

            Vector3 movingOffsetWithDirection = Vector3.Dot(_targetToFollow.forward, Vector3.right) > 0
                ? _movingOffsetLeft
                : _movingOffsetRight;

            MoveToTarget(targetPosition, movingOffsetWithDirection, _smoothing * deltaTime);
            _oldTargetForward = _targetToFollow.forward;
        }

        private void MoveToTarget(Vector3 targetPosition, Vector3 offset, float smoothing)
        {
            Vector3 target = targetPosition + offset;
            _oldTargetPosition = targetPosition;

            transform.position = Vector3.Slerp(transform.position, target, smoothing);
        }
    }
}