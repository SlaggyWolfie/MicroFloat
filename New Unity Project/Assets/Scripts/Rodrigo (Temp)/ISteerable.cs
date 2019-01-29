using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A steerable unit must have a position, velocity and mass
/// </summary
public interface ISteerable
{
    Vector3 Position { get; }
    Vector3 Velocity { get; set; }
    float Mass { get; }
}
