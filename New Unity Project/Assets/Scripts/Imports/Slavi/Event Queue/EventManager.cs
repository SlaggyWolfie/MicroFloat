using UnityEngine;

namespace Slavi
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this); 
                //Destroy(gameObject); 
            }
        }

        // Update is called once per frame
        private void Update() { EventQueue.Instance.DeliverAll(); }
    }
}
