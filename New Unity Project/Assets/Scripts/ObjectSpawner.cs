using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{


    [Tooltip("pickup")]
    [SerializeField] private GameObject[] _pickUps;
    [Tooltip("obstacles")]
    [SerializeField] private GameObject[] _obstacles;
    [Tooltip("The parent for the spawned pickups")]
    [SerializeField] private GameObject _parent;

    //Pickup
    [SerializeField]
    private float _PickupSpawnRange;
    [SerializeField]
    private float _PickupSeparation;
    [SerializeField]
    private int _PickupSpawnAttemts;
    [SerializeField]
    private int _PickupNumber;
    [SerializeField]

    //Obstacle
    private float _ObstacleRange;
    [SerializeField]
    private float _ObstacleSeparation;
    [SerializeField]
    private int _ObstacleSpawnAttemts;
    [SerializeField]
    private int _ObstacleNumber;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_PickupSpawnRange, 0.1f, _PickupSpawnRange));
    }

    private void Awake()
    {
        SpawnObjects(_obstacles, _ObstacleNumber, _ObstacleSeparation, _ObstacleRange, _ObstacleSpawnAttemts);
        SpawnObjects(_pickUps, _PickupNumber, _PickupSeparation, _PickupSpawnRange, _PickupSpawnAttemts);
    }

    [NaughtyAttributes.Button]
    private void SpawnPickups()
    {
        SpawnObjects(_pickUps, _PickupNumber, _PickupSeparation, _PickupSpawnRange, _PickupSpawnAttemts);
    }

    private void SpawnObjects(GameObject[] objects, int numOfObjects, float separation, float radius, int spawnTryNum)
    {
        if (objects.Length == 0) return;
        for (int i = 0; i < numOfObjects; i++)
        {
            GameObject obj = objects[Random.Range(0, objects.Length - 1)];

            bool validPos = false;
            int spawnAttempt = 0;
            Vector3 pos = Vector3.zero;
            while (!validPos && spawnAttempt < spawnTryNum)
            {
                Vector2 twoDimPos = Random.insideUnitCircle * radius;
                pos = new Vector3(twoDimPos.x, gameObject.transform.position.y, twoDimPos.y);
                validPos = true;
                Collider[] colliders = Physics.OverlapSphere(pos, separation);
                foreach (Collider c in colliders)
                {
                    if (c.gameObject.GetComponent<ICollidable>() != null)
                    {
                        validPos = false;
                    }
                }
                spawnAttempt++;
            }
            if (validPos)
            {
                if (_parent != null) Instantiate(obj, pos, Quaternion.identity, _parent.transform);

                else Instantiate(obj, pos, Quaternion.identity);
            }

        }
    }
    private void Start()
    {

    }
}
