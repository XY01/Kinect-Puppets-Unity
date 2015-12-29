using UnityEngine;
using System.Collections;

[System.Serializable]
public class KinectPuppetProfile
{
    public string   m_Name;
    public int[]    m_PartIndecies = new int[System.Enum.GetNames(typeof(KinectPuppet_Manager.BodyPartType)).Length];

    public KinectPuppetProfile()
    {
        m_Name = "Default Puppet";
        for (int i = 0; i < m_PartIndecies.Length; i++)
        {
            m_PartIndecies[i] = 0;
        }

        Save();
    }

    public KinectPuppetProfile( string name )
    {
        m_Name = name;
        Load();
    }

    public void Load( string name )
    {
        m_Name = name;
        Load();
    }

    public void Load()
    {
        for (int i = 0; i < m_PartIndecies.Length; i++)
        {
            m_PartIndecies[i] = PlayerPrefs.GetInt(m_Name + ".LimbIndex" + i);
        }

        Save();
    }

    public void Save( )
    {
        Debug.Log("Saving: " + m_Name);
        for (int i = 0; i < m_PartIndecies.Length; i++)
        {
            Debug.Log("Saving limb index: " + m_PartIndecies[i] );
            PlayerPrefs.SetInt(m_Name + ".LimbIndex" + i, m_PartIndecies[i]);                     
        }
    }
}

public class KinectPuppet : MonoBehaviour
{
    Transform[] m_Pivots;

    /*
    public Transform m_Head;
    public Transform m_Upper_Torso;
    public Transform m_Lower_Torso;

    public Transform m_Arm_Upper_Left;
    public Transform m_Arm_Lower_Left;

    public Transform m_Arm_Upper_Right;
    public Transform m_Arm_Lower_Right;

    public Transform m_Leg_Upper_Left;
    public Transform m_Leg_Lower_Left;
    public Transform m_Foot_Left;

    public Transform m_Leg_Upper_Right;
    public Transform m_Leg_Lower_Right;
    public Transform m_Foot_Right;
     * */

    // Array of body parts
    public BodyPart[] m_PuppetParts;

    public KinectPuppetProfile m_Profile;

    public bool m_UseAccessory1 = false;
    public bool m_UseAccessory2 = false;

	// Use this for initialization
	public void Initialize ( KinectPuppetProfile prof )
    {
        m_Profile = prof;

        m_PuppetParts = new BodyPart[KinectPuppet_Manager.Instance.m_BodyParts.Length];
        
        Spawn();
	}
	
	// Update is called once per frame
	void Update () 
    {
        for (int i = 0; i < m_PuppetParts.Length; i++)
        {
            if (m_PuppetParts[i] == null )
                continue;

            m_PuppetParts[i].UpdateParents( this );
        }
	}

    // Spawn the puppet
    void Spawn()
    {
        m_PuppetParts = new BodyPart[KinectPuppet_Manager.Instance.m_BodyParts.Length];
        for (int i = 0; i < m_PuppetParts.Length; i++)
        {
            if (KinectPuppet_Manager.Instance.m_BodyParts[i].m_Parts == null || KinectPuppet_Manager.Instance.m_BodyParts[i].m_Parts.Length == 0)
                continue;

           
            BodyPart part = Instantiate(KinectPuppet_Manager.Instance.m_BodyParts[i].m_Parts[m_Profile.m_PartIndecies[i]]) as BodyPart;
            part.m_Puppet = this;
            m_PuppetParts[i] = part;

            part.transform.SetParent(transform);
        }
    }

   
    public void SaveProfile( )
    {
        m_Profile.Save();
    }

    public void LoadProfile( KinectPuppetProfile profile )
    {
        print("LOADING: " + profile.m_Name );

        m_Profile = profile;
        m_Profile.Load();       

        for (int i = 0; i < m_PuppetParts.Length; i++)
        {
            int index = m_Profile.m_PartIndecies[i];

            if( index != 99 )
            {
                if (m_PuppetParts[i] != null)
                    Destroy(m_PuppetParts[i].gameObject);

                if (index >= KinectPuppet_Manager.Instance.m_BodyParts[i].m_Parts.Length)
                    continue;

                BodyPart part = Instantiate(KinectPuppet_Manager.Instance.m_BodyParts[i].m_Parts[index]) as BodyPart;
                m_PuppetParts[i] = part;
                part.m_Puppet = this;

                part.transform.SetParent(transform);
            }
        }

        print("Loaded: " + profile.m_Name);
    }

    public void NextPart( BodyPart part )
    {
        KinectPuppet_Manager.BodyPartType type = part.m_BodyPart;
        int typeIndex = (int)type;

        int partIndex = m_Profile.m_PartIndecies[typeIndex];
        int.TryParse("" + (part.name[0]), out partIndex);
        partIndex++;

        if ( partIndex >= KinectPuppet_Manager.Instance.m_BodyParts[typeIndex].m_Parts.Length )
            partIndex = 0;

        m_Profile.m_PartIndecies[typeIndex] = partIndex;

        Destroy(part.gameObject);

        BodyPart newPart = Instantiate(KinectPuppet_Manager.Instance.m_BodyParts[typeIndex].m_Parts[partIndex]) as BodyPart;
        newPart.m_Puppet = this;
        m_PuppetParts[typeIndex] = newPart;

        newPart.transform.SetParent(transform);

        KinectPuppet_Manager.Instance.SetSelectedBodyPart(newPart);

        print("Changing parts " + typeIndex + "    " + partIndex);
    }

    public void PrevPart(BodyPart part)
    {
        KinectPuppet_Manager.BodyPartType type = part.m_BodyPart;
        int typeIndex = (int)type;

        int partIndex = 0;
        int.TryParse("" + (part.name[0]), out partIndex);
        partIndex--;

        if (partIndex < 0)
            partIndex = KinectPuppet_Manager.Instance.m_BodyParts[typeIndex].m_Parts.Length - 1;

        Destroy(part.gameObject);

        BodyPart newPart = Instantiate(KinectPuppet_Manager.Instance.m_BodyParts[typeIndex].m_Parts[partIndex]) as BodyPart;
        newPart.m_Puppet = this;
        m_PuppetParts[typeIndex] = newPart;

        newPart.transform.SetParent(transform);

        KinectPuppet_Manager.Instance.SetSelectedBodyPart(newPart);

        print("Changing parts " + typeIndex + "    " + partIndex);
    }
    

    /*
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(m_Head.position, .2f );

        Gizmos.DrawWireSphere(m_Upper_Torso.position, .2f);
        Gizmos.DrawWireSphere(m_Lower_Torso.position, .2f);

        Gizmos.DrawWireSphere(m_Arm_Upper_Left.position, .2f);
        Gizmos.DrawWireSphere(m_Arm_Lower_Left.position, .2f);

        Gizmos.DrawWireSphere(m_Arm_Upper_Right.position, .2f);
        Gizmos.DrawWireSphere(m_Arm_Lower_Right.position, .2f);

        Gizmos.DrawWireSphere(m_Leg_Upper_Left.position, .2f);
        Gizmos.DrawWireSphere(m_Leg_Lower_Left.position, .2f);
        Gizmos.DrawWireSphere(m_Foot_Left.position, .2f);

        Gizmos.DrawWireSphere(m_Leg_Upper_Right.position, .2f);
        Gizmos.DrawWireSphere(m_Leg_Lower_Right.position, .2f);
        Gizmos.DrawWireSphere(m_Foot_Right.position, .2f);
    }
     * */
}
