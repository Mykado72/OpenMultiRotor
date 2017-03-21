using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class AxisMap : MonoBehaviour {

    public Slider [] sliders;
    public Dropdown[] axisNames;
    public Toggle [] inversors;
    public Mapping mapping;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        for (int axisNb = 0; axisNb < sliders.Length; axisNb++)
        {
            if (inversors[axisNb].isOn == true)
                sliders[axisNb].value = -Input.GetAxis("axis" + axisNb.ToString());
            else
                sliders[axisNb].value = Input.GetAxis("axis"+axisNb.ToString());
        }
	}

    public void Save()
    {
        mapping =Mapping.CreateInstance<Mapping>();
        
        for (int axisNb = 0; axisNb < sliders.Length; axisNb++)
        {
            if (axisNames[axisNb].itemText.text=="Roll")
            {
                mapping.rollAxisName = "axis" + axisNb.ToString();
                mapping.rollAxisInversor = inversors [axisNb].isOn;
                Debug.Log("Roll Inversor :" + inversors[axisNb].isOn);
            }
        }
        AssetDatabase.CreateAsset(mapping, "Assets/Mapping.asset");
        string json = JsonUtility.ToJson(mapping);
    }
}
