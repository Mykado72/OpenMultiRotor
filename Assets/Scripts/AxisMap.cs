using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System.Xml;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AxisMap : MonoBehaviour
{

    enum rcAxis { Pitch, Roll, Yaw, Throttle };
    public Slider[] sliders;
    public Dropdown[] dropdownAxis;
    public string[] axisNames;
    public Toggle[] inversors;
    public Mapping mapping;
    public Button savebutton;
    public string joyname;
    public string[] joynames;
    public bool joystickDetected;
    // Use this for initialization
    void Awake()
    {
        dropdownAxis[0].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[0].value, 0); });
        dropdownAxis[1].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[1].value, 1); });
        dropdownAxis[2].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[2].value, 2); });
        dropdownAxis[3].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[3].value, 3); });
        dropdownAxis[4].onValueChanged.AddListener(delegate { DropDownChange(dropdownAxis[4].value, 4); });
        DetectUSBControler();
    }

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        DetectUSBControler();
        for (int axisNb = 0; axisNb < sliders.Length; axisNb++)
        {
            if (inversors[axisNb].isOn == true)
                sliders[axisNb].value = -Input.GetAxis("axis" + axisNb.ToString());
            else
                sliders[axisNb].value = Input.GetAxis("axis" + axisNb.ToString());
        }
        TestIfSaveIsPossible(savebutton);
    }

    public void Save()
    {
        // mapping = Mapping.CreateInstance<Mapping>();
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
        joynames = Input.GetJoystickNames();
        if (joynames.Length > 0)
        {
            joyname = joynames[0];
            joyname= joyname.Replace(" ", "");
            string json = JsonUtility.ToJson(mapping);
            SaveMappings(joyname, json);
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
                    pitchAxisFound++;
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
        if ((pitchAxisFound == 1) && (rollAxisFound == 1) && (yawAxisFound == 1) && (throttleAxisFound == 1))
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

    private void SaveMappings(string Name, string jsonmap)
    {
        string filePath = Application.persistentDataPath;
        Debug.Log(filePath);
        FileStream fs = new FileStream(filePath+"/"+Name+".ControlerMap", FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(fs);
        writer.Write(jsonmap);
        writer.Close();
        writer.Dispose();
        fs.Close();
        fs.Dispose();
    }

    private Mapping LoadMappings(string Name)
    {
        string filePath = Application.persistentDataPath + "/" + Name + ".ControlerMap";
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            return JsonUtility.FromJson<Mapping>(dataAsJson);
        }
        else
        {
            return null;
        }
    }

    private void DetectUSBControler()
    {
        joynames = Input.GetJoystickNames();
        if (joynames.Length >= 1)
        {
            joyname = joynames[0];
            joyname = joyname.Replace(" ", "");
        }
        else
        {
            joyname = null;
        }
        if ((joyname == null) || (joyname == ""))
        {            
            joystickDetected = false;
        }
        else
        {   
            if (joystickDetected==false) // pour le faire qu'une fois
            {
                if (File.Exists(Application.persistentDataPath + "/" + joyname + ".ControlerMap"))
                {
                    mapping = LoadMappings(joyname);
                    Debug.Log(joyname);
                    dropdownAxis[mapping.pitchAxisNb].value = 0;  // Pitch
                    dropdownAxis[mapping.rollAxisNb].value = 1; // Roll
                    dropdownAxis[mapping.yawAxisNb].value = 2; // Yaw
                    dropdownAxis[mapping.throttleAxisNb].value = 3; // Throttle
                }
                else
                {
                    Debug.Log("pas trouvé de mapping pour " + joyname + ", chargement des valeurs par défaut");
                    dropdownAxis[0].value = 0;
                    dropdownAxis[1].value = 1;
                    dropdownAxis[2].value = 2;
                    dropdownAxis[3].value = 3;
                    dropdownAxis[4].value = 4;
                }
            }
            joystickDetected = true;
        }
    }
}