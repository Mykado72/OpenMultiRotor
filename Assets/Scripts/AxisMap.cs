using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class AxisMap : MonoBehaviour {

    enum rcAxis { Pitch, Roll, Yaw, Throttle };
    public Slider [] sliders;
    public Dropdown [] dropdownAxis;
    public string [] axisNames;
    public Toggle [] inversors;
    public Mapping mapping;
    public Button savebutton;
    public string joyname;
    public string[] joynames;
    // Use this for initialization
    void Awake () {
        dropdownAxis[0].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[0].value, 0); });
        dropdownAxis[1].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[1].value, 1); });
        dropdownAxis[2].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[2].value, 2); });
        dropdownAxis[3].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[3].value, 3); });
        dropdownAxis[4].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[4].value, 4); }); 
    }

    void Start()
    {
        joynames = Input.GetJoystickNames();
        joyname = joynames[0];
        joyname = joyname.Replace(" ", "");
        mapping = (Mapping)AssetDatabase.LoadAssetAtPath("Assets/" + joyname + ".asset", typeof(Mapping));
        if (mapping == null)
        {
            Debug.Log("Default");
            dropdownAxis[0].value = 0;
            dropdownAxis[1].value = 1;
            dropdownAxis[2].value = 2;
            dropdownAxis[3].value = 3;
            dropdownAxis[4].value = 4;
        }
        else
        {
            Debug.Log(joyname);
            dropdownAxis[mapping.pitchAxisNb].value = 0;  // Pitch
            dropdownAxis[mapping.rollAxisNb].value = 1; // Roll
            dropdownAxis[mapping.yawAxisNb].value = 2; // Yaw
            dropdownAxis[mapping.throttleAxisNb].value = 3; // Throttle
        }
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
        TestIfSaveIsPossible(savebutton);
    }

    public void Save()
    {
        mapping =Mapping.CreateInstance<Mapping>();       

        for (int axisNb = 0; axisNb < sliders.Length; axisNb++)
        {
            switch (dropdownAxis[axisNb].value)
            {
                case 0: // Pitch
                    mapping.pitchAxisNb = axisNb;
                    mapping.pitchAxisInversor = inversors[axisNb].isOn;
                    break;
                case 1: // Roll
                    mapping.rollAxisNb = axisNb;
                    mapping.rollAxisInversor = inversors[axisNb].isOn;
                    break;
                case 2: // Yaw
                    mapping.yawAxisNb = axisNb;
                    mapping.yawAxisInversor = inversors[axisNb].isOn;
                    break;
                case 3: // Throttle
                    mapping.throttleAxisNb = axisNb;
                    mapping.throttleAxisInversor = inversors[axisNb].isOn;
                    break;
                case 4: // None
                    break;
                default:
                    break;
            }
        }
        string[] joynames = Input.GetJoystickNames();
        string joyname = joynames[0];
        if (joynames.Length > 0)
        {
            Mapping existingAsset = (Mapping) AssetDatabase.LoadAssetAtPath("Assets/" + joyname.Replace(" ", "") + ".asset", typeof(Mapping));
            if (existingAsset == null)
            {
                AssetDatabase.CreateAsset(mapping, "Assets/" + joyname.Replace(" ", "") + ".asset");
            }
            else
            {
                EditorUtility.CopySerialized(mapping, existingAsset);
            }
        }
    }

    public void TestIfSaveIsPossible(Button button)
    {
        int pitchAxisFound = 0;
        int rollAxisFound = 0;
        int yawAxisFound = 0;
        int throttleAxisFound = 0;
        for (int axisNb = 0; axisNb < sliders.Length; axisNb++)
        {
            switch (dropdownAxis[axisNb].value)
            {
                case 0: // Pitch
                    pitchAxisFound ++;
                    break;
                case 1: // Roll
                    rollAxisFound++;
                    break;
                case 2: // Yaw
                    yawAxisFound++;
                    break;
                case 3: // Throttle
                    throttleAxisFound++;
                    break;
                case 4: // None
                    break;
                default:
                    break;
            }
        }
        if ((pitchAxisFound==1)&& (rollAxisFound == 1)&&(yawAxisFound == 1)&&(throttleAxisFound == 1))
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }


    public void DropDownChange(int value, int axisNb)
    {
        switch (value)
        {
            case 0: // Pitch
                axisNames[axisNb] = "Pitch";
                break;
            case 1: // Roll
                axisNames[axisNb] = "Roll";
                break;
            case 2: // Yaw
                axisNames[axisNb] = "Yaw";
                break;
            case 3: // Throttle
                axisNames[axisNb] = "Throttle";
                break;
            case 4: // None
                axisNames[axisNb] = "None";
                break;
            default:
                break;
        }
    }
}
