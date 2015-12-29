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
    public KinectPuppet[] m_Puppets = new KinectPuppet[2];


    public List<KinectPuppetProfile> m_PuppetProfiles = new List<KinectPuppetProfile>();

    List<string> m_SavedPuppetNames = new List<string>();

    public KinectPuppet_GUI m_GUI;

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start ()
    {
        // Load body parts from resources
        m_BodyParts = new BodyPartsArray[System.Enum.GetNames(typeof(BodyPartType)).Length];
        for (int i = 0; i < m_BodyParts.Length; i++)
        {
            BodyPartsArray newParts = new BodyPartsArray();
            newParts.m_Type = (BodyPartType)i;
            newParts.Initialize();

            m_BodyParts[i] = newParts;
        }
               

        // Load profiles
        LoadProfiles();

        // If no profiles loaded then create a default profile
        if (m_PuppetProfiles.Count == 0)
        {
            print("Creating default profile");
            CreateNewProfile("Default puppet", false);
        }

        //Create puppets
        m_Puppets = new KinectPuppet[2];
        for (int i = 0; i < 2; i++)
        {
            KinectPuppet puppet = new GameObject("Puppet " + i).AddComponent<KinectPuppet>();
            m_Puppets[i] = puppet;
            m_Puppets[i].Initialize(m_PuppetProfiles[0]);
        }

        // Turn second puppet off
        m_Puppets[1].gameObject.SetActive(false);


        m_GUI.Init( m_PuppetProfiles );

       //Spawn();
	}


    public void CreateNewProfile(string name)
    {
        CreateNewProfile(name, true);

    }

    void CreateNewProfile(string name, bool loadToPuppet )
    {
        print("Creating profile: " + name);
        if (name == "")
            return;

        bool alreadyExists = false;

        foreach (KinectPuppetProfile p in m_PuppetProfiles)
            if (p.m_Name == name)
                alreadyExists = true;

        if (!alreadyExists)
        {
            KinectPuppetProfile profile = new KinectPuppetProfile(name);
            m_PuppetProfiles.Add(profile);

            if (loadToPuppet)
                m_Puppets[0].LoadProfile(profile);
        }

        m_GUI.UpdateProfileList(m_PuppetProfiles);
    }

    string SaveCount = "ProfileNamesCount";
    string SaveNames = "ProfileNames";
    // Save / Load
    void SaveProfiles()
    {
        print("Saved puppets:");
        PlayerPrefs.SetInt(SaveCount, m_PuppetProfiles.Count);
        for (int i = 0; i < m_PuppetProfiles.Count; i++)
        {
            PlayerPrefs.SetString(SaveNames + i, m_PuppetProfiles[i].m_Name);
            print("--  " + m_PuppetProfiles[i]);
        }

        print("Save ended.");
    }

    void SaveCurrentProfile()
    {
        m_Puppets[0].SaveProfile();
    }

    void LoadProfiles()
    {
        int count = PlayerPrefs.GetInt(SaveCount);

         print("Loading: " + count + " puppets");
         for (int i = 0; i < count; i++)
         {
             string name = PlayerPrefs.GetString(SaveNames + i);
             CreateNewProfile(name, false);
         }

         print("Loading ended. Puppet profiles loaded: " + m_PuppetProfiles.Count );
    }

    void OnApplicationQuit()
    {
        SaveProfiles();
    }

    public void SetSelectedBodyPart( BodyPart part )
    {
        m_SelectedBodyPart = part;
        m_GUI.UpdateSelectedLimb();
        print("Part selected: " + m_SelectedBodyPart.name);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if( Input.GetKeyDown( KeyCode.D))
        {
            SaveCurrentProfile();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            m_Puppets[0].SaveProfile();
            SaveProfiles();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateNewProfile( "Test" + m_PuppetProfiles.Count, true);
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
