using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Slavi;
using UnityEngine;

public class CancerNode : MonoBehaviour
{
    [SerializeField] private GameObject _cancerPrefab = null;
    [SerializeField] private float _radius = 3;
    [SerializeField] private int _generations = 3;

    [ShowNonSerializedField]
    private bool _grown = false;
    private CancerNode _parent = null;
    private List<CancerNode> _children = new List<CancerNode>();

    public IEnumerable<CancerNode> GetCancerChildren() { return _children; }

    private CancerNode GetRootCancer()
    {
        CancerNode result = this;
        do result = result._parent;
        while (result != null && result._parent != null);
        return result ?? this;
    }

    //public bool Grown { get; set; }
    //public CancerNode Parent { get; set; }

    public void Grow()
    {
        if (_grown) return;

        int leaves = 0;
        if (Random.value < 0.5f) leaves++;
        if (Random.value < 0.5f) leaves++;

        while (leaves > 0)
        {
            if (GetRootCancer()._generations <= 0) break;
            _grown = true;

            GameObject cancer = Instantiate(_cancerPrefab, transform);
            CancerNode child = cancer.GetComponent<CancerNode>();
            child._parent = this;
            _children.Add(child);

            Vector3 possiblePosition = Vector3.zero;
            if (_parent == null)
            {
                possiblePosition = transform.position + Random.insideUnitSphere * 0.5f * _radius + _radius * Vector3.one;
            }
            else
            {
                Vector3 direction = transform.position - _parent.transform.position;
                direction = Quaternion.Euler(Random.Range(1, 136), Random.Range(1, 136), Random.Range(1, 136)) * direction;
                possiblePosition = transform.position + direction * _radius + Random.insideUnitSphere * 0.5f * _radius;
            }

            cancer.transform.position = possiblePosition;
            cancer.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360);

            leaves--;
            GetRootCancer()._generations--;
        }

    }

    private void GrowEventHandler(ObstacleGrowEvent e)
    {
        CancerNode theCancer = GetRootCancer();
        //theCancer._generations--;
        if (!_grown && theCancer._generations > 0) Grow();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void Start()
    {
        Slavi.EventQueue.Instance.AddListener<ObstacleGrowEvent>(GrowEventHandler);
    }
}
