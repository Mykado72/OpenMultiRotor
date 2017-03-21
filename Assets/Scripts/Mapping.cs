using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu()]
public class Mapping : ScriptableObject
{
    public string pitchAxisName;
    public string rollAxisName;
    public string yawAxisName;
    public string throttleAxisName;
    public bool pitchAxisInversor;
    public bool rollAxisInversor;
    public bool yawAxisInversor;
    public bool throttleAxisInversor;
}