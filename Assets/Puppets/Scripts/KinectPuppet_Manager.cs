using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Create puppet
// - Press click buttons to cycle through parts
// - Save puppet by name
// Kinect puppet
// - When body part is updated the pivot needs to update
// Recording and video output
// - Naming?

[System.Serializable]
public class BodyPartsArray
{
    public KinectPuppet_Manager.BodyPartType m_Type;
    public BodyPart[] m_Parts;

    public void Initialize()
    {
        m_Parts = (BodyPart[])Resources.LoadAll<BodyPart>("Kinect Puppet/" + m_Type.ToString());

        Debug.Log("Loaded from: " + m_Type.ToString());
    }
}

public class KinectPuppet_Manager : MonoBehaviour 
{
    public enum BodyPartType
    {
        Head,
        UpperTorso,
        LowerTorso,
        LegLeftUpper,
        LegLeftLower,
        FootLeft,
        LegRightUpper,
        LegRightLower,
        FootRight,
        ArmLeftUpper,
        ArmLeftLower,
        ArmRightUpper,
        ArmRightLower,
        Accessory0,
        Accessory1,
    }

    public enum State
    {
        Recording,
        Editing,
        Menu,
    }

    public State m_State = State.Menu;

    static KinectPuppet_Manager m_Instance { get; set; }
    public static KinectPuppet_Manager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public BodyPartsArray[] m_BodyParts;
    //public BodyPart[] m_PuppetParts;

    public BodyPart m_SelectedBodyPart;
    List< KinectPuppet > m_Puppets = new List<KinectPuppet>();

    List<string> m_SavedPuppetNames = new List<string>();

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start ()
    {
        m_BodyParts = new BodyPartsArray[System.Enum.GetNames(typeof(BodyPartType)).Length];

        for (int i = 0; i < m_BodyParts.Length; i++)
        {
            BodyPartsArray newParts = new BodyPartsArray();
            newParts.m_Type = (BodyPartType)i;
            newParts.Initialize();

            m_BodyParts[i] = newParts;
        }

        CreatePuppet();

       //Spawn();
	}

    void Save()
    {
        PlayerPrefs.SetInt("m_SavedPuppetNames.Count", m_SavedPuppetNames.Count);
        for (int i = 0; i < m_SavedPuppetNames.Count; i++)
        {
            PlayerPrefs.SetString("Name" + i, m_SavedPuppetNames[i]);
        }
    }

    void Load()
    {
        int count = PlayerPrefs.GetInt("m_SavedPuppetNames.Count");
        for (int i = 0; i < count; i++)
        {
           m_SavedPuppetNames.Add( PlayerPrefs.GetString("Name" + i) );
        }
    }

    public void CreatePuppet()
    {
        if (m_Puppets.Count >= 2)
            return;

        KinectPuppet puppet = new GameObject("Puppet " + m_Puppets.Count).AddComponent < KinectPuppet >();
        m_Puppets.Add(puppet);
    }

    public void RemovePuppet( KinectPuppet puppet )
    {
        m_Puppets.Remove(puppet);
        Destroy(puppet);
    }

    public void SetSelectedBodyPart( BodyPart part )
    {
        m_SelectedBodyPart = part;
        print("Part selected: " + m_SelectedBodyPart.name);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if( Input.GetKeyDown( KeyCode.A))
        {
            if (m_SelectedBodyPart != null )
                m_Puppets[0].NextPart(m_SelectedBodyPart);
        }
	}

    /*
    void Spawn()
    {
        m_PuppetParts = new BodyPart[m_BodyParts.Length];
        for (int i = 0; i < m_BodyParts.Length; i++)
        {
            if (m_BodyParts[i].m_Parts == null || m_BodyParts[i].m_Parts.Length == 0 )
                continue;

            BodyPart part = Instantiate(m_BodyParts[i].m_Parts[0]) as BodyPart;            
            m_PuppetParts[i] = part;    
        }
    }
    */

    void ChanegSelectedBodyPart()
    {

    }


    BodyPart[] GetBodyParts( BodyPartType type )
    {
        for (int i = 0; i < m_BodyParts.Length; i++)
        {
            if (m_BodyParts[i].m_Type == type)
                return m_BodyParts[i].m_Parts;
        }

        return null;
    }
}
