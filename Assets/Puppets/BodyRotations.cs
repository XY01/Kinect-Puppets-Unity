using UnityEngine;
using System.Collections;

public class BodyRotations : MonoBehaviour
{
    Transform m_Head;
    Transform m_Neck;

    Transform m_SpineBase;
    Transform m_SpineMid;

    Transform m_ShoulderLeft;
    Transform m_ElbowLeft;
    Transform m_WristLeft;

    Transform m_ShoulderRight;
    Transform m_ElbowRight;
    Transform m_WristRight;


    KinectPuppet m_Puppet;

	// Use this for initialization
	void Start () 
    {
        m_Head = GameObject.Find( name + "/Head").transform;
        m_Neck = GameObject.Find(name + "/Neck").transform;
        m_SpineBase = GameObject.Find(name + "/SpineBase").transform;
        m_SpineMid = GameObject.Find(name + "/SpineMid").transform;

        m_ShoulderLeft = GameObject.Find(name + "/ShoulderLeft").transform;
        m_ElbowLeft = GameObject.Find(name + "/ElbowLeft").transform;
        m_WristLeft = GameObject.Find(name + "/WristLeft").transform;

        m_ShoulderRight = GameObject.Find(name + "/ShoulderRight").transform;
        m_ElbowRight = GameObject.Find(name + "/ElbowRight").transform;
        m_WristRight = GameObject.Find(name + "/WristRight").transform;

        m_Puppet = GameObject.Find("Puppet").GetComponent< KinectPuppet >();
	}
	
    /*
	// Update is called once per frame
	void Update ()
    {
        // Neck
        float neckRotation = GetRotation(m_Neck,m_Head,  Vector2.right, -90);
        m_Puppet.m_Head.transform.SetLocalRotZ( neckRotation );

        //Upper torso
        float upperTorso = GetRotation(m_SpineMid, m_Neck, Vector2.right, -90);
        m_Puppet.m_Upper_Torso.transform.SetLocalRotZ(upperTorso);

        //Lower torso
        float lowerTorso = GetRotation(m_SpineMid, m_SpineBase, Vector2.right, -90);
        m_Puppet.m_Lower_Torso.transform.SetLocalRotZ(lowerTorso);

        //Left arm
        //Upper
        float upperArmLeft = GetRotation(m_ShoulderLeft, m_ElbowLeft, Vector2.right, -90);
        m_Puppet.m_Arm_Upper_Left.transform.SetLocalRotZ(upperArmLeft);

        //Lower
        float lowerArmLeft = GetRotation(m_ElbowLeft, m_WristLeft, Vector2.right, -90);
        m_Puppet.m_Arm_Lower_Left.transform.SetLocalRotZ(lowerArmLeft);
       
	}
     * */

    float GetRotation( Transform t1, Transform t2, Vector2 referenceAngle, float offset )
    {
        Vector2 vectorTo = t2.position - t1.position;
        float roation = Vector2.Angle(referenceAngle, vectorTo);
        return roation + offset;
    }
}
