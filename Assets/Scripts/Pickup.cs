using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour, ICollidable, IActivatable
{
    [SerializeField] private float _targetScale = 0.5f;

    private Vector3 _targetScaleVector = Vector3.zero;
    private float _distanceSpeed = 0;
    private float _scaleSpeed = 0;
    private bool _activated = false;
    private Core _core = null;
    private SphereCollider _suckBox;

    private bool _shouldAbsorb = false;
    private bool _collectAfterTimeStarted = false;

    private void Start()
    {
        //_core = GameObject.Find("CORE").GetComponent<Core>();
        _core = FindObjectOfType<Core>();
        _suckBox = _core.SuckBox;
        _activated = false;

        StartCoroutine(spawnDelay());
    }

    private void Update()
    {
        if (_core == null) return;
        float distance = Vector3./*Sqr*/Magnitude(transform.position - _core.transform.position);
        if (distance < _core._maxDistanceToShip)
        {
            _activated = true;
        }
        if (_activated) absorb(_core, distance);
    }

    //private void FixedUpdate()
    private void absorb(Core target, float distance)
    {
        if (_shouldAbsorb == false) return;
        if (_collectAfterTimeStarted == false) StartCoroutine(collectAfterTime(target));

        _distanceSpeed = distance * 5;
        _scaleSpeed = (transform.localScale.magnitude - _targetScale) / (distance * 0.5f);

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position/* + Vector3.up*/, _distanceSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScaleVector, _scaleSpeed * Time.deltaTime);
        //transform.localScale = Vector3.MoveTowards(transform.localScale, _targetScaleVector, _scaleSpeed * Time.deltaTime);

        if ((distance < 0.5f)) collect(target);
    }

    private IEnumerator spawnDelay()
    {
        yield return new WaitForSeconds(1);
        _shouldAbsorb = true;
    }

    private IEnumerator collectAfterTime(Core target)
    {
        _collectAfterTimeStarted = true;
        yield return new WaitForSeconds(2f);
        collect(target);
    }

    private void collect(Core target)
    {
        if (target.OnPickup != null) target.OnPickup.Invoke(this);
        Destroy(gameObject);
    }

    public void Activate()
    {
        _activated = true;
        _targetScaleVector = Vector3.one * _targetScale;
    }
}
