using UnityEngine;
using System.Collections;

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

    public bool m_UseAccessory1 = false;
    public bool m_UseAccessory2 = false;

	// Use this for initialization
	void Start ()
    {
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

            BodyPart part = Instantiate(KinectPuppet_Manager.Instance.m_BodyParts[i].m_Parts[0]) as BodyPart;
            m_PuppetParts[i] = part;

            part.transform.SetParent(transform);
        }
    }


    void Save( string name )
    {
        for (int i = 0; i < m_PuppetParts.Length; i++)
        {
            if (m_PuppetParts[i] == null)
                PlayerPrefs.SetInt(name + ".LimbIndex" + i, 99);
            else
            {
                int index = 0;
                int.TryParse(m_PuppetParts[i].name, out index);
                PlayerPrefs.SetInt(name + ".LimbIndex" + i, index);
            }
        }
    }

    void Load( string name )
    {
        for (int i = 0; i < m_PuppetParts.Length; i++)
        {
            int index = PlayerPrefs.GetInt(name + ".LimbIndex" + i);

            if( index != 99 )
            {
                if (m_PuppetParts[i] != null)
                    Destroy(m_PuppetParts[i].gameObject);

                BodyPart part = Instantiate(KinectPuppet_Manager.Instance.m_BodyParts[i].m_Parts[index]) as BodyPart;
                m_PuppetParts[i] = part;

                part.transform.SetParent(transform);
            }
        }
    }

    public void NextPart( BodyPart part )
    {
        KinectPuppet_Manager.BodyPartType type = part.m_BodyPart;
        int typeIndex = (int)type;

        int partIndex = 0;
        int.TryParse("" + (part.name[0]), out partIndex);
        partIndex++;

        if ( partIndex >= KinectPuppet_Manager.Instance.m_BodyParts[typeIndex].m_Parts.Length )
            partIndex = 0;

        Destroy(part.gameObject);

        BodyPart newPart = Instantiate(KinectPuppet_Manager.Instance.m_BodyParts[typeIndex].m_Parts[partIndex]) as BodyPart;
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
