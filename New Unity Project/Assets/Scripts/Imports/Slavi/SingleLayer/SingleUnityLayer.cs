using UnityEngine;

namespace Slavi
{
    [System.Serializable]
    public class SingleUnityLayer
    {
        [SerializeField] private int _layerIndex = 0;
        public int LayerIndex { get { return _layerIndex; } }
        public int Mask { get { return 1 << _layerIndex; } }

        public void Set(int layerIndex)
        {
            if (layerIndex > 0 && layerIndex < 32)
                _layerIndex = layerIndex;
        }
    }
}