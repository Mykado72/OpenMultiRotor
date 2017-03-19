using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AxisMap : MonoBehaviour {

    public Slider [] sliders;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        for (int axisNb = 0; axisNb < sliders.Length; axisNb++)
        {
            sliders[axisNb].value = Input.GetAxis("axis"+axisNb.ToString());
        }
	}

    public void MoveSlider(int sliderNb, int axisNb)
    {

    }
}
