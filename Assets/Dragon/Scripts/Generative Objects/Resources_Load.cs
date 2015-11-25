using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Resources_Load : MonoBehaviour
{
    public Texture2D[] m_Heads;
    public Texture2D[] m_Wings;
    public Texture2D[] m_Tails;    
    
    public Texture2D[] m_Particles;

    public Material m_HeadMat;
    public Material m_WingMat;
    public Material m_TailMat;

	// Use this for initialization
	void Start ()
    {       
        m_Heads = Resources.LoadAll<Texture2D>("DragonTextures/Heads");
        m_Tails = Resources.LoadAll<Texture2D>("DragonTextures/Tails");
        m_Particles = Resources.LoadAll<Texture2D>("DragonTextures/Particles");
        m_Wings = Resources.LoadAll<Texture2D>("DragonTextures/Wings");        	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if( Input.GetKeyDown( KeyCode.Alpha1 ) )
            SetToDragon(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetToDragon(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetToDragon(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetToDragon(3);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SetToDragon(4);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            SetToDragon(5);
	}

    void SetToDragon( int index )
    {
        m_HeadMat.SetTexture("_MainTex", m_Heads[index]);
        m_WingMat.SetTexture("_MainTex", m_Wings[index]);
        m_TailMat.SetTexture("_MainTex", m_Tails[index]);
    }
}
