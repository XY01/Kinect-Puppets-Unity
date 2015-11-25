using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;


public class TestNewOSC : MonoBehaviour
{
	public string m_OSCAddress;
	OSCListener m_OSCLookup;
	
	public float incomingFloat = 0;
	
	void Start ()
	{
		
		m_OSCLookup = new OSCListener( m_OSCAddress );
	}
	
	void Update ()
	{
		if( m_OSCLookup.Updated )
			incomingFloat = m_OSCLookup.GetDataAsFloat(0);
	}
}
