using UnityEngine;
using System.Collections;

public class ET_PSysController : MonoBehaviour 
{
	ParticleSystem m_PSys;

	void Start()
	{
		m_PSys = gameObject.GetComponent< ParticleSystem >();
	}

	public void EmitStart()
	{
		if( gameObject.activeSelf )
			m_PSys.enableEmission = true;
	}

	public void EmitStop()
	{
		if( gameObject.activeSelf )
			m_PSys.enableEmission = false;
	}

}
