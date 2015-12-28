using UnityEngine;
using System.Collections;

// Adjustable pivot
// 
public class BodyPart : MonoBehaviour
{
    Transform m_BasePivot; //transform
    public Transform m_EndPivot; //transform
    public Transform m_LeftPivot; //transform
    public Transform m_RightPivot; //transform

    public Transform m_Parent;

    public KinectPuppet_Manager.BodyPartType m_BodyPart = KinectPuppet_Manager.BodyPartType.Head;
    
    void Awake()
    {
        m_BasePivot = transform;
        name = name + m_BodyPart.ToString();
    }

    public void UpdateTransform()
    {
        transform.position = m_Parent.transform.position;
        transform.rotation = m_Parent.transform.rotation;
    }

    public void UpdateParents( KinectPuppet puppet )
    {
        Transform parent;

        switch( m_BodyPart )
        {
            case KinectPuppet_Manager.BodyPartType.Head:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.UpperTorso].m_EndPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.UpperTorso:
                parent = puppet.transform;
                break;
            case KinectPuppet_Manager.BodyPartType.LowerTorso:
                parent = puppet.transform;
                break;
            case KinectPuppet_Manager.BodyPartType.LegLeftUpper:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.LowerTorso].m_LeftPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.LegLeftLower:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.LegLeftUpper].m_EndPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.FootLeft:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.LegLeftLower].m_EndPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.LegRightUpper:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.LowerTorso].m_RightPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.LegRightLower:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.LegRightUpper].m_EndPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.FootRight:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.LegRightLower].m_EndPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.ArmLeftUpper:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.UpperTorso].m_LeftPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.ArmLeftLower:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.ArmLeftUpper].m_EndPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.ArmRightUpper:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.UpperTorso].m_RightPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.ArmRightLower:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.ArmRightUpper].m_EndPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.Accessory0:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.ArmRightUpper].m_EndPivot;
                break;
            case KinectPuppet_Manager.BodyPartType.Accessory1:
                parent = puppet.m_PuppetParts[(int)KinectPuppet_Manager.BodyPartType.ArmRightUpper].m_EndPivot;
                break;
            default:
                parent = null;
                break;
        }

        m_Parent = parent;
        UpdateTransform();
    }
}
