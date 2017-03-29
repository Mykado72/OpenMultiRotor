using System;
using UnityEngine;

[Serializable]

public class Mapping
{
    public int pitchAxisNb;
    public int rollAxisNb;
    public int yawAxisNb;
    public int throttleAxisNb;
    public bool pitchAxisInversor;
    public bool rollAxisInversor;
    public bool yawAxisInversor;
    public bool throttleAxisInversor;

    public Mapping()
    {
        pitchAxisNb=0;
        rollAxisNb=0;
        yawAxisNb=0;
        throttleAxisNb=0;
        pitchAxisInversor=false;
        rollAxisInversor = false;
        yawAxisInversor = false;
        throttleAxisInversor = false;
    }
}