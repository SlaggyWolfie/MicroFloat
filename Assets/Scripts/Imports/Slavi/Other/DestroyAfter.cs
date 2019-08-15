using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slavi
{
    public class DestroyAfter : MonoBehaviour
    {
        public float timer = 0f;

        //Coroutine Approach
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(timer);
            Destroy(gameObject);
        }

        //Void Approach
        //private void Start() { Destroy(gameObject, timer); }
    }
}
