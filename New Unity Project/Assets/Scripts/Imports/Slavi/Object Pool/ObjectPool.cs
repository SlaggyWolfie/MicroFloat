using UnityEngine;
using System.Collections.Generic;

namespace Slavi
{
    /// <summary>
    /// A very simple object pooling class.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        // collection of currently inactive instances of the prefab
        private Stack<GameObject> _inactiveInstances = new Stack<GameObject>();

        /// <summary>
        /// The prefab that this object pool returns instances of.
        /// </summary>
        [SerializeField] private GameObject _prefab = null;
        [SerializeField] private Transform _parent = null;

        /// <summary>
        /// Returns an instance of the prefab.
        /// </summary>
        /// <returns>Instance of the GameObject prefab.</returns>
        public GameObject GetObject()
        {
            GameObject spawnedGameObject;

            if (_inactiveInstances.Count > 0)
            {
                spawnedGameObject = _inactiveInstances.Pop();
            }
            else
            {
                spawnedGameObject = Instantiate(_prefab);

                // add the PooledObject component to the prefab so we know it came from this pool
                PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>();
                pooledObject.pool = this;
            }

            // put the instance in the root of the scene and enable it
            spawnedGameObject.transform.SetParent(_parent);
            spawnedGameObject.SetActive(true);

            // return a reference to the instance
            return spawnedGameObject;
        }

        /// <summary>
        /// Return an instance of the prefab to the pool
        /// </summary>
        /// <param name="toReturn">The object being returned.</param>
        /// <param name="worldPositionStays">If the world position stays.</param>
        public void ReturnObject(GameObject toReturn, bool worldPositionStays = false)
        {
            PooledObject pooledObject = toReturn.GetComponent<PooledObject>();

            // if the instance came from this pool, return it to the pool
            if (pooledObject != null && pooledObject.pool == this)
            {
                // make the instance a child of this and disable it
                toReturn.transform.SetParent(transform, worldPositionStays);
                toReturn.SetActive(false);

                // add the instance to the collection of inactive instances
                _inactiveInstances.Push(toReturn);
            }
            // otherwise, just destroy it
            else
            {
                Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying.");
                Destroy(toReturn);
            }
        }
    }
}