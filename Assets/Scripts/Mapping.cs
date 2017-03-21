using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu()]
public class Mapping : ScriptableObject
{
    public int pitchAxisNb;
    public int rollAxisNb;
    public int yawAxisNb;
    public int throttleAxisNb;
    public bool pitchAxisInversor;
    public bool rollAxisInversor;
    public bool yawAxisInversor;
    public bool throttleAxisInversor;
}