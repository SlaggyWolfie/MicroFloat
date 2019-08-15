using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootObstacle : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _exclude = new List<Transform>();
    [SerializeField]
    private List<Transform> _branches = new List<Transform>();

    public GameObject[] obstacles;
    void Start()
    {
        _exclude.Add(transform);
        Slavi.EventQueue.Instance.AddListener<Slavi.ObstacleGrowEvent>(ObstacleGrowListener);
        _branches = new List<Transform>(GetComponentsInChildren<Transform>());
    }

    public void GrowObstacle()
    {

        for (int i = _branches.Count - 1; i > 0; i--)
        {
            if (_exclude.Contains(_branches[i]))
            {
                _branches.Remove(_branches[i]);
            }
        }

        int r = Random.Range(0, _branches.Count);
        Transform luckyGuy = _branches[r];

        //_exclude.Add(luckyGuy);
        int n = Random.Range(0, obstacles.Length);
        GameObject g = Instantiate(obstacles[n], luckyGuy);
        for (int i = 0; i < g.GetComponentsInChildren<Transform>().Length; i++)
        {
            _branches.Add(g.GetComponentsInChildren<Transform>()[i]);
        }
        _branches.RemoveAt(r);
        _branches.Remove(g.transform);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    if (Random.Range(0, 1) <= 0.65f)
        //    {
        //        GrowObstacle();
        //    }
        //}
    }
    private void ObstacleGrowListener(Slavi.ObstacleGrowEvent e)
    {
        if (Random.Range(0, 1) <= 0.30f)
        {
            GrowObstacle();
        }
    }
}
