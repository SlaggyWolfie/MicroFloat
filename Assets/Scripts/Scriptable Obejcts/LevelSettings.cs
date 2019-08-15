using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelSettings : ScriptableObject
{
    [SerializeField] private int _pickupsAmount = 10;
    [SerializeField] private int _maxPickups = 3;
    [SerializeField] private int _maxShipPickups = 1;
    [SerializeField] private float _lightTimer = 0;

    public int PickupsAmount { get { return _pickupsAmount; } }
    public int MaxPickups { get { return _maxPickups; } }
    public int MaxShipPickups { get { return _maxShipPickups; } }
    public float LightTimer { get { return _lightTimer; } set { _lightTimer = value; } }

    public override string ToString()
    {
        return string.Format("Level: {0} total pickups, {1} pickups to win level, {2} max pickups the ship can carry at once" +
            ", {3} light/energy timer?", _pickupsAmount, _maxPickups, _maxShipPickups, _lightTimer);
    }
}
